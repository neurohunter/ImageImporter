using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Importer = ImageImporter.ImageImporter;
using ImageImporter.Test.Utilities;
using System.Collections.Generic;

namespace ImageImporter.Test.ImageImporter
{
    [TestFixture]
    public class ImageImporterTest
    {
        private const string TestDataDirectoryPath = @"TestData\ImageImporter";
        private const string ConfigsDirectory = "Configs";
        private const string DataDirectory = "Input";
        private const string OutputDirectory = "Output";
        private List<string> m_RawFiles = new List<string>{ ".cr2", ".arw"};
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
                new ImageImporterTestFileDescription("IMG_5283.CR2", new DateTime(2015, 06, 15), DateTime.Now, FileKind.RawImage),
                new ImageImporterTestFileDescription("IMG_2584.heic", new DateTime(2015, 06, 15), DateTime.Now, FileKind.Unrecognized),
                new ImageImporterTestFileDescription("2016-02-27 10.46.01.jpg", new DateTime(2016, 02, 27), DateTime.Now, FileKind.JpegImage),
                new ImageImporterTestFileDescription("DSC01668.ARW", new DateTime(2019, 09, 26), DateTime.Now, FileKind.RawImage),
            };

            TouchReferenceFiles();

            m_ImageImporter = new Importer();
        }

        private void TouchReferenceFiles()
        {
            foreach(var referenceFile in m_ReferenceFileDescriptions)
            {
                _ = new FileInfo(Path.Combine(TestDataDirectoryPath, DataDirectory, referenceFile.FileName))
                {
                    LastAccessTime = DateTime.Now
                };
            }
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
            var configPath = initializeFromFile ? Path.Combine(TestDataDirectoryPath,ConfigsDirectory,"config-all.xml") : string.Empty;
            if (initializeFromFile)
            {                
                m_ImageImporter.Initialize(configPath);
            }
            else
            {
                m_ImageImporter.Initialize(configPath, m_OutputDirectory, m_RawFiles, m_NonRawFiles, m_VideoFiles, string.Empty);                
            }
            m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory));
            ValidateOutput(m_RawFiles, m_NonRawFiles, m_VideoFiles);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportRawFileTest(bool initializeFromFile)
        {   
            var configPath = initializeFromFile ? Path.Combine(TestDataDirectoryPath,ConfigsDirectory,"config-raw.xml") : string.Empty;
            if (initializeFromFile)
            {                
                m_ImageImporter.Initialize(configPath);
            }
            else
            {
                m_ImageImporter.Initialize(configPath, m_OutputDirectory, m_RawFiles, new List<string>(), new List<string>(), string.Empty);                
            }
            m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory));
            ValidateOutput(m_RawFiles, new List<string>(), new List<string>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportNonRawFileTest(bool initializeFromFile)
        {
            var configPath = initializeFromFile ? Path.Combine(TestDataDirectoryPath,ConfigsDirectory,"config-nonraw.xml") : string.Empty;
            if (initializeFromFile)
            {                
                m_ImageImporter.Initialize(configPath);
            }
            else
            {
                m_ImageImporter.Initialize(configPath, m_OutputDirectory, new List<string>(), m_NonRawFiles, new List<string>(), string.Empty);                
            }
            m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory));
            ValidateOutput(new List<string>(), m_NonRawFiles, new List<string>());
        }

        [TestCase(true),Ignore("Get a small reference file set")]
        [TestCase(false)]
        public void ImportVideoFileTest(bool initializeFromFile)
        {
            var configPath = initializeFromFile ? Path.Combine(TestDataDirectoryPath,ConfigsDirectory,"config-video.xml") : string.Empty;
            if (initializeFromFile)
            {                
                m_ImageImporter.Initialize(configPath);
            }
            else
            {
                m_ImageImporter.Initialize(configPath, m_OutputDirectory, m_RawFiles, m_NonRawFiles, m_VideoFiles, string.Empty);                
            }
            m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory));
            Assert.Fail("Test is not implemented yet");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ImportMiscFileTest(bool initializeFromFile)
        {
            var configPath = initializeFromFile ? Path.Combine(TestDataDirectoryPath,ConfigsDirectory,"config-misc.xml") : string.Empty;
            if (initializeFromFile)
            {                
                m_ImageImporter.Initialize(configPath);
            }
            else
            {
                m_ImageImporter.Initialize(configPath, m_OutputDirectory, new List<string>(), new List<string>(), new List<string>(), string.Empty);                
            }
            m_ImageImporter.Import(Path.Combine(TestDataDirectoryPath,DataDirectory));
            ValidateOutput(new List<string>(), new List<string>(), new List<string>());
        }

        [Test]
        public void NoFileToImportTest()
        {
            var inputDirectory = Path.Combine(TestDataDirectoryPath, "EmptyDirectory");
            Directory.CreateDirectory(inputDirectory);
            m_ImageImporter.Import(inputDirectory);
            Assert.IsTrue(Directory.Exists(m_OutputDirectory));
            Assert.IsEmpty(Directory.GetFileSystemEntries(m_OutputDirectory));
            Directory.Delete(inputDirectory);
        }

        public void ValidateOutput(List<string> rawFileTypes, List<string> nonRawFileTypes, List<string> videoFileTypes)
        {
            var outputDirectory = new DirectoryInfo(m_OutputDirectory);
            Assert.IsTrue(outputDirectory.Exists);
            foreach(var referenceFile in m_ReferenceFileDescriptions)
            {
                var files = outputDirectory.GetFiles(referenceFile.FileName, SearchOption.AllDirectories);
                Assert.IsNotNull(files);
                Assert.That(files.Count, Is.AtMost(1));
                var requiredFileKind = FileKind.Unrecognized;
                if (rawFileTypes.Contains(referenceFile.Extension.ToLowerInvariant()))
                {
                    requiredFileKind = FileKind.RawImage;
                }
                if (nonRawFileTypes.Contains(referenceFile.Extension.ToLowerInvariant()))
                {
                    requiredFileKind = FileKind.JpegImage;
                }
                if (videoFileTypes.Contains(referenceFile.Extension.ToLowerInvariant()))
                {
                    requiredFileKind = FileKind.Video;
                }
                var expectedPath = referenceFile.GetExpectedPath(requiredFileKind);
                var expectedPathPresent = files[0].FullName.EndsWith(expectedPath);                
                Assert.IsTrue(
                    expectedPathPresent,
                    $"Expected {expectedPath}, got {files[0].FullName}"
                    );
            }
        }

    }
}
