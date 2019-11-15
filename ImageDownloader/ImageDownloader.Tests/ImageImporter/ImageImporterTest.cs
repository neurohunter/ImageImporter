using System;
using System.IO;
using NUnit.Framework;
using Importer = ImageImporter.ImageImporter;
using ImageImporter.Tests.Utilities;
using System.Collections.Generic;

namespace ImageImporter.Tests.ImageImporter
{
    [TestFixture]
    public class ImageImporterTest
    {
        private const string TestDataDirectoryPath = @"TestData\ImageImporter";
        private string m_OutputDirectory;
        private IImageImporter m_ImageImporter;

        [SetUp]
        public void SetUp()
        {
            m_OutputDirectory = Path.Combine(Environment.CurrentDirectory, "Output");
            if (Directory.Exists(m_OutputDirectory))
            {
                Directory.Delete(m_OutputDirectory, true);
            }
            Directory.CreateDirectory(m_OutputDirectory);
            m_ImageImporter = new Importer();
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(m_OutputDirectory))
            {
                Directory.Delete(m_OutputDirectory, true);
            }
        }

        [Test]
        public void InitializeFromFileTest()
        {
            var configurationFile = Path.Combine(TestDataDirectoryPath, "..", "ConfigurationProvider", "correct.xml");
            m_ImageImporter.Initialize(configurationFile);
            TestUtilities.ValidateConfiguration(
                    m_ImageImporter.Configuration,
                    new List<string>{".ext1", ".ext2" },new List<string>{".ext3"},new List<string>{".ext4"},
                    "*",
                    "*"
                    );
            
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportRawFileTest(bool initializeFromFile)
        {
            Assert.Fail("Test is not implemented yet");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportVideoFileTest(bool initializeFromFile)
        {
            Assert.Fail("Test is not implemented yet");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportNonRawFileTest(bool initializeFromFile)
        {
            Assert.Fail("Test is not implemented yet");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportMiscFileTest(bool initializeFromFile)
        {
            Assert.Fail("Test is not implemented yet");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void NoFileToImportTest(bool initializeFromFile)
        {
            Assert.Fail("Test is not implemented yet");
        }
    }
}
