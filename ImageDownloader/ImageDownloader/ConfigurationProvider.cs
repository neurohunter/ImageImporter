using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ImageImporter
{
    /// <summary>
    /// Camera type (defines file types to be used)
    /// </summary>
    public enum CameraType
    {
        Generic,
        Canon,
        Sony,
        iOS,
    }

    /// <summary>
    /// Configuration provider for ImageDownloader
    /// </summary>
    public class ConfigurationProvider
    {
        /// <summary>
        /// Provides default configuration per camera type
        /// </summary>
        /// <param name="cameraType">Camera type</param>
        /// <returns>Default configuration</returns>
        public Configuration ProvideDefaultConfiguration(CameraType cameraType= CameraType.Generic)
        {
            var configuration = new Configuration
            { 
                Destination = string.Empty,
                Pattern = string.Empty,
            };
            switch (cameraType)
            {
                case CameraType.Generic:
                    configuration.FileTypes = new FileTypes
                    {                    
                        VideoFileTypes = new string[0],
                        RawFileTypes = new string[0],
                        NonRawFileTypes = new string[0]
                    };
                    break;
                case CameraType.Canon:
                    configuration.FileTypes = ProvideDefaultFileTypesForCanon();
                    break;
                case CameraType.Sony:
                    configuration.FileTypes = ProvideDefaultFileTypesForSony();
                    break;
                case CameraType.iOS:
                    configuration.FileTypes = ProvideDefaultFileTypesForiOS();
                    break;
            }
            return configuration;
        }

        /// <summary>
        /// Creates a configuration from provided arguments
        /// </summary>
        /// <param name="rawTypes">Raw file types</param>
        /// <param name="nonRawTypes">Non-Raw (aka jpg) file types</param>
        /// <param name="videoTypes">Video file types</param>
        /// <param name="destination">Destination directory</param>
        /// <param name="pattern">Pattern to use</param>
        /// <returns>Configuration</returns>
        public Configuration InitializeFromParameters(IEnumerable<string> rawTypes, IEnumerable<string> nonRawTypes, IEnumerable<string> videoTypes, string destination, string pattern)
        {
            var destinationPath = string.IsNullOrEmpty(destination) ? string.Empty : destination;
            return new Configuration
            {
                Destination = System.IO.Path.IsPathRooted(destinationPath) ? destinationPath : System.IO.Path.Combine(System.Environment.CurrentDirectory, destinationPath),
                Pattern = string.IsNullOrEmpty(pattern) ? string.Empty : pattern,
                FileTypes = new FileTypes
                {
                    RawFileTypes = rawTypes?.ToArray() ?? new string[0],
                    NonRawFileTypes = nonRawTypes?.ToArray() ?? new string[0],
                    VideoFileTypes = videoTypes?.ToArray() ?? new string[0],
                }
            };
        }

        /// <summary>
        /// Gets configuration from a file on disk
        /// </summary>
        /// <param name="configurationFilePath">Path to a configuration file</param>
        /// <returns>Configuration</returns>
        public Configuration InitializeFromFile(string configurationFilePath)
        {
            Configuration configurationFromFile;
            try
            {
                    configurationFromFile = ReadConfigurationFromFile(configurationFilePath);
            }
            catch
            {
                configurationFromFile = ProvideDefaultConfiguration();
            }
            return configurationFromFile;
        }

        /// <summary>
        /// Wrapper for reading xml from disk
        /// </summary>
        /// <param name="filePath">Path to a configuration xml</param>
        /// <returns></returns>
        internal Configuration ReadConfigurationFromFile(string filePath)
        {
            Configuration configuration;
            var xmlReconstructionOutput = new XmlSerializer(typeof(Configuration));            
            using (var stringReader =  new StringReader(File.ReadAllText(filePath)))
            {
                configuration = (Configuration)xmlReconstructionOutput.Deserialize(stringReader);
            }
            return configuration;
        }

        /// <summary>
        /// Provides default file types for Canon cameras
        /// </summary>
        private FileTypes ProvideDefaultFileTypesForCanon()
        {
            return new FileTypes
            {
                NonRawFileTypes = new []{ ".jpg" },
                RawFileTypes = new [] { ".cr2", ".cr3"},
                VideoFileTypes = new [] { ".mov"}
            };
        }

        /// <summary>
        /// Provides default file types for Sony cameras
        /// </summary>
        private FileTypes ProvideDefaultFileTypesForSony()
        { 
            return new FileTypes
            {
                NonRawFileTypes = new []{ ".jpg" },
                RawFileTypes = new [] { ".arw"},
                VideoFileTypes = new [] { ".mov"}
            };
        }

        /// <summary>
        /// Provides default file types for iOS cameras (iPhone/iPod Touch/iPad)
        /// </summary>        
        private FileTypes ProvideDefaultFileTypesForiOS()
        { 
            return new FileTypes
            {
                NonRawFileTypes = new []{ ".jpg", ".heic" },
                RawFileTypes = new [] { ".tiff", ".dng"},
                VideoFileTypes = new [] { ".mov", ".hevc"}
            };
        }

    }
}
