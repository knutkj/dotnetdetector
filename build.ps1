#
# Tools init.
#
if ($env:WIX -eq $null) {
    throw 'WiX must be installed.'
}
$msbuild = [object].Assembly.Location |
    Split-Path -Parent |
    Join-Path -ChildPath MSBuild.exe
$nuget = $PWD | Join-Path -ChildPath Lib\NuGet\NuGet.exe
$candle = $env:WIX | Join-Path -ChildPath bin\candle.exe
$light = $env:WIX | Join-Path -ChildPath bin\light.exe
$msbuild, $nuget, $candle |
    Where-Object -FilterScript { -not (Test-Path -Path $_) } |
    ForEach-Object -Process { throw "Could not find $_." }

#
# Compile.
#
& $msbuild

#
# Build directories init.
#
$buildPath = $PWD | Join-Path -ChildPath build
$modulePath = $buildPath | Join-Path -ChildPath psmodule
$libPath = $buildPath | Join-Path -ChildPath lib
$modulePath, $libPath | 
    Where-Object -FilterScript { -not (Test-Path -Path $_) } |
    ForEach-Object -Process { New-Item -ItemType directory -Path $_ } |
    Out-Null

#
# NuGet package build.
#
& $nuget pack .\DotNetDetector\DotNetDetector.csproj -OutputDirectory $libPath

#
# Module file copy.
#
$tmpPath = $PWD | Join-Path -ChildPath tmp
$tmpPath | 
    Where-Object -FilterScript { $_ | Test-Path } |
    Remove-Item -Recurse -Force
New-Item -ItemType directory -Path $tmpPath | Out-Null
$projectPath = $PWD | Join-Path -ChildPath DotNetDetector
$wixFile = $projectPath | Join-Path -ChildPath Product.wxs
$manifest = $projectPath | Join-Path -ChildPath DotNetDetector.psd1
$module = $projectPath | Join-Path -ChildPath DotNetDetector.psm1
$assembly = $projectPath | Join-Path -ChildPath bin\debug\DotNetDetector.dll
$wixFile, $manifest, $module, $assembly | Copy-Item -Destination $tmpPath

#
# Module build.
#
Push-Location -Path $tmpPath
& $candle Product.wxs
& $light Product.wixobj
$moduleVersion = ($assembly | Get-ItemProperty).VersionInfo.ProductVersion
$moduleFileName = "DotNetDetector-$moduleVersion-x64.msi"
'Product.msi' |
    Copy-Item -Destination (
        $modulePath | Join-Path -ChildPath $moduleFileName
    )
Pop-Location