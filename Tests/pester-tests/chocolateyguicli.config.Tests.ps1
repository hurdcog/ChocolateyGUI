Import-Module ../helpers/gui-helpers.psm1

Describe "chocolateyguicli config" -Tag ChocolateyGuiCli {
    BeforeDiscovery {
        $Settings = Get-GuiSetting
    }

    Context "config (no subcommand) defaults to list" {
        BeforeAll {
            $Output = Invoke-GuiCli config
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should output setting entries' {
            $Output.Lines | Should -Not -BeNullOrEmpty
        }
    }

    Context "config list" {
        BeforeAll {
            $Output = Invoke-GuiCli config list
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should list all known settings' {
            $Output.String | Should -Match 'outdatedPackagesCacheDurationInMinutes'
        }
    }

    Context "config list with limit-output flag (-r)" {
        BeforeAll {
            $Output = Invoke-GuiCli config list -r
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should produce pipe-delimited output' {
            $Output.Lines | Should -Not -BeNullOrEmpty
            $Output.Lines[0] | Should -Match '\|'
        }

        It 'Should be parseable as CSV with pipe delimiter' {
            $parsed = $Output.Lines | ConvertFrom-Csv -Delimiter '|' -Header Key, Value, Description
            $parsed | Should -Not -BeNullOrEmpty
        }
    }

    Context "Contains reference of the setting (<_.Key>)" -ForEach $Settings {
        BeforeAll {
            $Output = Invoke-GuiCli config list
        }

        It 'Should contain the setting key in list output' {
            $Output.String | Should -Match $Key
        }
    }

    Context "config get for setting (<_.Key>)" -ForEach $Settings {
        BeforeAll {
            $Output = Invoke-GuiCli config get --name $Key
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }
    }

    Context "config set and unset for 'outdatedPackagesCacheDurationInMinutes'" {
        BeforeAll {
            $OriginalOutput = Invoke-GuiCli config get --name outdatedPackagesCacheDurationInMinutes
            $OriginalValue = $OriginalOutput.Lines | Where-Object { $_ -ne '' } | Select-Object -First 1
            $SetOutput = Invoke-GuiCli config set --name outdatedPackagesCacheDurationInMinutes --value 120
            $GetAfterSetOutput = Invoke-GuiCli config get --name outdatedPackagesCacheDurationInMinutes
        }

        AfterAll {
            # Restore the original value (or unset if it was empty)
            if ([string]::IsNullOrWhiteSpace($OriginalValue)) {
                Invoke-GuiCli config unset --name outdatedPackagesCacheDurationInMinutes
            } else {
                Invoke-GuiCli config set --name outdatedPackagesCacheDurationInMinutes --value $OriginalValue
            }
        }

        It 'config set Should exit Success (0)' {
            $SetOutput.ExitCode | Should -Be 0 -Because $SetOutput.String
        }

        It 'config get after set Should return the new value' {
            $GetAfterSetOutput.String | Should -Match '120'
        }
    }

    Context "config unset for 'defaultSourceName'" {
        BeforeAll {
            Invoke-GuiCli config set --name defaultSourceName --value 'TestSource' | Out-Null
            $UnsetOutput = Invoke-GuiCli config unset --name defaultSourceName
            $GetAfterUnsetOutput = Invoke-GuiCli config get --name defaultSourceName
        }

        It 'config unset Should exit Success (0)' {
            $UnsetOutput.ExitCode | Should -Be 0 -Because $UnsetOutput.String
        }

        It 'config get after unset Should return empty value' {
            $GetAfterUnsetOutput.String.Trim() | Should -BeNullOrEmpty
        }
    }

    Context "config help (-?)" {
        BeforeAll {
            $Output = Invoke-GuiCli config -?
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should output usage information' {
            $Output.String | Should -Match 'chocolateyguicli config'
        }
    }

    Context "config with unknown subcommand defaults to list" {
        BeforeAll {
            $Output = Invoke-GuiCli config unknownsubcommand
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should emit a warning about the unknown command' {
            $Output.String | Should -Match 'list'
        }
    }
}
