Import-Module ../helpers/gui-helpers.psm1

Describe "chocolateyguicli" -Tag ChocolateyGuiCli {
    BeforeDiscovery {
        # Perhaps a better way is to pull these from the LiteDB similar to how we do features from the xml in CLI, but this will do for an initial setup.
        $Features = Get-GuiFeature
    }

    Context "Basic CLI functionality" {
        BeforeAll {
            $Output = Invoke-GuiCli
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should output appropriate message' {
            $Output.Lines | Should -Contain "Please run chocolateyguicli with 'chocolateyguicli -?' or 'chocolateyguicli <command> -?' for specific help on each command"
        }
    }

    Context "Help flag (-?)" {
        BeforeAll {
            $Output = Invoke-GuiCli -?
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should output command listing' {
            $Output.String | Should -Match 'chocolateyguicli'
        }

        It 'Should list available commands' {
            $Output.String | Should -Match 'feature'
            $Output.String | Should -Match 'config'
            $Output.String | Should -Match 'purge'
        }
    }

    Context "limit-output flag (-r)" {
        BeforeAll {
            $Output = Invoke-GuiCli -r
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }
    }

    Context "Lists available features" {
        BeforeAll {
            $Output = Invoke-GuiCli feature list
        }

        It "Contains reference of the option (<_.Name>)" -ForEach $Features {
            $Output.String | Should -Match $Name
        }
    }

    Context "feature list with limit-output flag (-r)" {
        BeforeAll {
            $Output = Invoke-GuiCli feature list -r
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should produce pipe-delimited output' {
            $Output.Lines | Should -Not -BeNullOrEmpty
            $Output.Lines[0] | Should -Match '\|'
        }
    }

    Context "feature (no subcommand) defaults to list" {
        BeforeAll {
            $Output = Invoke-GuiCli feature
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should list features' {
            $Output.String | Should -Not -BeNullOrEmpty
        }
    }

    Context "feature help (-?)" {
        BeforeAll {
            $Output = Invoke-GuiCli feature -?
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should output usage information' {
            $Output.String | Should -Match 'chocolateyguicli feature'
        }

        It 'Should mention enable and disable subcommands' {
            $Output.String | Should -Match 'enable'
            $Output.String | Should -Match 'disable'
        }
    }

    Context "feature with unknown subcommand defaults to list with warning" {
        BeforeAll {
            $Output = Invoke-GuiCli feature unknownsubcommand
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should emit a warning about the unknown command' {
            $Output.String | Should -Match 'list'
        }
    }

    Context "Toggles the feature (<_.Name>) successfully" -ForEach $Features {
        BeforeAll {
            if ($Enabled) {
                $DisableOutput = Invoke-GuiCli feature disable --name $_.Name
                $EnableOutput = Invoke-GuiCli feature enable --name $_.Name
            } else {
                $EnableOutput = Invoke-GuiCli feature enable --name $_.Name
                $DisableOutput = Invoke-GuiCli feature disable --name $_.Name
            }
        }

        It "Should exit success (0)" {
            $EnableOutput.ExitCode | Should -Be 0 -Because $EnableOutput.String
            $DisableOutput.ExitCode | Should -Be 0 -Because $DisableOutput.String
        }

        It "enable Should output a confirmation message for (<_.Name>)" {
            $EnableOutput.String | Should -Match $Name
        }

        It "disable Should output a confirmation message for (<_.Name>)" {
            $DisableOutput.String | Should -Match $Name
        }
    }

    Context "feature enable with invalid name" {
        BeforeAll {
            $Output = Invoke-GuiCli feature enable --name NonExistentFeatureThatDoesNotExist
        }

        It 'Should exit with non-zero code' {
            $Output.ExitCode | Should -Not -Be 0 -Because $Output.String
        }
    }

    Context "feature disable with invalid name" {
        BeforeAll {
            $Output = Invoke-GuiCli feature disable --name NonExistentFeatureThatDoesNotExist
        }

        It 'Should exit with non-zero code' {
            $Output.ExitCode | Should -Not -Be 0 -Because $Output.String
        }
    }
}
