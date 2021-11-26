using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[assembly:InternalsVisibleTo("ImageImporter.Test")]
namespace ImageImporter
{
    /// <summary>
    /// Configuration provider for ImageDownloader
    /// </summary>
    public class ConfigurationProvider
    {
        /// <summary>
        /// Provides default configuration per camera type
        /// </summary>
        /// <returns>Default configuration</returns>
        public Configuration Initialize()
        {
            return new Configuration
            { 
                Destination = System.Environment.CurrentDirectory,
                Pattern = string.Empty,
                FileTypes = new FileTypes
                    {                    
                        VideoFileTypes = System.Array.Empty<string>(),
                        RawFileTypes = System.Array.Empty<string>(),
                        NonRawFileTypes = System.Array.Empty<string>()
                },
            };
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
        public Configuration Initialize(IEnumerable<string> rawTypes, IEnumerable<string> nonRawTypes, IEnumerable<string> videoTypes, string destination, string pattern)
        {
            var destinationPath = string.IsNullOrEmpty(destination) ? string.Empty : destination;
            return new Configuration
            {
                Destination = Path.IsPathRooted(destinationPath) ? destinationPath : Path.Combine(System.Environment.CurrentDirectory, destinationPath),
                Pattern = string.IsNullOrEmpty(pattern) ? string.Empty : pattern,
                FileTypes = new FileTypes
                {
                    RawFileTypes = rawTypes?.ToArray() ?? System.Array.Empty<string>(),
                    NonRawFileTypes = nonRawTypes?.ToArray() ?? System.Array.Empty<string>(),
                    VideoFileTypes = videoTypes?.ToArray() ?? System.Array.Empty<string>(),
                }
            };
        }

        /// <summary>
        /// Gets configuration from a file on disk
        /// </summary>
        /// <param name="configurationFilePath">Path to a configuration file</param>
        /// <returns>Configuration</returns>
        public Configuration Initialize(string configurationFilePath)
        {
            Configuration configurationFromFile;
            try
            {
                configurationFromFile = ReadConfigurationFromFile(configurationFilePath);
                    
                if (string.IsNullOrEmpty(configurationFromFile.Destination))
                {
                    configurationFromFile.Destination = string.Empty;
                }
                if (configurationFromFile.FileTypes.NonRawFileTypes == null)
                {
                    configurationFromFile.FileTypes.NonRawFileTypes = System.Array.Empty<string>();
                }
                if (configurationFromFile.FileTypes.RawFileTypes == null)
                {
                    configurationFromFile.FileTypes.RawFileTypes = System.Array.Empty<string>();
                }
                if (configurationFromFile.FileTypes.VideoFileTypes == null)
                {
                    configurationFromFile.FileTypes.VideoFileTypes = System.Array.Empty<string>();
                }
                if (string.IsNullOrEmpty(configurationFromFile.Pattern))
                {
                    configurationFromFile.Pattern = string.Empty;
                }
            }
            catch
            {
                configurationFromFile = Initialize();
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
    }
}
