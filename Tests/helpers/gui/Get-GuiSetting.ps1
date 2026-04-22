function Get-GuiSetting {
    <#
        .Synopsis
            Helper function to call chocolateyguicli and return the settings information as a PSCustomObject.
    #>
    [CmdletBinding()]
    param(
        [string[]]$Setting = '*'
    )

    $settingList = (Invoke-GuiCli config list -r).Lines |
        ConvertFrom-Csv -Delimiter '|' -Header Key, Value, Description |
        Select-Object Key, Value, Description
    foreach ($s in $Setting) {
        $settingList | Where-Object Key -Like $s
    }
}
