using System;
using NUnit.Framework;
using ImageDownloader;
using System.Collections;

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

        private static IEnumerable TestCaseData
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

        [TestCaseSource(nameof(TestCaseData))]
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
    }
}