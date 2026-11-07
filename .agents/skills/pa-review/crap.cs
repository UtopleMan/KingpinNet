#!/usr/bin/env dotnet
#:package Microsoft.CodeAnalysis.CSharp@5.6.0

// CRAP score for one method: complexity² × (1 − coverage)³ + complexity.
// Usage: dotnet run crap.cs -- <file-or-directory> <Method|Type.Method|Namespace.Type.Method> [--coverage <cobertura.xml>] [--threshold <n>]
// Without --coverage it uses the newest coverage.cobertura.xml under the current directory,
// as written by: dotnet test --collect:"XPlat Code Coverage"

using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: dotnet run crap.cs -- <file-or-directory> <Method|Type.Method> [--coverage <cobertura.xml>] [--threshold <n>]");
    return 2;
}

var searchRoot = Path.GetFullPath(args[0]);
var query = args[1];
var coveragePath = Option("--coverage") ?? FindNewestCoverage(Directory.GetCurrentDirectory());
var threshold = double.Parse(Option("--threshold") ?? "30", System.Globalization.CultureInfo.InvariantCulture);

var methods = SourceFiles(searchRoot)
    .SelectMany(MethodsIn)
    .Where(method => method.FullName == query || method.FullName.EndsWith("." + query, StringComparison.Ordinal))
    .ToList();

if (methods.Count == 0)
{
    Console.Error.WriteLine($"No method named '{query}' under {searchRoot}");
    return 3;
}

var coverage = coveragePath is null ? null : CoverageReport.Load(coveragePath);
Console.WriteLine(coveragePath is null
    ? "Coverage: none found, so every method counts as uncovered (run: dotnet test --collect:\"XPlat Code Coverage\")"
    : $"Coverage: {coveragePath}");

var isAnyOverThreshold = false;
foreach (var method in methods)
{
    var (covered, coverable) = coverage?.LinesIn(method.File, method.StartLine, method.EndLine) ?? (0, 0);
    var ratio = coverable == 0 ? 0 : (double)covered / coverable;
    var crap = Math.Pow(method.Complexity, 2) * Math.Pow(1 - ratio, 3) + method.Complexity;
    var isOverThreshold = crap > threshold;
    isAnyOverThreshold |= isOverThreshold;

    var coverageText = coverable == 0 ? "no coverage data" : $"{ratio:P1} ({covered}/{coverable} lines)";
    Console.WriteLine();
    Console.WriteLine($"{method.FullName}  {Path.GetRelativePath(Directory.GetCurrentDirectory(), method.File)}:{method.StartLine}");
    Console.WriteLine($"  complexity {method.Complexity}, coverage {coverageText}, CRAP {crap:F1} {(isOverThreshold ? $"OVER {threshold}" : "ok")}");
}

return isAnyOverThreshold ? 1 : 0;

string? Option(string name)
{
    var index = Array.IndexOf(args, name);
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

static IEnumerable<string> SourceFiles(string root)
{
    if (File.Exists(root))
    {
        return [root];
    }

    return Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
        .Where(path => !IsInIgnoredDirectory(path));
}

static string? FindNewestCoverage(string root)
{
    return Directory.EnumerateFiles(root, "coverage.cobertura.xml", SearchOption.AllDirectories)
        .Where(path => !IsInIgnoredDirectory(path))
        .OrderByDescending(File.GetLastWriteTimeUtc)
        .FirstOrDefault();
}

static bool IsInIgnoredDirectory(string path)
{
    var segments = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    return segments.Any(segment => segment is "bin" or "obj" or ".git" or "node_modules");
}

static IEnumerable<MethodInfo> MethodsIn(string file)
{
    var root = CSharpSyntaxTree.ParseText(File.ReadAllText(file), path: file).GetRoot();
    foreach (var node in root.DescendantNodes())
    {
        var name = node switch
        {
            MethodDeclarationSyntax method => method.Identifier.Text,
            ConstructorDeclarationSyntax constructor => constructor.Identifier.Text,
            LocalFunctionStatementSyntax function => function.Identifier.Text,
            _ => null,
        };
        if (name is null)
        {
            continue;
        }

        var lines = node.GetLocation().GetLineSpan();
        yield return new MethodInfo(
            ContainerName(node) + name,
            Path.GetFullPath(file),
            lines.StartLinePosition.Line + 1,
            lines.EndLinePosition.Line + 1,
            Complexity(node));
    }
}

// Namespaces, types and enclosing methods, outermost first, each followed by a dot.
static string ContainerName(SyntaxNode node)
{
    var names = node.Ancestors()
        .Select(ancestor => ancestor switch
        {
            BaseNamespaceDeclarationSyntax ns => ns.Name.ToString(),
            BaseTypeDeclarationSyntax type => type.Identifier.Text,
            MethodDeclarationSyntax method => method.Identifier.Text,
            LocalFunctionStatementSyntax function => function.Identifier.Text,
            _ => null,
        })
        .OfType<string>()
        .Reverse();
    return string.Concat(names.Select(name => name + "."));
}

// Cyclomatic complexity: 1 plus one per decision point. Nested local functions are
// scored on their own, so their branches are left out of the method that holds them.
static int Complexity(SyntaxNode method)
{
    var decisions = method
        .DescendantNodes(child => child == method || child is not LocalFunctionStatementSyntax)
        .Count(node => node switch
        {
            IfStatementSyntax or WhileStatementSyntax or DoStatementSyntax or ForStatementSyntax => true,
            CommonForEachStatementSyntax or CatchClauseSyntax or ConditionalExpressionSyntax => true,
            CaseSwitchLabelSyntax or CasePatternSwitchLabelSyntax or ConditionalAccessExpressionSyntax => true,
            SwitchExpressionArmSyntax arm => arm.Pattern is not DiscardPatternSyntax,
            BinaryExpressionSyntax binary => binary.Kind() is SyntaxKind.LogicalAndExpression
                or SyntaxKind.LogicalOrExpression
                or SyntaxKind.CoalesceExpression,
            AssignmentExpressionSyntax assignment => assignment.IsKind(SyntaxKind.CoalesceAssignmentExpression),
            BinaryPatternSyntax => true,
            _ => false,
        });
    return 1 + decisions;
}

record MethodInfo(string FullName, string File, int StartLine, int EndLine, int Complexity);

// Line hits per source file from a Cobertura report. Async methods, lambdas and partial
// classes each get their own <class> element, so hits for one line are merged by maximum.
class CoverageReport(Dictionary<string, Dictionary<int, int>> hitsByFile)
{
    public static CoverageReport Load(string path)
    {
        var document = XDocument.Load(path);
        var sources = document.Descendants("source").Select(source => source.Value).ToList();
        var hitsByFile = new Dictionary<string, Dictionary<int, int>>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in document.Descendants("class"))
        {
            var file = (string?)type.Attribute("filename");
            if (file is null)
            {
                continue;
            }

            var hits = hitsByFile.TryGetValue(file, out var existing) ? existing : hitsByFile[file] = [];
            foreach (var line in type.Elements("lines").Elements("line"))
            {
                var number = (int)line.Attribute("number")!;
                hits[number] = Math.Max(hits.GetValueOrDefault(number), (int)line.Attribute("hits")!);
            }
        }

        return new CoverageReport(hitsByFile.ToDictionary(
            entry => Resolve(entry.Key, sources),
            entry => entry.Value,
            StringComparer.OrdinalIgnoreCase));
    }

    public (int Covered, int Coverable) LinesIn(string file, int startLine, int endLine)
    {
        if (!hitsByFile.TryGetValue(file, out var hits))
        {
            return (0, 0);
        }

        var lines = hits.Where(hit => hit.Key >= startLine && hit.Key <= endLine).ToList();
        return (lines.Count(hit => hit.Value > 0), lines.Count);
    }

    // Cobertura filenames are absolute, or relative to one of the report's <source> roots.
    static string Resolve(string file, List<string> sources)
    {
        if (Path.IsPathRooted(file))
        {
            return Path.GetFullPath(file);
        }

        var match = sources.Select(source => Path.Combine(source, file)).FirstOrDefault(File.Exists);
        return Path.GetFullPath(match ?? file);
    }
}
