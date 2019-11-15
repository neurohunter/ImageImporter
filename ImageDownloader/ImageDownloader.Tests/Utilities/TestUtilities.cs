using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ImageImporter.Tests.Utilities
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
                    if (ReferenceEquals(expectedDestination,null))
                    {
                        Assert.IsNull(configuration.Destination);
                    }
                    else
                    {
                        Assert.IsNotNull(configuration.Destination);
                        if (expectedDestination.Equals("*"))
                        {
                            Assert.IsNotEmpty(configuration.Destination);
                        }
                        else
                        {
                            Assert.AreEqual(expectedDestination, configuration.Destination);
                        }
                    }
                    if (ReferenceEquals(expectedPattern, null))
                    {
                        Assert.IsNull(configuration.Pattern);
                    }
                    else
                    {
                        Assert.IsNotNull(configuration.Pattern);
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
