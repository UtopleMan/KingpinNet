Register-ArgumentCompleter -CommandName {{AppName}} -ScriptBlock {
    param($commandName, $wordToComplete, $cursorPosition)
    {{AppPath}}{{AppName}} suggest "$wordToComplete" | ForEach-Object {
        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
    }
}
