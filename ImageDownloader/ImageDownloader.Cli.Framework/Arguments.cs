using System.Collections.Generic;

namespace ImageImporter.Cli.Framework
{
    /// <summary>
    /// Which files to process
    /// </summary>
    public enum FilesToProcess
    {
        /// <summary>
        /// All fiels in the directory
        /// </summary>
        All = 0,
        /// <summary>
        /// Only new files
        /// </summary>
        New = 1
    }

    internal class Arguments
    {
        /// <summary>
        /// Use current directory
        /// </summary>
        public bool NoPathProvided
        {
            get => OutputDirectory.Equals(System.Environment.CurrentDirectory);
        }
        /// <summary>
        /// Directory from which images are imported
        /// </summary>
        public string InputDirectory { set; get; }
        /// <summary>
        /// Directory to which images are imported
        /// </summary>
        public string OutputDirectory { set; get; }
        /// <summary>
        /// Path to a configuration file
        /// </summary>
        public string ConfigurationPath {set;get;}
        /// <summary>
        /// List of file extensions treated as RAW (e.g. cr2, arw)
        /// </summary>
        public List<string> RawFileExtensions{get;set;}
        /// <summary>
        /// List of file extensions treated as non-RAW (e.g. jpg, heic)
        /// </summary>
        public List<string> NonRawFileExtensions {get;set;}
        /// <summary>
        /// List of video file extensions (e.g. mov)
        /// </summary>
        public List<string> VideoFileExtensions { get;set;}

        public FilesToProcess FilesToProcess { get; set; }

        public Arguments()
        {
            RawFileExtensions = new List<string>();
            NonRawFileExtensions = new List<string>();
            VideoFileExtensions = new List<string>();
            FilesToProcess = FilesToProcess.All;
        }
    }
}
