// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ConfigServiceTests.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Services;
using LiteDB;
using Xunit;

namespace ChocolateyGui.Common.Tests
{
    public class ConfigServiceTests
    {
        private static ConfigService CreateConfigService(
            AppConfiguration globalConfig = null,
            AppConfiguration userConfig = null)
        {
            var globalDb = new LiteDatabase(":memory:");
            var userDb = new LiteDatabase(":memory:");

            if (globalConfig != null)
            {
                globalConfig.Id = "v0.18.0";
                globalDb.GetCollection<AppConfiguration>(nameof(AppConfiguration)).Insert(globalConfig);
            }

            if (userConfig != null)
            {
                userConfig.Id = "v0.18.0";
                userDb.GetCollection<AppConfiguration>(nameof(AppConfiguration)).Insert(userConfig);
            }

            return new ConfigService(globalDb, userDb);
        }

        [Fact]
        public void Constructor_WithNullGlobalDatabase_SetsGlobalCollectionToNull()
        {
            var userDb = new LiteDatabase(":memory:");
            var service = new ConfigService(null, userDb);

            Assert.Null(service.GlobalCollection);
        }

        [Fact]
        public void Constructor_WithNullGlobalDatabase_UsesDefaultGlobalSettings()
        {
            var userDb = new LiteDatabase(":memory:");
            var service = new ConfigService(null, userDb);

            Assert.Equal("60", service.GlobalAppConfiguration.OutdatedPackagesCacheDurationInMinutes);
            Assert.True(service.GlobalAppConfiguration.UseKeyboardBindings);
            Assert.True(service.GlobalAppConfiguration.DefaultToTileViewForLocalSource);
            Assert.True(service.GlobalAppConfiguration.DefaultToTileViewForRemoteSource);
        }

        [Fact]
        public void Constructor_WithEmptyDatabase_UsesDefaultGlobalSettings()
        {
            var service = CreateConfigService();

            Assert.Equal("60", service.GlobalAppConfiguration.OutdatedPackagesCacheDurationInMinutes);
            Assert.True(service.GlobalAppConfiguration.UseKeyboardBindings);
            Assert.True(service.GlobalAppConfiguration.DefaultToTileViewForLocalSource);
            Assert.True(service.GlobalAppConfiguration.DefaultToTileViewForRemoteSource);
        }

        [Fact]
        public void Constructor_WithExistingGlobalConfig_LoadsFromDatabase()
        {
            var storedGlobal = new AppConfiguration
            {
                OutdatedPackagesCacheDurationInMinutes = "120",
                UseKeyboardBindings = false,
                DefaultToTileViewForLocalSource = false,
                DefaultToTileViewForRemoteSource = false
            };

            var service = CreateConfigService(globalConfig: storedGlobal);

            Assert.Equal("120", service.GlobalAppConfiguration.OutdatedPackagesCacheDurationInMinutes);
            Assert.False(service.GlobalAppConfiguration.UseKeyboardBindings);
            Assert.False(service.GlobalAppConfiguration.DefaultToTileViewForLocalSource);
            Assert.False(service.GlobalAppConfiguration.DefaultToTileViewForRemoteSource);
        }

        [Fact]
        public void SetEffectiveConfiguration_WithOnlyGlobalValues_UsesGlobalValues()
        {
            var globalConfig = new AppConfiguration
            {
                OutdatedPackagesCacheDurationInMinutes = "90",
                DefaultToTileViewForLocalSource = false
            };

            var service = CreateConfigService(globalConfig: globalConfig);
            service.SetEffectiveConfiguration();

            Assert.Equal("90", service.EffectiveAppConfiguration.OutdatedPackagesCacheDurationInMinutes);
            Assert.False(service.EffectiveAppConfiguration.DefaultToTileViewForLocalSource);
        }

        [Fact]
        public void SetEffectiveConfiguration_WithOnlyUserValues_UsesUserValues()
        {
            var userConfig = new AppConfiguration
            {
                OutdatedPackagesCacheDurationInMinutes = "45"
            };

            var service = CreateConfigService(userConfig: userConfig);
            service.SetEffectiveConfiguration();

            Assert.Equal("45", service.EffectiveAppConfiguration.OutdatedPackagesCacheDurationInMinutes);
        }

        [Fact]
        public void SetEffectiveConfiguration_UserOverridesGlobal_UsesUserValue()
        {
            var globalConfig = new AppConfiguration
            {
                OutdatedPackagesCacheDurationInMinutes = "60"
            };

            var userConfig = new AppConfiguration
            {
                OutdatedPackagesCacheDurationInMinutes = "30"
            };

            var service = CreateConfigService(globalConfig: globalConfig, userConfig: userConfig);
            service.SetEffectiveConfiguration();

            Assert.Equal("30", service.EffectiveAppConfiguration.OutdatedPackagesCacheDurationInMinutes);
        }

        [Fact]
        public void SetEffectiveConfiguration_UserAndGlobalSameValue_UsesGlobalValue()
        {
            var globalConfig = new AppConfiguration
            {
                OutdatedPackagesCacheDurationInMinutes = "60"
            };

            var userConfig = new AppConfiguration
            {
                OutdatedPackagesCacheDurationInMinutes = "60"
            };

            var service = CreateConfigService(globalConfig: globalConfig, userConfig: userConfig);
            service.SetEffectiveConfiguration();

            Assert.Equal("60", service.EffectiveAppConfiguration.OutdatedPackagesCacheDurationInMinutes);
        }

        [Fact]
        public void SetEffectiveConfiguration_BothValuesNull_DoesNotSetEffectiveValue()
        {
            var service = CreateConfigService();
            service.SetEffectiveConfiguration();

            // DefaultSourceName is not set in defaults, so it should remain null
            Assert.Null(service.EffectiveAppConfiguration.DefaultSourceName);
        }

        [Fact]
        public void SetEffectiveConfiguration_UserFeatureOverridesGlobal_UsesUserValue()
        {
            var globalConfig = new AppConfiguration
            {
                ShowConsoleOutput = false
            };

            var userConfig = new AppConfiguration
            {
                ShowConsoleOutput = true
            };

            var service = CreateConfigService(globalConfig: globalConfig, userConfig: userConfig);
            service.SetEffectiveConfiguration();

            Assert.True(service.EffectiveAppConfiguration.ShowConsoleOutput);
        }

        [Fact]
        public void SetEffectiveConfiguration_OnlyGlobalFeature_UsesGlobalValue()
        {
            var globalConfig = new AppConfiguration
            {
                ShowConsoleOutput = true
            };

            var service = CreateConfigService(globalConfig: globalConfig);
            service.SetEffectiveConfiguration();

            Assert.True(service.EffectiveAppConfiguration.ShowConsoleOutput);
        }

        [Fact]
        public void GetEffectiveConfiguration_WhenEffectiveConfigurationIsNull_CallsSetEffectiveConfiguration()
        {
            var service = CreateConfigService();

            var result = service.GetEffectiveConfiguration();

            Assert.NotNull(result);
        }

        [Fact]
        public void GetEffectiveConfiguration_WhenAlreadySet_ReturnsCachedValue()
        {
            var service = CreateConfigService();
            service.SetEffectiveConfiguration();
            var firstCall = service.GetEffectiveConfiguration();

            var secondCall = service.GetEffectiveConfiguration();

            Assert.Same(firstCall, secondCall);
        }

        [Fact]
        public void GetGlobalConfiguration_ReturnsGlobalAppConfiguration()
        {
            var globalConfig = new AppConfiguration
            {
                OutdatedPackagesCacheDurationInMinutes = "75"
            };

            var service = CreateConfigService(globalConfig: globalConfig);

            var result = service.GetGlobalConfiguration();

            Assert.Equal("75", result.OutdatedPackagesCacheDurationInMinutes);
        }

        [Fact]
        public void GetFeatures_Global_ReturnsAllFeatureProperties()
        {
            var service = CreateConfigService();
            service.SetEffectiveConfiguration();

            var features = service.GetFeatures(global: true).ToList();

            Assert.NotEmpty(features);
            // Verify well-known features are present
            Assert.Contains(features, f => f.Title == "ShowConsoleOutput");
            Assert.Contains(features, f => f.Title == "DefaultToTileViewForLocalSource");
            Assert.Contains(features, f => f.Title == "DefaultToTileViewForRemoteSource");
            Assert.Contains(features, f => f.Title == "ExcludeInstalledPackages");
        }

        [Fact]
        public void GetFeatures_ReturnsFeaturesSortedByTitle()
        {
            var service = CreateConfigService();
            service.SetEffectiveConfiguration();

            var features = service.GetFeatures(global: false).ToList();
            var titles = features.Select(f => f.Title).ToList();
            var sortedTitles = titles.OrderBy(t => t).ToList();

            Assert.Equal(sortedTitles, titles);
        }

        [Fact]
        public void GetFeatures_DefaultConfiguration_DefaultToTileViewForLocalSourceIsEnabled()
        {
            var service = CreateConfigService();
            service.SetEffectiveConfiguration();

            var features = service.GetFeatures(global: true).ToList();
            var feature = features.FirstOrDefault(f => f.Title == "DefaultToTileViewForLocalSource");

            Assert.NotNull(feature);
            Assert.True(feature.Enabled);
        }

        [Fact]
        public void GetFeatures_FeatureWithResourceKeys_ReturnsTitleAsKey()
        {
            var service = CreateConfigService();
            service.SetEffectiveConfiguration();

            var featuresWithKeys = service.GetFeatures(global: true, useResourceKeys: true).ToList();

            Assert.NotEmpty(featuresWithKeys);
            // Each feature should still have a Title
            Assert.All(featuresWithKeys, f => Assert.NotEmpty(f.Title));
        }

        [Fact]
        public void GetSettings_ReturnsAllConfigProperties()
        {
            var service = CreateConfigService();
            service.SetEffectiveConfiguration();

            var settings = service.GetSettings(global: true).ToList();

            Assert.NotEmpty(settings);
            Assert.Contains(settings, s => s.Key == "OutdatedPackagesCacheDurationInMinutes");
            Assert.Contains(settings, s => s.Key == "DefaultSourceName");
            Assert.Contains(settings, s => s.Key == "UseLanguage");
        }

        [Fact]
        public void GetSettings_ReturnsSortedByKey()
        {
            var service = CreateConfigService();
            service.SetEffectiveConfiguration();

            var settings = service.GetSettings(global: false).ToList();
            var keys = settings.Select(s => s.Key).ToList();
            var sortedKeys = keys.OrderBy(k => k).ToList();

            Assert.Equal(sortedKeys, keys);
        }

        [Fact]
        public void GetSettings_DefaultOutdatedPackagesCacheDurationInMinutes_IsCorrectDefault()
        {
            var service = CreateConfigService();
            service.SetEffectiveConfiguration();

            var settings = service.GetSettings(global: true).ToList();
            var setting = settings.FirstOrDefault(s => s.Key == "OutdatedPackagesCacheDurationInMinutes");

            Assert.NotNull(setting);
            Assert.Equal("60", setting.Value);
        }

        [Fact]
        public void UpdateSettings_UserSettings_UpdatesUserDatabase()
        {
            var userDb = new LiteDatabase(":memory:");
            var globalDb = new LiteDatabase(":memory:");
            var service = new ConfigService(globalDb, userDb);

            var updatedSettings = new AppConfiguration
            {
                Id = "v0.18.0",
                OutdatedPackagesCacheDurationInMinutes = "180"
            };

            service.UpdateSettings(updatedSettings, global: false);

            var storedSettings = userDb
                .GetCollection<AppConfiguration>(nameof(AppConfiguration))
                .FindById("v0.18.0");
            Assert.Equal("180", storedSettings.OutdatedPackagesCacheDurationInMinutes);
        }

        [Fact]
        public void UpdateSettings_WhenGlobalCollectionIsNull_DoesNotThrow()
        {
            var userDb = new LiteDatabase(":memory:");
            var service = new ConfigService(null, userDb);

            var updatedSettings = new AppConfiguration
            {
                Id = "v0.18.0",
                OutdatedPackagesCacheDurationInMinutes = "120"
            };

            // Should not throw even when GlobalCollection is null
            service.UpdateSettings(updatedSettings, global: true);
        }

        [Fact]
        public void UpdateSettings_FiresSettingsChangedEvent()
        {
            var userDb = new LiteDatabase(":memory:");
            var globalDb = new LiteDatabase(":memory:");
            var service = new ConfigService(globalDb, userDb);
            service.SetEffectiveConfiguration();

            var eventFired = false;
            service.SettingsChanged += (s, e) => eventFired = true;

            service.UpdateSettings(
                new AppConfiguration { Id = "v0.18.0" },
                global: false);

            Assert.True(eventFired);
        }

        [Fact]
        public void UpdateSettings_UpdatesExistingRecord_WhenRecordAlreadyExists()
        {
            var userDb = new LiteDatabase(":memory:");
            var globalDb = new LiteDatabase(":memory:");
            var service = new ConfigService(globalDb, userDb);
            service.SetEffectiveConfiguration();

            // First update — inserts
            service.UpdateSettings(
                new AppConfiguration { Id = "v0.18.0", OutdatedPackagesCacheDurationInMinutes = "30" },
                global: false);

            // Second update — updates
            service.UpdateSettings(
                new AppConfiguration { Id = "v0.18.0", OutdatedPackagesCacheDurationInMinutes = "90" },
                global: false);

            var storedSettings = userDb
                .GetCollection<AppConfiguration>(nameof(AppConfiguration))
                .FindById("v0.18.0");
            Assert.Equal("90", storedSettings.OutdatedPackagesCacheDurationInMinutes);
        }
    }
}
