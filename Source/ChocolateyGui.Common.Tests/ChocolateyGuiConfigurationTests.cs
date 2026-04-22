// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyGuiConfigurationTests.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Common.Models;
using Xunit;

namespace ChocolateyGui.Common.Tests
{
    public class ChocolateyGuiConfigurationTests
    {
        [Fact]
        public void Constructor_SetsRegularOutputToTrue()
        {
            var config = new ChocolateyGuiConfiguration();

            Assert.True(config.RegularOutput);
        }

        [Fact]
        public void Constructor_InitializesInformationProperty()
        {
            var config = new ChocolateyGuiConfiguration();

            Assert.NotNull(config.Information);
        }

        [Fact]
        public void Constructor_InitializesFeatureCommandProperty()
        {
            var config = new ChocolateyGuiConfiguration();

            Assert.NotNull(config.FeatureCommand);
        }

        [Fact]
        public void Constructor_InitializesConfigCommandProperty()
        {
            var config = new ChocolateyGuiConfiguration();

            Assert.NotNull(config.ConfigCommand);
        }

        [Fact]
        public void Constructor_InitializesPurgeCommandProperty()
        {
            var config = new ChocolateyGuiConfiguration();

            Assert.NotNull(config.PurgeCommand);
        }

        [Fact]
        public void Constructor_DefaultsHelpRequestedToFalse()
        {
            var config = new ChocolateyGuiConfiguration();

            Assert.False(config.HelpRequested);
        }

        [Fact]
        public void Constructor_DefaultsUnsuccessfulParsingToFalse()
        {
            var config = new ChocolateyGuiConfiguration();

            Assert.False(config.UnsuccessfulParsing);
        }

        [Fact]
        public void Constructor_DefaultsGlobalToFalse()
        {
            var config = new ChocolateyGuiConfiguration();

            Assert.False(config.Global);
        }

        [Fact]
        public void Constructor_DefaultsCommandNameToNull()
        {
            var config = new ChocolateyGuiConfiguration();

            Assert.Null(config.CommandName);
        }

        [Fact]
        public void Constructor_DefaultsInputToNull()
        {
            var config = new ChocolateyGuiConfiguration();

            Assert.Null(config.Input);
        }

        [Fact]
        public void Properties_CanBeSetAfterConstruction()
        {
            var config = new ChocolateyGuiConfiguration();

            config.CommandName = "feature";
            config.HelpRequested = true;
            config.UnsuccessfulParsing = true;
            config.RegularOutput = false;
            config.Input = "list";
            config.Global = true;

            Assert.Equal("feature", config.CommandName);
            Assert.True(config.HelpRequested);
            Assert.True(config.UnsuccessfulParsing);
            Assert.False(config.RegularOutput);
            Assert.Equal("list", config.Input);
            Assert.True(config.Global);
        }
    }
}
