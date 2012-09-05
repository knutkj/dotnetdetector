function Get-DotNetVersion {
    
    #
    # .SYNOPSIS
    # Get installed Microsoft .NET Framework versions.
    #
    # .EXAMPLE
    # Get-DotNetVersion | Format-Table -AutoSize
    # Version ServicePacks   Profiles
    # ------- ------------   --------
    # 4.0     {}           ClientFull
    # 3.5     {1.0}        ClientFull
    # 3.0     {1.0, 2.0}   ClientFull
    # 2.0     {1.0, 2.0}   ClientFull
    #
    # .EXAMPLE
    # Get-DotNetVersion | Select-Object -ExpandProperty Version
    # Major  Minor  Build  Revision
    # -----  -----  -----  --------
    # 4      0      -1     -1      
    # 3      5      -1     -1      
    # 3      0      -1     -1      
    # 2      0      -1     -1    
    #
    
    [CmdletBinding()]
    param(
    )
    
    process {
        [DotNetDetector.Detector]::Versions
    }
}