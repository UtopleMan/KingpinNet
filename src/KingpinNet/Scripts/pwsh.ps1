Register-ArgumentCompleter -Native -CommandName {{AppName}} -ScriptBlock {
    param($commandName, $wordToComplete, $cursorPosition)
        {{AppName}} suggest --position $cursorPosition "$wordToComplete" | ForEach-Object {
           [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
        }
}
