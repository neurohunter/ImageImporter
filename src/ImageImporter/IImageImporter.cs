using System;
using System.Collections.Generic;

namespace ImageImporter
{
    public interface IImageImporter
    {
        /// <summary>
        /// ImageImporter configuration
        /// </summary>
        Configuration Configuration{get;}

        /// <summary>
        /// Initializes configuration from a configuration file
        /// </summary>
        /// <param name="configurationFilePath">Path to a configuration file</param>
        void Initialize(string configurationFilePath);

        /// <summary>
        /// Initializes from command-line parameters. All separately passed parameters have priority over configuration file
        /// </summary>
        /// <param name="configurationFilePath">Path to a configuration file</param>
        /// <param name="outputDirectory">Output directory</param>
        /// <param name="rawFiles">Filter mask for RAW image types</param>
        /// <param name="nonRawFiles">Filter mask for non-RAW image types</param>
        /// <param name="videoFiles">Filter mask for video files</param>
        /// <param name="pattern">Pattern to use</param>
        void Initialize(string configurationFilePath, string outputDirectory, IEnumerable<string> rawFiles, IEnumerable<string> nonRawFiles, IEnumerable<string> videoFiles, string pattern);

        /// <summary>
        /// Gets the files from the input directory and processes them according to defined configuration
        /// </summary>
        /// <param name="inputDrirectory"></param>
        void Import(string inputDrirectory);

        /// <summary>
        /// File import process has started
        /// </summary>
        event EventHandler<ImportEventArgs> ImportStarted;
        /// <summary>
        /// A file was copied
        /// </summary>
        event EventHandler<FileEventArgs> FileCopied;
        /// <summary>
        /// An error occured
        /// </summary>
        event EventHandler<FileEventArgs> FileFailed;
        /// <summary>
        /// File import process has completed
        /// </summary>
        event EventHandler<ImportEventArgs> ImportFinished;

    }
}
