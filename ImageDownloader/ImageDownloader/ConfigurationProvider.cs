using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ImageDownloader.Configuration;

namespace ImageDownloader
{
    public enum CameraType
    {
        Generic,
        Canon,
        Sony,
        iOS,
    }

    public class ConfigurationProvider
    {
        public Configuration.Configuration ProvideDefaultConfiguration(CameraType cameraType= CameraType.Generic)
        {
            var configuration = new Configuration.Configuration
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
                        RawFileRypes = new string[0],
                        NonRawFileRypes = new string[0]
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

        public Configuration.Configuration InitializeFromFile(string configurationFilePath)
        {
            Configuration.Configuration configurationFromFile;
            try
                {
                    configurationFromFile = ReadConfigurationFromFile(configurationFilePath);
                }
            catch(Exception e)
            {
                configurationFromFile = ProvideDefaultConfiguration();
            }
            return configurationFromFile;
        }

        internal Configuration.Configuration ReadConfigurationFromFile(string filePath)
        {
            Configuration.Configuration configuration;
            var xmlReconstructionOutput = new XmlSerializer(typeof(Configuration.Configuration));            
            using (var stringReader =  new StringReader(File.ReadAllText(filePath)))
            {
                configuration = (Configuration.Configuration)xmlReconstructionOutput.Deserialize(stringReader);
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
                NonRawFileRypes = new []{ ".jpg" },
                RawFileRypes = new [] { ".cr2", ".cr3"},
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
                NonRawFileRypes = new []{ ".jpg" },
                RawFileRypes = new [] { ".arw"},
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
                NonRawFileRypes = new []{ ".jpg", ".heic" },
                RawFileRypes = new [] { ".tiff", ".dng"},
                VideoFileTypes = new [] { ".mov", ".hevc"}
            };
        }

    }
}
