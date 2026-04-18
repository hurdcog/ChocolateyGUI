// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NuGetVersionTypeConverterTests.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Common.Models;
using NuGet.Versioning;
using Xunit;

namespace ChocolateyGui.Common.Tests
{
    public class NuGetVersionTypeConverterTests
    {
        private readonly NuGetVersionTypeConverter _converter = new NuGetVersionTypeConverter();

        [Fact]
        public void CanConvertFrom_StringType_ReturnsTrue()
        {
            var result = _converter.CanConvertFrom(null, typeof(string));

            Assert.True(result);
        }

        [Fact]
        public void CanConvertFrom_IntType_ReturnsFalse()
        {
            var result = _converter.CanConvertFrom(null, typeof(int));

            Assert.False(result);
        }

        [Fact]
        public void CanConvertFrom_ObjectType_ReturnsFalse()
        {
            var result = _converter.CanConvertFrom(null, typeof(object));

            Assert.False(result);
        }

        [Theory]
        [InlineData("1.0.0")]
        [InlineData("2.3.4")]
        [InlineData("1.0.0-beta")]
        [InlineData("1.0.0-alpha.1")]
        [InlineData("1.2.3.4")]
        [InlineData("0.0.1")]
        public void ConvertFrom_ValidVersionString_ReturnsNuGetVersion(string version)
        {
            var result = _converter.ConvertFrom(null, null, version);

            Assert.NotNull(result);
            Assert.IsType<NuGetVersion>(result);
            Assert.Equal(version, ((NuGetVersion)result).OriginalVersion);
        }

        [Fact]
        public void ConvertFrom_ValidVersionString_ReturnsCorrectParsedVersion()
        {
            var result = (NuGetVersion)_converter.ConvertFrom(null, null, "2.3.4");

            Assert.Equal(2, result.Major);
            Assert.Equal(3, result.Minor);
            Assert.Equal(4, result.Patch);
        }

        [Theory]
        [InlineData("not-a-version")]
        [InlineData("abc")]
        [InlineData("1.2.3.4.5")]
        [InlineData("")]
        public void ConvertFrom_InvalidVersionString_ReturnsNull(string invalidVersion)
        {
            var result = _converter.ConvertFrom(null, null, invalidVersion);

            Assert.Null(result);
        }

        [Fact]
        public void ConvertFrom_NullValue_ReturnsNull()
        {
            var result = _converter.ConvertFrom(null, null, null);

            Assert.Null(result);
        }

        [Fact]
        public void ConvertFrom_PrereleaseVersionString_ReturnsPrereleaseVersion()
        {
            var result = (NuGetVersion)_converter.ConvertFrom(null, null, "1.0.0-beta");

            Assert.True(result.IsPrerelease);
            Assert.Equal("beta", result.Release);
        }
    }
}
