#Requires -Version 7.0
param(
    [ValidateSet('Chrome', 'Edge')]
    [string] $Browser = 'Chrome',

    [ValidateRange(40, 2400)]
    [int] $SampleCount = 300,

    [ValidateRange(8, 100)]
    [int] $SampleIntervalMilliseconds = 16,

    [ValidateRange(0, 120)]
    [int] $PostResizeObservationSeconds = 10,

    [switch] $Visible,

    [switch] $ExerciseCompatibilityMatrix,

    [string] $EvidenceDirectory = (Join-Path $PSScriptRoot '../validation/evidence/web')
)

$ErrorActionPreference = 'Stop'
$dorotiRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$repoRoot = (Resolve-Path (Join-Path $dorotiRoot '..')).Path
$webProject = Join-Path $repoRoot 'DorotiDemoApp/web/DorotiDemoApp.Web.csproj'
$evidenceRoot = [IO.Path]::GetFullPath($EvidenceDirectory)
$allowedEvidenceRoot = [IO.Path]::GetFullPath((Join-Path $dorotiRoot 'validation/evidence/web'))
if (-not $evidenceRoot.StartsWith($allowedEvidenceRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Evidence directory must stay under $allowedEvidenceRoot"
}
[IO.Directory]::CreateDirectory($evidenceRoot) | Out-Null

$tempParent = [IO.Path]::GetFullPath((Join-Path $repoRoot '.doroti/tmp'))
[IO.Directory]::CreateDirectory($tempParent) | Out-Null
$runId = "web-flk-$((Get-Date).ToString('yyyyMMdd-HHmmss'))-$([Guid]::NewGuid().ToString('N').Substring(0, 8))"
$runRoot = Join-Path $tempParent $runId
$publishRoot = Join-Path $runRoot 'publish'
$profileRoot = Join-Path $runRoot 'browser-profile'
[IO.Directory]::CreateDirectory($runRoot) | Out-Null
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$scenarioSuffix = if ($ExerciseCompatibilityMatrix) { '-matrix' } else { '' }
$artifactStem = "web-resize-$($Browser.ToLowerInvariant())$scenarioSuffix-$stamp"
$rawPath = Join-Path $evidenceRoot "$artifactStem.raw.json"
$summaryPath = Join-Path $evidenceRoot "$artifactStem.summary.json"
$finalScreenshotPath = Join-Path $evidenceRoot "$artifactStem.final.png"
$firstBlankScreenshotPath = Join-Path $evidenceRoot "$artifactStem.first-blank.png"
$subprocessTimeout = [TimeSpan]::FromMinutes(20)

function Remove-ScopedDirectory([string] $Path) {
    $resolved = [IO.Path]::GetFullPath($Path)
    if (-not $resolved.StartsWith($tempParent, [StringComparison]::OrdinalIgnoreCase) -or $resolved -eq $tempParent) {
        throw "Refusing to remove unscoped path $resolved"
    }
    if (Test-Path -LiteralPath $resolved) { Remove-Item -LiteralPath $resolved -Recurse -Force }
}

function Invoke-Process(
    [string] $FileName,
    [string[]] $Arguments,
    [string] $WorkingDirectory,
    [TimeSpan] $Timeout = $subprocessTimeout) {
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $FileName
    $start.WorkingDirectory = $WorkingDirectory
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    foreach ($argument in $Arguments) { [void] $start.ArgumentList.Add($argument) }
    $process = [Diagnostics.Process]::Start($start)
    if ($null -eq $process) { throw "Could not start $FileName" }
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit([int]$Timeout.TotalMilliseconds)) {
        $process.Kill($true)
        throw "$FileName exceeded the 20-minute subprocess timeout."
    }
    $output = $stdout.GetAwaiter().GetResult()
    $errorOutput = $stderr.GetAwaiter().GetResult()
    if ($process.ExitCode -ne 0) {
        throw "$FileName failed with exit code $($process.ExitCode).`n$output`n$errorOutput"
    }
    return [pscustomobject]@{ Output = $output; Error = $errorOutput }
}

function Start-BackgroundProcess([string] $FileName, [string[]] $Arguments, [string] $WorkingDirectory) {
    $start = [Diagnostics.ProcessStartInfo]::new()
    $start.FileName = $FileName
    $start.WorkingDirectory = $WorkingDirectory
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    foreach ($argument in $Arguments) { [void] $start.ArgumentList.Add($argument) }
    $process = [Diagnostics.Process]::Start($start)
    if ($null -eq $process) { throw "Could not start $FileName" }
    $process.BeginOutputReadLine()
    $process.BeginErrorReadLine()
    return $process
}

function Get-FreeTcpPort {
    $listener = [Net.Sockets.TcpListener]::new([Net.IPAddress]::Loopback, 0)
    $listener.Start()
    try { return ([Net.IPEndPoint]$listener.LocalEndpoint).Port }
    finally { $listener.Stop() }
}

function Wait-Http([string] $Url, [TimeSpan] $Timeout) {
    $deadline = [DateTime]::UtcNow + $Timeout
    do {
        try {
            $response = Invoke-WebRequest -UseBasicParsing -Uri $Url -TimeoutSec 2
            if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 500) { return }
        } catch { Start-Sleep -Milliseconds 100 }
    } while ([DateTime]::UtcNow -lt $deadline)
    throw "Timed out waiting for $Url"
}

function Find-Browser([string] $Name) {
    $candidates = if ($Name -eq 'Chrome') {
        @(
            (Join-Path $env:ProgramFiles 'Google/Chrome/Application/chrome.exe'),
            (Join-Path ${env:ProgramFiles(x86)} 'Google/Chrome/Application/chrome.exe'),
            (Join-Path $env:LOCALAPPDATA 'Google/Chrome/Application/chrome.exe'))
    } else {
        @(
            (Join-Path ${env:ProgramFiles(x86)} 'Microsoft/Edge/Application/msedge.exe'),
            (Join-Path $env:ProgramFiles 'Microsoft/Edge/Application/msedge.exe'))
    }
    $path = $candidates | Where-Object { $_ -and (Test-Path -LiteralPath $_ -PathType Leaf) } | Select-Object -First 1
    if (-not $path) { throw "$Name executable was not found." }
    return $path
}

$script:cdpId = 0
$script:cdpEvents = [Collections.Generic.List[object]]::new()

function Send-Cdp([Net.WebSockets.ClientWebSocket] $Socket, [string] $Method, [hashtable] $Parameters = @{}) {
    $id = ++$script:cdpId
    $json = @{ id = $id; method = $Method; params = $Parameters } | ConvertTo-Json -Depth 20 -Compress
    $bytes = [Text.Encoding]::UTF8.GetBytes($json)
    [void]($Socket.SendAsync(
        [ArraySegment[byte]]::new($bytes),
        [Net.WebSockets.WebSocketMessageType]::Text,
        $true,
        [Threading.CancellationToken]::None).GetAwaiter().GetResult())
    while ($true) {
        $stream = [IO.MemoryStream]::new()
        do {
            $buffer = [byte[]]::new(1024 * 1024)
            $receive = $Socket.ReceiveAsync(
                [ArraySegment[byte]]::new($buffer),
                [Threading.CancellationToken]::None).GetAwaiter().GetResult()
            if ($receive.MessageType -eq [Net.WebSockets.WebSocketMessageType]::Close) {
                throw 'Chrome DevTools Protocol socket closed unexpectedly.'
            }
            $stream.Write($buffer, 0, $receive.Count)
        } while (-not $receive.EndOfMessage)
        $message = [Text.Encoding]::UTF8.GetString($stream.ToArray()) | ConvertFrom-Json -Depth 30
        if ($message.id -eq $id) {
            if ($message.error) { throw "CDP $Method failed: $($message.error | ConvertTo-Json -Compress)" }
            return $message.result
        }
        if ($message.method) { $script:cdpEvents.Add($message) }
    }
}

function Invoke-JavaScript([Net.WebSockets.ClientWebSocket] $Socket, [string] $Expression) {
    $result = Send-Cdp $Socket 'Runtime.evaluate' @{
        expression = $Expression
        returnByValue = $true
        awaitPromise = $true
    }
    if ($result.exceptionDetails) {
        throw "Browser evaluation failed: $($result.exceptionDetails.text)"
    }
    return $result.result.value
}

function Wait-JavaScriptTrue(
    [Net.WebSockets.ClientWebSocket] $Socket,
    [string] $Expression,
    [TimeSpan] $Timeout,
    [string] $Failure) {
    $deadline = [DateTime]::UtcNow + $Timeout
    do {
        if ([bool](Invoke-JavaScript $Socket $Expression)) { return }
        Start-Sleep -Milliseconds 50
    } while ([DateTime]::UtcNow -lt $deadline)
    throw $Failure
}

function Capture-Screenshot([Net.WebSockets.ClientWebSocket] $Socket) {
    $result = Send-Cdp $Socket 'Page.captureScreenshot' @{
        format = 'png'
        fromSurface = $true
        captureBeyondViewport = $false
    }
    return [Convert]::FromBase64String([string]$result.data)
}

function Test-BlankScreenshot([byte[]] $Bytes) {
    $stream = [IO.MemoryStream]::new($Bytes, $false)
    $bitmap = [Drawing.Bitmap]::new($stream)
    try {
        $different = 0
        for ($row = 1; $row -lt 20; $row++) {
            for ($column = 1; $column -lt 20; $column++) {
                $pixel = $bitmap.GetPixel(
                    [Math]::Min($bitmap.Width - 1, [Math]::Floor($bitmap.Width * $column / 20.0)),
                    [Math]::Min($bitmap.Height - 1, [Math]::Floor($bitmap.Height * $row / 20.0)))
                if ([Math]::Abs($pixel.R - 20) -gt 10 -or
                    [Math]::Abs($pixel.G - 18) -gt 10 -or
                    [Math]::Abs($pixel.B - 24) -gt 10) { $different++ }
            }
        }
        return $different -lt 4
    }
    finally {
        $bitmap.Dispose()
        $stream.Dispose()
    }
}

function Get-AppBarLogicalHeight([byte[]] $Bytes, [double] $DeviceScaleFactor) {
    $stream = [IO.MemoryStream]::new($Bytes, $false)
    $bitmap = [Drawing.Bitmap]::new($stream)
    try {
        $first = -1
        $last = -1
        $gaps = 0
        $maxRow = [Math]::Min($bitmap.Height - 1, [Math]::Ceiling(240 * $DeviceScaleFactor))
        for ($row = 0; $row -le $maxRow; $row++) {
            $purple = 0
            for ($sample = 1; $sample -le 48; $sample++) {
                $column = [Math]::Min($bitmap.Width - 1, [Math]::Floor($bitmap.Width * $sample / 49.0))
                $pixel = $bitmap.GetPixel($column, $row)
                if ($pixel.B - $pixel.R -gt 25 -and $pixel.B - $pixel.G -gt 35 -and
                    $pixel.R -gt 45 -and $pixel.B -gt 100) { $purple++ }
            }
            if ($purple -ge 24) {
                if ($first -lt 0) { $first = $row }
                $last = $row
                $gaps = 0
            } elseif ($first -ge 0 -and ++$gaps -gt 2) {
                break
            }
        }
        if ($first -lt 0 -or $last -lt $first) { return $null }
        return ($last - $first + 1) / [Math]::Max(0.01, $DeviceScaleFactor)
    }
    finally {
        $bitmap.Dispose()
        $stream.Dispose()
    }
}

function Get-CircularControlAspect([byte[]] $Bytes, [double] $DeviceScaleFactor) {
    $stream = [IO.MemoryStream]::new($Bytes, $false)
    $bitmap = [Drawing.Bitmap]::new($stream)
    try {
        $minimumRun = [Math]::Max(3, [Math]::Floor(4 * $DeviceScaleFactor))
        $maximumRun = [Math]::Max($minimumRun, [Math]::Ceiling(90 * $DeviceScaleFactor))
        $firstRow = [Math]::Min($bitmap.Height - 1, [Math]::Floor(80 * $DeviceScaleFactor))
        $lastRow = [Math]::Min($bitmap.Height - 1, [Math]::Floor($bitmap.Height * 0.78))
        $lastColumn = [Math]::Min($bitmap.Width - 1, [Math]::Floor($bitmap.Width * 0.48))
        $runs = [Collections.Generic.List[object]]::new()
        for ($row = $firstRow; $row -le $lastRow; $row++) {
            $start = -1
            for ($column = 0; $column -le $lastColumn + 1; $column++) {
                $isLavender = $false
                if ($column -le $lastColumn) {
                    $pixel = $bitmap.GetPixel($column, $row)
                    $isLavender = $pixel.B -ge 185 -and $pixel.R -ge 115 -and
                        $pixel.B - $pixel.R -ge 20 -and $pixel.B - $pixel.G -ge 28
                }
                if ($isLavender -and $start -lt 0) {
                    $start = $column
                } elseif (-not $isLavender -and $start -ge 0) {
                    $width = $column - $start
                    if ($width -ge $minimumRun -and $width -le $maximumRun) {
                        $runs.Add([pscustomobject]@{
                            row = $row
                            left = $start
                            right = $column - 1
                            width = $width
                        })
                    }
                    $start = -1
                }
            }
        }

        $components = [Collections.Generic.List[object]]::new()
        foreach ($run in $runs) {
            $component = $components | Where-Object {
                $run.row - $_.lastRow -le 2 -and
                $run.left -le $_.right + 2 -and $run.right -ge $_.left - 2
            } | Select-Object -First 1
            if ($null -eq $component) {
                $component = [pscustomobject]@{
                    firstRow = $run.row
                    lastRow = $run.row
                    left = $run.left
                    right = $run.right
                    pixels = [long]$run.width
                    maxRun = $run.width
                }
                $components.Add($component)
            } else {
                $component.lastRow = $run.row
                $component.left = [Math]::Min($component.left, $run.left)
                $component.right = [Math]::Max($component.right, $run.right)
                $component.pixels += $run.width
                $component.maxRun = [Math]::Max($component.maxRun, $run.width)
            }
        }

        $candidates = foreach ($component in $components) {
            $width = $component.right - $component.left + 1
            $height = $component.lastRow - $component.firstRow + 1
            $logicalHeight = $height / [Math]::Max(0.01, $DeviceScaleFactor)
            $fillRatio = $component.pixels / [double]($width * $height)
            if ($logicalHeight -ge 18 -and $logicalHeight -le 72 -and
                $fillRatio -ge 0.55 -and $fillRatio -le 0.90 -and
                [Math]::Abs(($width / [double]$height) - 1.0) -le 0.15) {
                [pscustomobject]@{
                    width = $width
                    height = $height
                    aspect = $width / [double]$height
                    fillRatio = $fillRatio
                    area = $width * $height
                }
            }
        }
        return $candidates | Sort-Object area -Descending | Select-Object -First 1
    }
    finally {
        $bitmap.Dispose()
        $stream.Dispose()
    }
}

function Get-SourceFingerprint([string[]] $RelativePaths) {
    $builder = [Text.StringBuilder]::new()
    foreach ($relativePath in ($RelativePaths | Sort-Object)) {
        $path = Join-Path $repoRoot $relativePath
        $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
        [void]$builder.Append($relativePath.Replace('\', '/')).Append('=').Append($hash).Append("`n")
    }
    return [Convert]::ToHexString(
        [Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($builder.ToString()))
    ).ToLowerInvariant()
}

$server = $null
$browserProcess = $null
$socket = $null
$closeWithinFiveSeconds = $false
try {
    [void](Invoke-Process 'dotnet' @('publish', $webProject, '-c', 'Release', '-o', $publishRoot, '--nologo') $repoRoot)
    $wwwroot = Join-Path $publishRoot 'wwwroot'
    if (-not (Test-Path -LiteralPath (Join-Path $wwwroot '_framework/blazor.webassembly.js'))) {
        throw 'Release publish is missing the Blazor WebAssembly boot loader.'
    }
    $python = (Get-Command python -ErrorAction Stop).Source
    $httpPort = Get-FreeTcpPort
    $debugPort = Get-FreeTcpPort
    $server = Start-BackgroundProcess $python @('-m', 'http.server', [string]$httpPort, '--bind', '127.0.0.1', '--directory', $wwwroot) $repoRoot
    Wait-Http "http://127.0.0.1:$httpPort/" ([TimeSpan]::FromSeconds(30))

    $browserPath = Find-Browser $Browser
    $browserArguments = @(
        "--remote-debugging-port=$debugPort",
        "--user-data-dir=$profileRoot",
        '--no-first-run',
        '--no-default-browser-check',
        '--disable-background-timer-throttling',
        '--disable-renderer-backgrounding',
        '--disable-backgrounding-occluded-windows',
        '--disable-features=CalculateNativeWinOcclusion')
    if ($Visible) {
        $browserArguments += '--start-maximized'
    } else {
        $browserArguments += '--window-position=-32000,-32000'
        $browserArguments += '--window-size=1280,720'
    }
    $browserArguments += 'about:blank'
    $browserProcess = Start-BackgroundProcess $browserPath $browserArguments $repoRoot
    Wait-Http "http://127.0.0.1:$debugPort/json/version" ([TimeSpan]::FromSeconds(30))
    $targetUrl = "http://127.0.0.1:$httpPort/?dorotiResizeDiagnostics=1"
    $target = Invoke-RestMethod -Method Put -Uri "http://127.0.0.1:$debugPort/json/new?$([Uri]::EscapeDataString($targetUrl))"
    $socket = [Net.WebSockets.ClientWebSocket]::new()
    [void]($socket.ConnectAsync(
        [Uri]$target.webSocketDebuggerUrl,
        [Threading.CancellationToken]::None).GetAwaiter().GetResult())
    [void](Send-Cdp $socket 'Page.enable')
    [void](Send-Cdp $socket 'Runtime.enable')
    [void](Send-Cdp $socket 'Log.enable')
    [void](Send-Cdp $socket 'Page.navigate' @{ url = $targetUrl })

    $diagnosticExpression = "document.querySelector('.doroti-root')?.getAttribute('data-doroti-resize-diagnostics') ?? null"
    $deadline = [DateTime]::UtcNow + [TimeSpan]::FromSeconds(30)
    do {
        Start-Sleep -Milliseconds 100
        $diagnosticsJson = Invoke-JavaScript $socket $diagnosticExpression
    } while (-not $diagnosticsJson -and [DateTime]::UtcNow -lt $deadline)
    if (-not $diagnosticsJson) {
        $failurePage = [ordered]@{
            url = Invoke-JavaScript $socket 'location.href'
            title = Invoke-JavaScript $socket 'document.title'
            bodyText = Invoke-JavaScript $socket 'document.body?.innerText ?? null'
            readyState = Invoke-JavaScript $socket 'document.readyState'
            bootstrapDataset = Invoke-JavaScript $socket 'JSON.stringify(document.documentElement.dataset)'
            events = $script:cdpEvents
        }
        $failureScreenshot = Capture-Screenshot $socket
        [IO.File]::WriteAllBytes($finalScreenshotPath, $failureScreenshot)
        [IO.File]::WriteAllText($rawPath, ($failurePage | ConvertTo-Json -Depth 30), [Text.UTF8Encoding]::new($false))
        throw "Doroti Web did not expose resize diagnostics within 30 seconds. Failure evidence: $rawPath"
    }
    Start-Sleep -Seconds 2

    Add-Type -AssemblyName System.Drawing
    $stableScreenshot = Capture-Screenshot $socket
    $stableDeviceScaleFactor = [double](Invoke-JavaScript $socket 'window.devicePixelRatio')
    $stableAppBarLogicalHeight = Get-AppBarLogicalHeight $stableScreenshot $stableDeviceScaleFactor
    if ($null -eq $stableAppBarLogicalHeight) {
        throw 'The fixed-height AppBar visual oracle could not find the stable app bar.'
    }
    $stableCircularControl = Get-CircularControlAspect $stableScreenshot $stableDeviceScaleFactor
    if ($null -eq $stableCircularControl) {
        throw 'The circular-control aspect oracle could not find the stable control.'
    }
    $blankCount = 0
    $blankDurationMilliseconds = 0.0
    $firstBlankSaved = $false
    $sampleWatch = [Diagnostics.Stopwatch]::StartNew()
    $previousSampleMilliseconds = 0.0
    $blankSamples = [Collections.Generic.List[object]]::new()
    $appBarSamples = [Collections.Generic.List[object]]::new()
    $appBarGeometryFailures = [Collections.Generic.List[object]]::new()
    $circularControlSamples = [Collections.Generic.List[object]]::new()
    $circularControlGeometryFailures = [Collections.Generic.List[object]]::new()
    $circularControlObservedSamples = 0
    $circularControlNotObservedSamples = 0
    $matrixCases = @()
    if ($ExerciseCompatibilityMatrix) {
        foreach ($dpr in @(1.0, 1.25, 1.5, 2.0)) {
            foreach ($zoom in @(80, 100, 125, 150)) {
                $matrixCases += [pscustomobject]@{
                    baseDpr = $dpr
                    zoomPercent = $zoom
                    effectiveDeviceScaleFactor = $dpr * $zoom / 100.0
                }
            }
        }
    }
    $requestedMatrixCases = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $nativeWindow = Send-Cdp $socket 'Browser.getWindowForTarget'
    $availableScreen = ([string](Invoke-JavaScript $socket @"
JSON.stringify({
  left: Number(screen.availLeft || 0),
  top: Number(screen.availTop || 0),
  width: Number(screen.availWidth),
  height: Number(screen.availHeight)
})
"@)) | ConvertFrom-Json
    $windowMargin = 20
    $nativeWindowLeft = if ($Visible) { [int]$availableScreen.left + $windowMargin } else { -32000 }
    $nativeWindowTop = if ($Visible) { [int]$availableScreen.top + $windowMargin } else { -32000 }
    $maximumNativeWindowWidth = [Math]::Max(1,
        [Math]::Min(1200, [int]$availableScreen.width - ($windowMargin * 2)))
    $maximumNativeWindowHeight = [Math]::Max(1,
        [Math]::Min(800, [int]$availableScreen.height - ($windowMargin * 2)))
    $minimumNativeWindowWidth = [Math]::Max([Math]::Min(320, $maximumNativeWindowWidth),
        $maximumNativeWindowWidth - [Math]::Min(360, [Math]::Floor($maximumNativeWindowWidth * 0.3)))
    $minimumNativeWindowHeight = [Math]::Max([Math]::Min(320, $maximumNativeWindowHeight),
        $maximumNativeWindowHeight - [Math]::Min(180, [Math]::Floor($maximumNativeWindowHeight * 0.25)))
    for ($sample = 0; $sample -lt $SampleCount; $sample++) {
        $cycle = ($sample % 120) / 120.0
        $wave = if ($cycle -le 0.5) { $cycle * 2 } else { (1 - $cycle) * 2 }
        $matrixCase = if ($ExerciseCompatibilityMatrix) {
            $matrixCases[[Math]::Min($matrixCases.Count - 1,
                [Math]::Floor($sample * $matrixCases.Count / $SampleCount))]
        } else { $null }
        $baseWidth = if ($matrixCase) {
            800 + [Math]::Round(480 * $wave)
        } else {
            $maximumNativeWindowWidth - [Math]::Round(
                ($maximumNativeWindowWidth - $minimumNativeWindowWidth) * $wave)
        }
        $baseHeight = if ($matrixCase) {
            500 + [Math]::Round(220 * $wave)
        } else {
            $maximumNativeWindowHeight - [Math]::Round(
                ($maximumNativeWindowHeight - $minimumNativeWindowHeight) * $wave)
        }
        $zoomScale = if ($matrixCase) { $matrixCase.zoomPercent / 100.0 } else { 1.0 }
        $width = [Math]::Max(1, [Math]::Round($baseWidth / $zoomScale))
        $height = [Math]::Max(1, [Math]::Round($baseHeight / $zoomScale))
        $deviceScaleFactor = if ($matrixCase) { $matrixCase.effectiveDeviceScaleFactor } else { $stableDeviceScaleFactor }
        if ($matrixCase) {
            [void]$requestedMatrixCases.Add("$($matrixCase.baseDpr)x@$($matrixCase.zoomPercent)%")
        }
        if ($ExerciseCompatibilityMatrix -and $sample -eq [Math]::Floor($SampleCount * 0.7)) {
            [void](Send-Cdp $socket 'Emulation.setCPUThrottlingRate' @{ rate = 4 })
        }
        if ($ExerciseCompatibilityMatrix -and $sample -eq [Math]::Floor($SampleCount * 0.85)) {
            [void](Send-Cdp $socket 'Emulation.setCPUThrottlingRate' @{ rate = 1 })
        }
        if ($matrixCase) {
            [void](Send-Cdp $socket 'Emulation.setDeviceMetricsOverride' @{
                width = $width; height = $height; deviceScaleFactor = $deviceScaleFactor; mobile = $false
                screenWidth = $width; screenHeight = $height
            })
        } else {
            [void](Send-Cdp $socket 'Browser.setWindowBounds' @{
                windowId = $nativeWindow.windowId
                bounds = @{
                    left = $nativeWindowLeft; top = $nativeWindowTop
                    width = $width; height = $height; windowState = 'normal'
                }
            })
        }
        Start-Sleep -Milliseconds $SampleIntervalMilliseconds
        $observedScaleFactor = [double](Invoke-JavaScript $socket 'window.devicePixelRatio')
        $screenshot = Capture-Screenshot $socket
        $appBarLogicalHeight = Get-AppBarLogicalHeight $screenshot $observedScaleFactor
        $appBarSample = [ordered]@{
            sample = $sample
            requestedDeviceScaleFactor = $deviceScaleFactor
            observedDeviceScaleFactor = $observedScaleFactor
            logicalHeight = if ($null -eq $appBarLogicalHeight) { $null } else { [Math]::Round($appBarLogicalHeight, 3) }
            delta = if ($null -eq $appBarLogicalHeight) { $null } else {
                [Math]::Round([Math]::Abs($appBarLogicalHeight - $stableAppBarLogicalHeight), 3)
            }
        }
        $appBarSamples.Add($appBarSample)
        if ($null -eq $appBarLogicalHeight -or
            [Math]::Abs($appBarLogicalHeight - $stableAppBarLogicalHeight) -gt 1.0) {
            $appBarGeometryFailures.Add($appBarSample)
        }
        $circularControl = Get-CircularControlAspect $screenshot $observedScaleFactor
        $aspectError = if ($null -eq $circularControl) { $null } else {
            [Math]::Abs(($circularControl.aspect / $stableCircularControl.aspect) - 1.0)
        }
        $tolerancePhysicalPixels = [Math]::Max(
            1, [Math]::Ceiling([Math]::Max(0.01, $observedScaleFactor - 0.001)))
        $pixelDelta = if ($null -eq $circularControl) { $null } else {
            [Math]::Abs($circularControl.width - $circularControl.height)
        }
        $circularControlSample = [ordered]@{
            sample = $sample
            aspect = if ($null -eq $circularControl) { $null } else { [Math]::Round($circularControl.aspect, 5) }
            stableAspect = [Math]::Round($stableCircularControl.aspect, 5)
            aspectErrorPercent = if ($null -eq $aspectError) { $null } else { [Math]::Round($aspectError * 100, 3) }
            widthPhysicalPixels = if ($null -eq $circularControl) { $null } else { $circularControl.width }
            heightPhysicalPixels = if ($null -eq $circularControl) { $null } else { $circularControl.height }
            pixelDelta = $pixelDelta
            tolerancePhysicalPixels = $tolerancePhysicalPixels
        }
        $circularControlSamples.Add($circularControlSample)
        if ($null -eq $circularControl) {
            $circularControlNotObservedSamples++
        } else {
            $circularControlObservedSamples++
        }
        if ($null -ne $circularControl -and $pixelDelta -gt $tolerancePhysicalPixels) {
            $circularControlGeometryFailures.Add($circularControlSample)
        }
        $nowMilliseconds = $sampleWatch.Elapsed.TotalMilliseconds
        if (Test-BlankScreenshot $screenshot) {
            $blankCount++
            $blankDurationMilliseconds += [Math]::Max(0, $nowMilliseconds - $previousSampleMilliseconds)
            $blankSamples.Add([ordered]@{
                sample = $sample
                elapsedMilliseconds = [Math]::Round($nowMilliseconds, 3)
                width = $width
                height = $height
                deviceScaleFactor = $deviceScaleFactor
            })
            if (-not $firstBlankSaved) {
                [IO.File]::WriteAllBytes($firstBlankScreenshotPath, $screenshot)
                $firstBlankSaved = $true
            }
        }
        $previousSampleMilliseconds = $nowMilliseconds
    }
    [void](Send-Cdp $socket 'Emulation.setCPUThrottlingRate' @{ rate = 1 })

    $compatibilityResults = $null
    if ($ExerciseCompatibilityMatrix) {
        [void](Send-Cdp $socket 'Emulation.clearDeviceMetricsOverride')
        Start-Sleep -Milliseconds 250

        $window = Send-Cdp $socket 'Browser.getWindowForTarget'
        for ($sample = 0; $sample -lt 120; $sample++) {
            $cycle = ($sample % 60) / 59.0
            $wave = if ($cycle -le 0.5) { $cycle * 2 } else { (1 - $cycle) * 2 }
            [void](Send-Cdp $socket 'Browser.setWindowBounds' @{
                windowId = $window.windowId
                bounds = @{
                    left = -32000; top = -32000
                    width = 900 + [Math]::Round(360 * $wave)
                    height = 620 + [Math]::Round(180 * $wave)
                    windowState = 'normal'
                }
            })
            Start-Sleep -Milliseconds 8
        }
        [void](Send-Cdp $socket 'Browser.setWindowBounds' @{
            windowId = $window.windowId
            bounds = @{ windowState = 'maximized' }
        })
        Start-Sleep -Milliseconds 250
        [void](Send-Cdp $socket 'Browser.setWindowBounds' @{
            windowId = $window.windowId
            bounds = @{ left = -32000; top = -32000; width = 1280; height = 720; windowState = 'normal' }
        })
        Start-Sleep -Milliseconds 250

        [void](Send-Cdp $socket 'Page.setWebLifecycleState' @{ state = 'frozen' })
        Start-Sleep -Milliseconds 250
        [void](Send-Cdp $socket 'Page.setWebLifecycleState' @{ state = 'active' })
        [void](Send-Cdp $socket 'Page.bringToFront')
        Start-Sleep -Milliseconds 250

        $presenterBeforeLoss = ([string](Invoke-JavaScript $socket "globalThis.__dorotiResizeDiagnostics.presenter('doroti-surface')")) | ConvertFrom-Json
        $lossSupported = [bool](Invoke-JavaScript $socket "globalThis.__dorotiResizeDiagnostics.loseContext('doroti-surface')")
        if (-not $lossSupported) { throw 'WEBGL_lose_context is unavailable.' }
        Wait-JavaScriptTrue $socket `
            "JSON.parse(globalThis.__dorotiResizeDiagnostics.presenter('doroti-surface')).contextLost" `
            ([TimeSpan]::FromSeconds(5)) 'WebGL context loss was not observed.'
        $restoreSupported = [bool](Invoke-JavaScript $socket "globalThis.__dorotiResizeDiagnostics.restoreContext('doroti-surface')")
        if (-not $restoreSupported) { throw 'WEBGL_lose_context restore is unavailable.' }
        $minimumContextGeneration = [long]$presenterBeforeLoss.contextGeneration + 1
        $restoreRebuiltFront = $false
        Start-Sleep -Milliseconds 250
        [void](Capture-Screenshot $socket)
        Start-Sleep -Seconds 2
        $restoreDeadline = [DateTime]::UtcNow + [TimeSpan]::FromSeconds(10)
        do {
            $restoreRebuiltFront = [bool](Invoke-JavaScript $socket `
                "(() => { const p = JSON.parse(globalThis.__dorotiResizeDiagnostics.presenter('doroti-surface')); return !p.contextLost && p.contextGeneration >= $minimumContextGeneration && p.frontGeneration !== null; })()")
            if (-not $restoreRebuiltFront) { Start-Sleep -Milliseconds 250 }
        } while (-not $restoreRebuiltFront -and [DateTime]::UtcNow -lt $restoreDeadline)
        $presenterAfterLoss = ([string](Invoke-JavaScript $socket "globalThis.__dorotiResizeDiagnostics.presenter('doroti-surface')")) | ConvertFrom-Json
        $postRestoreScreenshot = Capture-Screenshot $socket
        $postRestoreBlank = Test-BlankScreenshot $postRestoreScreenshot
        if ($postRestoreBlank) {
            $blankCount++
            if (-not $firstBlankSaved) {
                [IO.File]::WriteAllBytes($firstBlankScreenshotPath, $postRestoreScreenshot)
                $firstBlankSaved = $true
            }
        }
        $compatibilityResults = [ordered]@{
            requestedDprZoomCases = @($requestedMatrixCases | Sort-Object)
            requestedDprZoomCaseCount = $requestedMatrixCases.Count
            zoomMethod = 'CDP device metrics: logical viewport divided by zoom and effective DPR multiplied by zoom'
            nativeWindowBoundsSamples = 120
            maximizeRestore = 'exercised'
            cpuSlowdownRate = 4
            backgroundForeground = 'Page frozen then active'
            restorePresentationOpportunity = 'validation-only CDP screenshot after restore'
            contextLossRestore = [ordered]@{
                supported = $true
                contextGenerationBefore = $presenterBeforeLoss.contextGeneration
                contextGenerationAfter = $presenterAfterLoss.contextGeneration
                frontGenerationAfter = $presenterAfterLoss.frontGeneration
                rebuiltExactFront = $restoreRebuiltFront
                postRestoreBlank = $postRestoreBlank
            }
        }
    }
    if ($PostResizeObservationSeconds -gt 0) {
        Start-Sleep -Seconds $PostResizeObservationSeconds
    }
    $diagnosticsJson = [string](Invoke-JavaScript $socket $diagnosticExpression)
    $diagnostics = $diagnosticsJson | ConvertFrom-Json -Depth 30
    $presenterJson = [string](Invoke-JavaScript $socket "globalThis.__dorotiResizeDiagnostics.presenter('doroti-surface')")
    $presenter = $presenterJson | ConvertFrom-Json
    $finalScreenshot = Capture-Screenshot $socket
    [IO.File]::WriteAllBytes($finalScreenshotPath, $finalScreenshot)

    $trace = @($diagnostics.trace)
    $targets = @($trace | Where-Object phase -eq 'target-observed')
    $requests = @($trace | Where-Object phase -eq 'present-requested')
    $terminals = @($trace | Where-Object { $_.terminal })
    $resets = @($trace | Where-Object phase -eq 'backing-reset-start')
    $resetCommits = @($trace | Where-Object phase -eq 'backing-reset-end')
    $stableRefreshes = @($trace | Where-Object phase -eq 'stable-front-refresh')
    $commits = @($trace | Where-Object phase -eq 'front-commit')
    $targetGenerations = @($targets | ForEach-Object { [long]$_.epoch.generation })
    $generationRegressions = 0
    for ($index = 1; $index -lt $targetGenerations.Count; $index++) {
        if ($targetGenerations[$index] -lt $targetGenerations[$index - 1]) { $generationRegressions++ }
    }
    $terminalGroups = @($terminals | Group-Object rafId)
    $requestIds = @($requests.rafId | Sort-Object -Unique)
    $terminalIds = @($terminals.rafId | Sort-Object -Unique)
    $unterminated = @($requestIds | Where-Object { $terminalIds -notcontains $_ })
    $duplicateTerminals = @($terminalGroups | Where-Object Count -ne 1)
    $staleFrontCommits = @($commits | Where-Object {
        $commit = $_
        $latest = $targets | Where-Object { [long]$_.sequence -lt [long]$commit.sequence } | Select-Object -Last 1
        $latest -and [long]$latest.epoch.generation -ne [long]$commit.epoch.generation
    })
    $resetCoverageFailures = @($resets | Where-Object {
        $reset = $_
        -not ($trace | Where-Object {
            [long]$_.sequence -gt [long]$reset.sequence -and
            [long]$_.epoch.generation -eq [long]$reset.epoch.generation -and
            $_.phase -in @('backing-reset-end', 'startup-background-commit')
        } | Select-Object -First 1)
    })
    $blitErrors = @($trace | Where-Object {
        if ($_.phase -notin @('front-commit', 'stable-front-refresh', 'startup-background-commit') -or -not $_.detail) {
            return $false
        }
        $detail = $_.detail | ConvertFrom-Json
        return $detail.error -ne 0 -or @($detail.priorErrors).Count -ne 0
    })
    $provisionalGeometryFailures = @($trace | Where-Object {
        if ($_.phase -ne 'stable-front-refresh' -or -not $_.detail) {
            return $false
        }
        $detail = $_.detail | ConvertFrom-Json
        return $detail.policy -ne 'target-sized-default-top-left-crop' -or
            [Math]::Abs([double]$detail.scaleX - 1.0) -gt 0.000001 -or
            [Math]::Abs([double]$detail.scaleY - 1.0) -gt 0.000001 -or
            [long]$detail.sourceRect[2] -ne [long]$detail.destinationRect[2] -or
            [long]$detail.sourceRect[3] -ne [long]$detail.destinationRect[3]
    })
    $latestTarget = $targets | Select-Object -Last 1
    $latestExact = $commits | Where-Object {
        [long]$_.epoch.generation -eq [long]$latestTarget.epoch.generation
    } | Select-Object -Last 1
    $browserExceptions = @($script:cdpEvents | Where-Object {
        $_.method -in @('Runtime.exceptionThrown', 'Log.entryAdded') -and
        ($_.method -ne 'Log.entryAdded' -or
            ($_.params.entry.level -eq 'error' -and $_.params.entry.url -notlike '*/favicon.ico'))
    })
    $browserWarnings = @($script:cdpEvents | Where-Object {
        $_.method -eq 'Log.entryAdded' -and $_.params.entry.level -eq 'warning'
    })
    $sourceFiles = @(
        'Doroti/src/Doroti.Host.Web/Web/doroti.web.ts',
        'Doroti/src/Doroti.Host.Web/wwwroot/doroti.web.css',
        'Doroti/src/Doroti.Host.Web/DorotiWebGlSurface.razor',
        'Doroti/src/Doroti.Host.Web/BrowserSkiaCapabilities.cs',
        'Doroti/src/Doroti.Skia.Rendering/SkiaSceneRenderer.cs',
        'Doroti/eng/validate-web-resize-continuity-live.ps1')
    $sourceFingerprint = Get-SourceFingerprint $sourceFiles
    $summary = [ordered]@{
        schemaVersion = 'doroti.web-resize-continuity-live/v2'
        capturedAt = [DateTimeOffset]::Now.ToString('o')
        status = 'PASS'
        browser = $Browser
        scenario = if ($ExerciseCompatibilityMatrix) { 'compatibility-matrix' } else { 'baseline' }
        sourceFingerprint = $sourceFingerprint
        inputMotion = if ($ExerciseCompatibilityMatrix) {
            'CDP viewport/DPR compatibility triangle wave'
        } else {
            'native browser-window bounds triangle wave'
        }
        inputSamples = $SampleCount
        sampleIntervalMilliseconds = $SampleIntervalMilliseconds
        postResizeObservationSeconds = $PostResizeObservationSeconds
        visibleWindow = [bool]$Visible
        coverage = 'smoke-regression'
        targetCount = $targets.Count
        generationMinimum = if ($targetGenerations.Count) { $targetGenerations[0] } else { $null }
        generationMaximum = if ($targetGenerations.Count) { $targetGenerations[-1] } else { $null }
        backingResetCount = $resets.Count
        backingResetCommitCount = $resetCommits.Count
        stableFrontRefreshCount = $stableRefreshes.Count
        stableFrontRefreshLatencyMicroseconds = @($stableRefreshes.durationMicroseconds)
        exactFrontCommitCount = $commits.Count
        staleFrontCommitCount = $staleFrontCommits.Count
        blankExposureCount = $blankCount
        blankExposureDurationMilliseconds = [Math]::Round($blankDurationMilliseconds, 3)
        blankSamples = $blankSamples.ToArray()
        appBarGeometry = [ordered]@{
            stableLogicalHeight = [Math]::Round($stableAppBarLogicalHeight, 3)
            toleranceLogicalPixels = 1.0
            failedSamples = $appBarGeometryFailures.Count
            samples = $appBarSamples.ToArray()
        }
        circularControlGeometry = [ordered]@{
            stableAspect = [Math]::Round($stableCircularControl.aspect, 5)
            tolerance = 'max(1 physical pixel, ceil(devicePixelRatio))'
            observedSamples = $circularControlObservedSamples
            notObservedSamples = $circularControlNotObservedSamples
            minimumObservedSamples = [Math]::Max(4, [Math]::Ceiling($SampleCount * 0.1))
            failedSamples = $circularControlGeometryFailures.Count
            samples = $circularControlSamples.ToArray()
        }
        queueHighWatermark = ($trace.queueDepth | Measure-Object -Maximum).Maximum
        terminal = [ordered]@{
            submitted = @($terminals | Where-Object terminal -eq 'submitted').Count
            superseded = @($terminals | Where-Object terminal -eq 'superseded').Count
            failed = @($terminals | Where-Object terminal -eq 'failed').Count
            unterminated = $unterminated.Count
            duplicate = $duplicateTerminals.Count
        }
        generationRegressions = $generationRegressions
        resetCoverageFailures = $resetCoverageFailures.Count
        gpuBlitErrors = $blitErrors.Count
        nonUniformProvisionalScale = $provisionalGeometryFailures.Count
        latestTargetExactCommit = [bool]$latestExact
        contextId = $presenter.context
        contextGeneration = $presenter.contextGeneration
        surfaceGeneration = $diagnostics.snapshot.surfaceGeneration
        frontGeneration = $presenter.frontGeneration
        observedDevicePixelRatios = @($targets.epoch.devicePixelRatio | Sort-Object -Unique)
        compatibility = $compatibilityResults
        browserExceptionCount = $browserExceptions.Count
        browserWarningCount = $browserWarnings.Count
        screenshotSampling = 'validation-only CDP screenshots; no product readback'
        rawTrace = [IO.Path]::GetRelativePath($repoRoot, $rawPath).Replace('\', '/')
        finalScreenshot = [IO.Path]::GetRelativePath($repoRoot, $finalScreenshotPath).Replace('\', '/')
        firstBlankScreenshot = if ($firstBlankSaved) {
            [IO.Path]::GetRelativePath($repoRoot, $firstBlankScreenshotPath).Replace('\', '/')
        } else { $null }
    }
    $failures = [Collections.Generic.List[string]]::new()
    if ($targets.Count -lt $SampleCount) { $failures.Add("targetCount=$($targets.Count) < $SampleCount") }
    if ($blankCount -ne 0) { $failures.Add("blankExposureCount=$blankCount") }
    if ($appBarGeometryFailures.Count -ne 0) {
        $failures.Add("appBarGeometryFailures=$($appBarGeometryFailures.Count)")
    }
    if ($circularControlGeometryFailures.Count -ne 0) {
        $failures.Add("circularControlGeometryFailures=$($circularControlGeometryFailures.Count)")
    }
    if ($circularControlObservedSamples -lt $summary.circularControlGeometry.minimumObservedSamples) {
        $failures.Add("circularControlObservedSamples=$circularControlObservedSamples < $($summary.circularControlGeometry.minimumObservedSamples)")
    }
    if ($staleFrontCommits.Count -ne 0) { $failures.Add("staleFrontCommitCount=$($staleFrontCommits.Count)") }
    if ($generationRegressions -ne 0) { $failures.Add("generationRegressions=$generationRegressions") }
    if ($summary.queueHighWatermark -gt 2) { $failures.Add("queueHighWatermark=$($summary.queueHighWatermark)") }
    if ($summary.terminal.failed -ne 0 -or $unterminated.Count -ne 0 -or $duplicateTerminals.Count -ne 0) {
        $failures.Add("terminal failed=$($summary.terminal.failed) unterminated=$($unterminated.Count) duplicate=$($duplicateTerminals.Count)")
    }
    if ($resetCoverageFailures.Count -ne 0) { $failures.Add("resetCoverageFailures=$($resetCoverageFailures.Count)") }
    if ($blitErrors.Count -ne 0) { $failures.Add("gpuBlitErrors=$($blitErrors.Count)") }
    if ($provisionalGeometryFailures.Count -ne 0) {
        $failures.Add("nonUniformProvisionalScale=$($provisionalGeometryFailures.Count)")
    }
    if (-not $latestExact) { $failures.Add('latest target lacks an exact front commit') }
    if ($browserExceptions.Count -ne 0) { $failures.Add("browserExceptionCount=$($browserExceptions.Count)") }
    if ($ExerciseCompatibilityMatrix -and $requestedMatrixCases.Count -ne 16) {
        $failures.Add("requestedDprZoomCaseCount=$($requestedMatrixCases.Count) expected=16")
    }
    if ($ExerciseCompatibilityMatrix -and
        ([long]$compatibilityResults.contextLossRestore.contextGenerationAfter -le
            [long]$compatibilityResults.contextLossRestore.contextGenerationBefore -or
         -not [bool]$compatibilityResults.contextLossRestore.rebuiltExactFront -or
         [bool]$compatibilityResults.contextLossRestore.postRestoreBlank)) {
        $failures.Add('WebGL context loss/restore did not rebuild a visible front')
    }
    if ($failures.Count -ne 0) {
        $summary.status = 'FAIL'
        $summary.failures = $failures.ToArray()
    }
    $raw = [ordered]@{
        schemaVersion = 'doroti.web-resize-continuity-raw/v1'
        capturedAt = [DateTimeOffset]::Now.ToString('o')
        browser = $Browser
        scenario = if ($ExerciseCompatibilityMatrix) { 'compatibility-matrix' } else { 'baseline' }
        sourceFingerprint = $sourceFingerprint
        snapshot = $diagnostics.snapshot
        presenter = $presenter
        trace = $trace
        blankSamples = $blankSamples.ToArray()
        appBarSamples = $appBarSamples.ToArray()
        circularControlSamples = $circularControlSamples.ToArray()
        browserEvents = $browserExceptions
        browserWarnings = $browserWarnings
    }
    [IO.File]::WriteAllText($rawPath, ($raw | ConvertTo-Json -Depth 30), [Text.UTF8Encoding]::new($false))
    [IO.File]::WriteAllText($summaryPath, ($summary | ConvertTo-Json -Depth 20), [Text.UTF8Encoding]::new($false))

    $closeWatch = [Diagnostics.Stopwatch]::StartNew()
    try { [void](Send-Cdp $socket 'Browser.close') } catch { }
    if (-not $browserProcess.WaitForExit(5000)) { $browserProcess.Kill($true) }
    $closeWithinFiveSeconds = $browserProcess.HasExited -and $closeWatch.Elapsed -le [TimeSpan]::FromSeconds(5)
    if (-not $closeWithinFiveSeconds) {
        $summary.status = 'FAIL'
        $summary.closeWithinFiveSeconds = $false
        [IO.File]::WriteAllText($summaryPath, ($summary | ConvertTo-Json -Depth 20), [Text.UTF8Encoding]::new($false))
        throw "Browser did not close normally within five seconds. Summary: $summaryPath"
    }
    if ($summary.status -ne 'PASS') { throw "Web resize continuity failed: $($failures -join '; '). Summary: $summaryPath" }
    Write-Output ($summary | ConvertTo-Json -Depth 20 -Compress)
    Write-Output "Doroti Web resize continuity live: PASS ($summaryPath)"
}
finally {
    if ($socket) { $socket.Dispose() }
    if ($browserProcess -and -not $browserProcess.HasExited) { $browserProcess.Kill($true) }
    if ($server -and -not $server.HasExited) { $server.Kill($true) }
    Remove-ScopedDirectory $runRoot
}
