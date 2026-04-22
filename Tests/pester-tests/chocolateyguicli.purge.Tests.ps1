Import-Module ../helpers/gui-helpers.psm1

Describe "chocolateyguicli purge" -Tag ChocolateyGuiCli {
    Context "purge icons" {
        BeforeAll {
            $Output = Invoke-GuiCli purge icons
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }
    }

    Context "purge outdated" {
        BeforeAll {
            $Output = Invoke-GuiCli purge outdated
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }
    }

    Context "purge help (-?)" {
        BeforeAll {
            $Output = Invoke-GuiCli purge -?
        }

        It 'Should exit Success (0)' {
            $Output.ExitCode | Should -Be 0 -Because $Output.String
        }

        It 'Should output usage information' {
            $Output.String | Should -Match 'chocolateyguicli purge'
        }

        It 'Should mention icons and outdated subcommands' {
            $Output.String | Should -Match 'icons'
            $Output.String | Should -Match 'outdated'
        }
    }

    Context "purge with unknown subcommand" {
        BeforeAll {
            $Output = Invoke-GuiCli purge unknownsubcommand
        }

        It 'Should exit with non-zero code' {
            $Output.ExitCode | Should -Not -Be 0 -Because $Output.String
        }
    }

    Context "purge with no subcommand" {
        BeforeAll {
            $Output = Invoke-GuiCli purge
        }

        It 'Should exit with non-zero code' {
            $Output.ExitCode | Should -Not -Be 0 -Because $Output.String
        }
    }
}
