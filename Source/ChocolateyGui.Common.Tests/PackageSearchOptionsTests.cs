// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageSearchOptionsTests.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Common.Models;
using Xunit;

namespace ChocolateyGui.Common.Tests
{
    public class PackageSearchOptionsTests
    {
        [Fact]
        public void Constructor_SetsAllPropertiesCorrectly()
        {
            var options = new PackageSearchOptions(
                pageSize: 30,
                currentPage: 1,
                sortColumn: "Title",
                includePrerelease: true,
                includeAllVersions: false,
                matchWord: true,
                source: "https://community.chocolatey.org/api/v2/");

            Assert.Equal(30, options.PageSize);
            Assert.Equal(1, options.CurrentPage);
            Assert.Equal("Title", options.SortColumn);
            Assert.True(options.IncludePrerelease);
            Assert.False(options.IncludeAllVersions);
            Assert.True(options.MatchQuery);
            Assert.Equal("https://community.chocolatey.org/api/v2/", options.Source);
        }

        [Fact]
        public void Constructor_WithDefaults_SetsPropertiesCorrectly()
        {
            var options = new PackageSearchOptions(
                pageSize: 50,
                currentPage: 0,
                sortColumn: string.Empty,
                includePrerelease: false,
                includeAllVersions: false,
                matchWord: false,
                source: null);

            Assert.Equal(50, options.PageSize);
            Assert.Equal(0, options.CurrentPage);
            Assert.Equal(string.Empty, options.SortColumn);
            Assert.False(options.IncludePrerelease);
            Assert.False(options.IncludeAllVersions);
            Assert.False(options.MatchQuery);
            Assert.Null(options.Source);
        }

        [Fact]
        public void DefaultConstructor_ProducesDefaultValues()
        {
            var options = default(PackageSearchOptions);

            Assert.Equal(0, options.PageSize);
            Assert.Equal(0, options.CurrentPage);
            Assert.Null(options.SortColumn);
            Assert.False(options.IncludePrerelease);
            Assert.False(options.IncludeAllVersions);
            Assert.False(options.MatchQuery);
            Assert.Null(options.Source);
            Assert.Null(options.TagsQuery);
        }

        [Fact]
        public void TagsQuery_CanBeSetAfterConstruction()
        {
            var options = new PackageSearchOptions(10, 0, "Title", false, false, false, null);
            options.TagsQuery = new[] { "tag1", "tag2" };

            Assert.Equal(2, options.TagsQuery.Length);
            Assert.Equal("tag1", options.TagsQuery[0]);
            Assert.Equal("tag2", options.TagsQuery[1]);
        }

        [Fact]
        public void Properties_CanBeSetViaPropertySetters()
        {
            var options = new PackageSearchOptions(10, 0, "Title", false, false, false, null);

            options.PageSize = 100;
            options.CurrentPage = 5;
            options.SortColumn = "Downloads";
            options.IncludePrerelease = true;
            options.IncludeAllVersions = true;
            options.MatchQuery = true;
            options.Source = "https://example.com/feed";

            Assert.Equal(100, options.PageSize);
            Assert.Equal(5, options.CurrentPage);
            Assert.Equal("Downloads", options.SortColumn);
            Assert.True(options.IncludePrerelease);
            Assert.True(options.IncludeAllVersions);
            Assert.True(options.MatchQuery);
            Assert.Equal("https://example.com/feed", options.Source);
        }
    }
}
