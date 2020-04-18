using System;
using NUnit.Framework;
using ImageImporter.FileProcessor;

namespace ImageImporter.Tests.FileProcessor
{
    [TestFixture]
    public class FileProcessorFactoryTest
    {
        private FileProcessorFactory m_FileProcessorFactory;

        [SetUp]
        public void SetUp()
        {
            m_FileProcessorFactory = new FileProcessorFactory();
        }

        [TestCase(FileKind.JpegImage, typeof(ExifFileProcessor))]
        [TestCase(FileKind.RawImage, typeof(ExifFileProcessor))]
        [TestCase(FileKind.Video, typeof(ExifFileProcessor))]
        [TestCase(FileKind.Unrecognized, typeof(GenericFileProcessor))]
        [TestCase(10, typeof(NullFileProcessor))]
        public void ProvideFileProcessorTest(FileKind fileKind, Type expectedFileProcessor)
        {
            var fileProcessor = m_FileProcessorFactory.ProvideProcessorForFile(fileKind);
            Assert.IsInstanceOf(expectedFileProcessor, fileProcessor);
        }
    }
}
