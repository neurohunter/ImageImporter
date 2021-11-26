using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ImageImporter.Test.Utilities
{
    public static class TestUtilities
    {
        public static void ValidateConfiguration(
            Configuration configuration, 
            IEnumerable<string> expectedRawTypes, IEnumerable<string> expectedNonRawTypes, IEnumerable<string> expectedVideoTypes, 
            string expectedDestination, string expectedPattern)
        {
            Assert.Multiple(() => 
                { 
                    Assert.IsNotNull(configuration);
                    Assert.IsNotNull(configuration.Destination);
                    if (string.IsNullOrEmpty(expectedDestination) || expectedDestination.Equals("*"))
                    {
                        Assert.IsNotEmpty(configuration.Destination);
                    }
                    else
                    {
                        Assert.AreEqual(expectedDestination, configuration.Destination);
                    }
                    Assert.IsNotNull(configuration.Pattern);
                    if (string.IsNullOrEmpty(expectedPattern))
                    {
                        Assert.IsEmpty(configuration.Pattern);
                    }
                    else
                    {
                        if (expectedPattern.Equals("*"))
                        {
                            Assert.IsNotEmpty(configuration.Pattern);
                        }
                        else
                        {
                            Assert.AreEqual(expectedPattern, configuration.Pattern);
                        }
                    }
                    Assert.IsNotNull(configuration.FileTypes);
                    CollectionAssert.AreEquivalent(expectedRawTypes ?? Array.Empty<string>(), configuration.FileTypes.RawFileTypes);
                    CollectionAssert.AreEquivalent(expectedNonRawTypes ?? Array.Empty<string>(), configuration.FileTypes.NonRawFileTypes);
                    CollectionAssert.AreEquivalent(expectedVideoTypes ?? Array.Empty<string>(), configuration.FileTypes.VideoFileTypes);
                }
            );
        }
    }
}
