using System.Collections.Generic;
using System.Linq;

namespace ImageImporter.FileProcessor
{
    public class FileProcessorFactory
    {
        /// <summary>
        /// Available file processors
        /// </summary>
        private readonly List<IFileProcessor> m_FileProcessors;
        
        public FileProcessorFactory()
        {
            m_FileProcessors = new List<IFileProcessor>
            {
                new NullFileProcessor(),
                new GenericFileProcessor(),
                new ExifFileProcessor()
            };
        }

        /// <summary>
        /// Provides a processor for a specified file kind
        /// </summary>
        /// <param name="fileKind">File kind to process</param>
        /// <returns>File processor</returns>
        public IFileProcessor ProvideProcessorForFile(FileKind fileKind)
        {
            switch (fileKind)
            {
                case FileKind.Unrecognized:
                    return m_FileProcessors.OfType<GenericFileProcessor>().First();
                case FileKind.RawImage:
                case FileKind.JpegImage:
                case FileKind.Video:
                    return m_FileProcessors.OfType<ExifFileProcessor>().First();
                default:
                    return m_FileProcessors.OfType<NullFileProcessor>().First();
            }
        }
    }
}
