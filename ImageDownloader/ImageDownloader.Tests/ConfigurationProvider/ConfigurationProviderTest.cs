using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Tests.ConfigurationProvider
{
    public class Tests
    {
        private ImageDownloader.ConfigurationProvider m_Provider;
        [SetUp]
        public void Setup()
        {
            m_Provider = new ImageDownloader.ConfigurationProvider();
        }

        private static IEnumerable ReadConfigurationFromFileTestData
        {
            get
            {
                yield return new TestCaseData("nonexistent.xml", typeof(System.IO.FileNotFoundException)).SetName("Non-existing configuration file");
                yield return new TestCaseData(System.IO.Path.Combine("ConfigurationProvider","correct.xml"), null).SetName("Correct configuration file");
                yield return new TestCaseData(System.IO.Path.Combine("ConfigurationProvider","malformed.xml"), typeof(System.InvalidOperationException)).SetName("Non-deserializable configuration file");
                yield return new TestCaseData(System.IO.Path.Combine("ConfigurationProvider","incorrect_path.xml"), null).SetName("Deserializable incorrect configuration file");
                yield return new TestCaseData(System.IO.Path.Combine("ConfigurationProvider","incomplete.xml"), null).SetName("Deserializable incomplete configuration file");
            }
        }

        [TestCaseSource(nameof(ReadConfigurationFromFileTestData))]
        public void ReadConfigurationFromFileTest(string filePath, Type thrownExceptionType)
        {
            if (thrownExceptionType!=null)
            {                
                Assert.Throws(
                    Is.InstanceOf(thrownExceptionType), 
                    () => 
                    { 
                        m_Provider.ReadConfigurationFromFile(filePath);
                    }
                );
            }
            else
            {
                var configuration = m_Provider.ReadConfigurationFromFile(filePath);
                Assert.IsNotNull(configuration);
            }
        }

        private static IEnumerable InitializeFromParametersTestCaseData
        {
            get
            {
                yield return new TestCaseData(new List<string>{ ".cr2", ".cr3",}, new List<string>{ ".jpg"}, new List<string>{ ".mov"}, @"c:\test", "YYYY_MM_DD").SetName("Fully filled");
                yield return new TestCaseData(null, null, null, null, null).SetName("All null");
                yield return new TestCaseData(new List<string>{ ".cr2", ".cr3",}, new List<string>{ ".jpg"}, new List<string>{ ".mov"}, @"c:\test", null).SetName("No pattern");
                yield return new TestCaseData(new List<string>{ ".cr2", ".cr3",}, new List<string>{ ".jpg"}, new List<string>{ ".mov"}, null, "YYYY_MM_DD").SetName("No directory");
                yield return new TestCaseData(new List<string>{ ".cr2", ".cr3",}, new List<string>{ ".jpg"}, null, @"c:\test", "YYYY_MM_DD").SetName("No video");
                yield return new TestCaseData(new List<string>{ ".cr2", ".cr3",}, null, new List<string>{ ".mov"}, @"c:\test", "YYYY_MM_DD").SetName("No jpg");
                yield return new TestCaseData(null, new List<string>{ ".jpg"}, new List<string>{ ".mov"}, @"c:\test", "YYYY_MM_DD").SetName("No RAW");
            }
        }

        [TestCaseSource(nameof(InitializeFromParametersTestCaseData))]
        public void InitializeFromParametersTest(IEnumerable<string> rawTypes, IEnumerable<string> nonRawTypes, IEnumerable<string> videoTypes, string destination, string pattern)
        {
            var configuration = m_Provider.InitializeFromParameters(rawTypes, nonRawTypes, videoTypes, destination, pattern);
            Assert.Multiple(() => 
                { 
                    CollectionAssert.AreEquivalent(rawTypes ?? new string[0], configuration.FileTypes.RawFileRypes);
                    CollectionAssert.AreEquivalent(nonRawTypes ?? new string[0], configuration.FileTypes.NonRawFileRypes);
                    CollectionAssert.AreEquivalent(videoTypes ?? new string[0], configuration.FileTypes.VideoFileTypes);
                    Assert.AreEqual(destination, configuration.Destination);
                    Assert.AreEqual(pattern, configuration.Pattern);
                }
            );
        }
    }
}