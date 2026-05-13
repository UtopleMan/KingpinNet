namespace KingpinNet.AiHelp;

public sealed record ExitCode(int Code, string Description);
public sealed record Example(string Intent, string Command);
public sealed record Note(string Text);
public sealed record Prefer(string Rule, string When, string Why);
public sealed record Avoid(string Rule, string Unless, string Why);
