using System.Collections.Generic;

namespace ImageDownloader
{
    public interface IImageDownloader
    {
        /// <summary>
        /// Initializes configuration from a configuration file
        /// </summary>
        /// <param name="configurationFilePath">Path to a configuration file</param>
        void Initialize(string configurationFilePath);

        /// <summary>
        /// Gets the files from the input directory and processes them according to defined configuration
        /// </summary>
        /// <param name="inputDrirectory"></param>
        void Download(string inputDrirectory);

        /// <summary>
        /// Gets the files from input directory and puts them to the ouput directory according to file types and defined patterns
        /// </summary>
        /// <param name="inputDirectory">Input directory</param>
        /// <param name="outputDirectory">Output directory</param>
        void Download(string inputDirectory, string outputDirectory);

        /// <summary>
        /// Gets the files from input directory and puts them to the ouput directory according to file types and defined patterns
        /// </summary>
        /// <param name="inputDirectory">Input directory</param>
        /// <param name="outputDirectory">Output directory</param>
        /// <param name="rawFiles">File masks for RAW images</param>
        /// <param name="nonRawFiles">File masks for non-RAW images</param>
        /// <param name="videoFiles">File masks for video files</param>
        /// <param name="pattern">Pattern to rename files to</param>
        void Download(string inputDirectory, string outputDirectory, IEnumerable<string> rawFiles, IEnumerable<string> nonRawFiles, IEnumerable<string> videoFiles, string pattern);
    }
}
