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
# Module x86 preparation.
#
$wixFileContent = $tmpPath |
    Join-Path -ChildPath (Split-Path -Leaf $wixFile) |
    Get-Item |
    Get-Content
$wixXmlDoc = [xml] $wixFileContent
$wixXmlDoc.Wix.Product.Name = 'DotNetDetector PowerShell Module (x86)'
$wixXmlDoc.Wix.Product.UpgradeCode = '58E7A7FF-E5CF-4BDC-A7B9-12E1657357FB'
$wixXmlDoc.Wix.Product.Package.Platform = 'x86'
$wixXmlDoc.Wix.Product.Directory.Directory.Id = 'SystemFolder'
$wixXmlDoc.Wix.Product.DirectoryRef.Component | % { $_.Win64 = 'no' }
$wixXmlDoc.Save(($tmpPath | Join-Path -ChildPath Productx86.wxs))

#
# Module build.
#
Push-Location -Path $tmpPath
'Product.wxs', 'Productx86.wxs' | ForEach-Object -Process { & $candle $_ }
'Product.wixobj', 'Productx86.wixobj' | ForEach-Object -Process { & $light $_ }
$moduleVersion = ($assembly | Get-ItemProperty).VersionInfo.ProductVersion
$module32FileName = "DotNetDetectorPowerShellModule-$moduleVersion-x86.msi"
$module64FileName = "DotNetDetectorPowerShellModule-$moduleVersion-x64.msi"
'Product.msi' |
    Copy-Item -Destination (
        $modulePath | Join-Path -ChildPath $module64FileName
    )
'Productx86.msi' |
    Copy-Item -Destination (
        $modulePath | Join-Path -ChildPath $module32FileName
    )
Pop-Location