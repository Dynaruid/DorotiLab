function Resolve-DorotiPython {
    # Resolve once; the wrapper and its child use this same absolute interpreter.
    foreach ($name in @('python3', 'python')) {
        $command = Get-Command $name -CommandType Application -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($null -ne $command -and $command.Source -notmatch '[\\/]WindowsApps[\\/]') { return [IO.Path]::GetFullPath($command.Source) }
    }
    throw 'Python 3 is required for this operation; install python3 or python and add it to PATH.'
}
