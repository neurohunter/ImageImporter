using System;
using System.IO;
using System.Linq;
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
        private const string ConfigsDirectory = "Configs";
        private const string DataDirectory = "Input";
        private const string OutputDirectory = "Output";
        private List<string> m_RawFiles = new List<string>{ ".cr2"};
        private List<string> m_NonRawFiles = new List<string>{ ".jpg"};
        private List<string> m_VideoFiles = new List<string>{ ".mov"};
        private List<ImageImporterTestFileDescription> m_ReferenceFileDescriptions;
        private string m_OutputDirectory;
        private IImageImporter m_ImageImporter;

        [SetUp]
        public void SetUp()
        {
            m_OutputDirectory = Path.Combine(Environment.CurrentDirectory, OutputDirectory);
            if (Directory.Exists(m_OutputDirectory))
            {
                Directory.Delete(m_OutputDirectory, true);
            }
            Directory.CreateDirectory(m_OutputDirectory);
            m_ReferenceFileDescriptions = new List<ImageImporterTestFileDescription>
            {
                new ImageImporterTestFileDescription("IMG_5283.CR2", new DateTime(2015, 06, 15), new DateTime(2019, 11, 15), FileKind.RawImage),
                new ImageImporterTestFileDescription("IMG_2584.heic", new DateTime(2015, 06, 15), new DateTime(2019, 11, 15), FileKind.Unrecognized),
                new ImageImporterTestFileDescription("2016-02-27 10.46.01.jpg", new DateTime(2016, 02, 27), new DateTime(2019, 11, 15), FileKind.JpegImage),
            };
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
        public void ImportFileTest(bool initializeFromFile)
        {
            if (initializeFromFile)
            {
                var configPath = Path.Combine(TestDataDirectoryPath,ConfigsDirectory,"config-all.xml");
                m_ImageImporter.Initialize(configPath);
                m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory));
            }
            else
            {
                m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory), m_OutputDirectory, m_RawFiles, m_NonRawFiles, m_VideoFiles, string.Empty);
            }
            var outputDirectory = new DirectoryInfo(m_OutputDirectory);
            Assert.IsTrue(outputDirectory.Exists);
            foreach(var referenceFile in m_ReferenceFileDescriptions)
            {
                var files = outputDirectory.GetFiles(referenceFile.FileName, SearchOption.AllDirectories);
                Assert.IsNotNull(files);
                Assert.That(files.Count, Is.AtMost(1));
                var expectedPath = referenceFile.GetExpectedPath(referenceFile.FileKind);
                Assert.IsTrue(
                    files[0].FullName.EndsWith(expectedPath),
                    $"Expected {expectedPath}, got {files[0].FullName}"
                    );
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportRawFileTest(bool initializeFromFile)
        {   
            if (initializeFromFile)
            {
                var configPath = Path.Combine(TestDataDirectoryPath,ConfigsDirectory,"config-raw.xml");
                m_ImageImporter.Initialize(configPath);
                m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory));
            }
            else
            {
                m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory), m_OutputDirectory, m_RawFiles, new List<string>(), new List<string>(), string.Empty);
            }
            var outputDirectory = new DirectoryInfo(m_OutputDirectory);
            Assert.IsTrue(outputDirectory.Exists);
            foreach(var referenceFile in m_ReferenceFileDescriptions)
            {
                var files = outputDirectory.GetFiles(referenceFile.FileName, SearchOption.AllDirectories);
                Assert.IsNotNull(files);
                Assert.That(files.Count, Is.AtMost(1));
                var expectedPath = referenceFile.GetExpectedPath(m_RawFiles.Contains(referenceFile.Extension.ToLowerInvariant()) ? referenceFile.FileKind : FileKind.Unrecognized);
                Assert.IsTrue(
                    files[0].FullName.EndsWith(expectedPath),
                    $"Expected {expectedPath}, got {files[0].FullName}"
                    );
            }
        }

        [TestCase(true),Ignore("Get a small reference file set")]
        [TestCase(false)]
        public void ImportVideoFileTest(bool initializeFromFile)
        {
            if (initializeFromFile)
            {
                var configPath = Path.Combine(TestDataDirectoryPath,ConfigsDirectory,"config-video.xml");
                m_ImageImporter.Initialize(configPath);
                m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory));
            }
            else
            {
                m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory), m_OutputDirectory, new List<string>(), new List<string>(), m_VideoFiles, string.Empty);
            }
            Assert.Fail("Test is not implemented yet");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportNonRawFileTest(bool initializeFromFile)
        {
            if (initializeFromFile)
            {
                var configPath = Path.Combine(TestDataDirectoryPath,ConfigsDirectory,"config-nonraw.xml");
                m_ImageImporter.Initialize(configPath);
                m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory));
            }
            else
            {
                m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory), m_OutputDirectory, new List<string>(), m_NonRawFiles, new List<string>(), string.Empty);
            }
            var outputDirectory = new DirectoryInfo(m_OutputDirectory);
            Assert.IsTrue(outputDirectory.Exists);
            foreach(var referenceFile in m_ReferenceFileDescriptions)
            {
                var files = outputDirectory.GetFiles(referenceFile.FileName, SearchOption.AllDirectories);
                Assert.IsNotNull(files);
                Assert.That(files.Count, Is.AtMost(1));
                var expectedPath = referenceFile.GetExpectedPath(m_NonRawFiles.Contains(referenceFile.Extension.ToLowerInvariant()) ? referenceFile.FileKind : FileKind.Unrecognized);
                Assert.IsTrue(
                    files[0].FullName.EndsWith(expectedPath),
                    $"Expected {expectedPath}, got {files[0].FullName}"
                    );
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportMiscFileTest(bool initializeFromFile)
        {
            if (initializeFromFile)
            {
                var configPath = Path.Combine(TestDataDirectoryPath,ConfigsDirectory,"config-misc.xml");
                m_ImageImporter.Initialize(configPath);
                m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory));
            }
            else
            {
                m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory), m_OutputDirectory, new List<string>(), new List<string>(), new List<string>(), string.Empty);
            }
            var outputDirectory = new DirectoryInfo(m_OutputDirectory);
            Assert.IsTrue(outputDirectory.Exists);
            foreach(var referenceFile in m_ReferenceFileDescriptions)
            {
                var files = outputDirectory.GetFiles(referenceFile.FileName, SearchOption.AllDirectories);
                Assert.IsNotNull(files);
                Assert.That(files.Count, Is.AtMost(1));
                var expectedPath = referenceFile.GetExpectedPath(FileKind.Unrecognized);
                Assert.IsTrue(
                    files[0].FullName.EndsWith(expectedPath),
                    $"Expected {expectedPath}, got {files[0].FullName}"
                    );
            }
        }

        [Test]
        public void NoFileToImportTest()
        {
            var inputDirectory = Path.Combine(TestDataDirectoryPath, "EmptyDirectory");
            Directory.CreateDirectory(inputDirectory);
            m_ImageImporter.Import(inputDirectory, m_OutputDirectory);
            Assert.IsTrue(Directory.Exists(m_OutputDirectory));
            Assert.IsEmpty(Directory.GetFileSystemEntries(m_OutputDirectory));
            Directory.Delete(inputDirectory);
        }
    }
}
