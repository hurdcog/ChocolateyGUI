// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="TranslationSourceTests.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Globalization;
using ChocolateyGui.Common.Utilities;
using Xunit;

namespace ChocolateyGui.Common.Tests
{
    public class TranslationSourceTests
    {
        [Fact]
        public void Instance_ReturnsSameSingleton()
        {
            var first = TranslationSource.Instance;
            var second = TranslationSource.Instance;

            Assert.Same(first, second);
        }

        [Fact]
        public void Indexer_NullKey_ReturnsEmptyString()
        {
            var result = TranslationSource.Instance[null];

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Indexer_EmptyKey_ReturnsEmptyString()
        {
            var result = TranslationSource.Instance[string.Empty];

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Indexer_WithParameters_NullKey_ReturnsEmptyString()
        {
            var result = TranslationSource.Instance[null, "param1"];

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Indexer_WithParameters_EmptyKey_ReturnsEmptyString()
        {
            var result = TranslationSource.Instance[string.Empty, "param1"];

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Indexer_WithNullParameters_ReturnsValueWithoutFormatting()
        {
            var source = TranslationSource.Instance;
            var withoutParams = source["VersionNumberProvider_VersionFormat"];
            var withNullParams = source["VersionNumberProvider_VersionFormat", (object[])null];

            Assert.Equal(withoutParams, withNullParams);
        }

        [Fact]
        public void Indexer_WithEmptyParameters_ReturnsValueWithoutFormatting()
        {
            var source = TranslationSource.Instance;
            var withoutParams = source["VersionNumberProvider_VersionFormat"];
            var withEmptyParams = source["VersionNumberProvider_VersionFormat", new object[0]];

            Assert.Equal(withoutParams, withEmptyParams);
        }

        [Fact]
        public void CurrentCulture_WhenChanged_FiresPropertyChangedEvent()
        {
            var source = TranslationSource.Instance;
            var originalCulture = source.CurrentCulture;
            var eventFired = false;
            PropertyChangedEventHandler handler = (s, e) => eventFired = true;

            source.PropertyChanged += handler;
            try
            {
                source.CurrentCulture = new CultureInfo("fr-FR");
            }
            finally
            {
                source.PropertyChanged -= handler;
                source.CurrentCulture = originalCulture;
            }

            Assert.True(eventFired);
        }

        [Fact]
        public void CurrentCulture_WhenSetToSameValue_DoesNotFirePropertyChangedEvent()
        {
            var source = TranslationSource.Instance;
            var originalCulture = source.CurrentCulture;
            var eventCount = 0;
            PropertyChangedEventHandler handler = (s, e) => eventCount++;

            source.PropertyChanged += handler;
            try
            {
                // Setting the same culture should not fire the event
                source.CurrentCulture = originalCulture;
            }
            finally
            {
                source.PropertyChanged -= handler;
            }

            Assert.Equal(0, eventCount);
        }

        [Fact]
        public void CurrentCulture_PropertyChangedEvent_UsesEmptyPropertyName()
        {
            var source = TranslationSource.Instance;
            var originalCulture = source.CurrentCulture;
            string capturedPropertyName = null;
            PropertyChangedEventHandler handler = (s, e) => capturedPropertyName = e.PropertyName;

            source.PropertyChanged += handler;
            try
            {
                source.CurrentCulture = new CultureInfo("de-DE");
            }
            finally
            {
                source.PropertyChanged -= handler;
                source.CurrentCulture = originalCulture;
            }

            Assert.Equal(string.Empty, capturedPropertyName);
        }
    }
}
