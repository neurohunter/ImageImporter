using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using MetadataExtractor;
using System.Linq;
using MetadataExtractor.Formats.Exif;

namespace ImageImporter
{
    /// <summary>
    /// FIle metatype
    /// </summary>
    public enum FileKind
    {
        /// <summary>
        /// Unidentified
        /// </summary>
        [Description("MISC")]
        Unrecognized = 0,
        /// <summary>
        /// RAW image
        /// </summary>
        [Description("RAW")]
        RawImage = 1,
        /// <summary>
        /// JPG (non-RAW) image
        /// </summary>
        [Description("JPG")]
        JpegImage = 2,
        /// <summary>
        /// Video file
        /// </summary>
        [Description("VIDEO")]
        Video = 3
    }

    public class ImportEventArgs : EventArgs
    {
        public ImportEventArgs(string sourceDirectory, string destinationDirectory, int numberOfFiles)
        {
            SourceDirectory = sourceDirectory;
            DestinationDirectory = destinationDirectory;
            NumberOfFiles = numberOfFiles;
        }

        public string SourceDirectory {get;}
        public string DestinationDirectory {get;}
        public int NumberOfFiles {get;}
    }

    public class FileEventArgs : EventArgs
    {
        public FileEventArgs(string filename, string subDirectory, string errorMessage)
        {
            Filename = filename;
            SubDirectory = subDirectory;
            ErrorMessage = errorMessage;
        }

        public FileEventArgs(string filename, string subDirectory):this(filename, subDirectory, string.Empty){ }

        public string Filename {get;}
        public string SubDirectory {get;}
        public string ErrorMessage {get;}
    }


    public class ImageImporter : IImageImporter
    {
        Configuration m_Configuration;
        private readonly ConfigurationProvider m_ConfigurationProvider;

        #region Events
        public event EventHandler<ImportEventArgs> ImportStarted;

        private void OnImportStarted(ImportEventArgs e)
        {
            ImportStarted?.Invoke(this, e);
        }

        public event EventHandler<FileEventArgs> FileCopied;

        private void OnFileCopied(FileEventArgs e)
        {
            FileCopied?.Invoke(this, e);
        }

        public event EventHandler<FileEventArgs> FileFailed;

        private void OnFileFailed(FileEventArgs e)
        {
            FileFailed?.Invoke(this, e);
        }

        public event EventHandler<ImportEventArgs> ImportFinished;

        public void OnImportFinished(ImportEventArgs e)
        {
            ImportFinished?.Invoke(this, e);
        }
        #endregion

        public ImageImporter()
        { 
            m_ConfigurationProvider = new ConfigurationProvider();
            m_Configuration = m_ConfigurationProvider.ProvideDefaultConfiguration();
            
        }
        
        /// <inheritdoc/>
        public void Initialize(string configurationFilePath)
        {
            m_Configuration = m_ConfigurationProvider.InitializeFromFile(configurationFilePath);
        }
        
        /// <inheritdoc/>
        public void Import(string inputDrirectory)
        {
            var outputDirectory = !string.IsNullOrEmpty(m_Configuration.Destination) ? m_Configuration.Destination : Environment.CurrentDirectory;
            Import(inputDrirectory, outputDirectory, m_Configuration.FileTypes.RawFileRypes, m_Configuration.FileTypes.NonRawFileRypes, m_Configuration.FileTypes.VideoFileTypes, string.Empty);
        }

        /// <inheritdoc/>
        public void Import(string inputDirectory, string outputDirectory)
        {
            Import(inputDirectory, outputDirectory, m_Configuration.FileTypes.RawFileRypes, m_Configuration.FileTypes.NonRawFileRypes, m_Configuration.FileTypes.VideoFileTypes, string.Empty);
        }

        /// <inheritdoc/>
        public void Import(string inputDirectory, string outputDirectory, IEnumerable<string> rawFiles, IEnumerable<string> nonRawFiles, IEnumerable<string> videoFiles, string pattern)
        {
            m_Configuration.FileTypes.RawFileRypes = rawFiles.ToArray();
            m_Configuration.FileTypes.NonRawFileRypes = nonRawFiles.ToArray();
            m_Configuration.FileTypes.VideoFileTypes = videoFiles.ToArray();
            if (!System.IO.Directory.Exists(outputDirectory))
            {
                System.IO.Directory.CreateDirectory(outputDirectory);
            }

            var filesToImport = System.IO.Directory.GetFiles(inputDirectory).ToList();

            OnImportStarted(new ImportEventArgs(inputDirectory, outputDirectory, filesToImport.Count));
            foreach(var file in System.IO.Directory.GetFiles(inputDirectory))
            {
                var fileInfo = new FileInfo(file);
                ProcessSingleFile(fileInfo, outputDirectory);
            }
            OnImportFinished(new ImportEventArgs(inputDirectory, outputDirectory, filesToImport.Count));
        }

        /// <summary>
        /// Processes a single file in an input directory
        /// </summary>
        /// <param name="fileInfo">File to process</param>
        /// <param name="outputDirectory">Directory to put processed file to</param>
        private void ProcessSingleFile(FileInfo fileInfo, string outputDirectory)
        {
            var subDirectory = string.Empty;
            var dateTimeTaken = DateTime.Now;
            try
            {
                var metadataDirectories = ImageMetadataReader.ReadMetadata(fileInfo.FullName);
                var exifTagDirectory = metadataDirectories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                if (exifTagDirectory != null)
                {
                    dateTimeTaken = exifTagDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeDigitized, out var dateTime) ? dateTime : dateTimeTaken;
                    var fileClass = ClassifyFile(fileInfo.Extension.ToLower());
                    subDirectory = fileClass.GetAttributeOfType<DescriptionAttribute>().Description;
                }
            }
            catch (Exception e)
            {
                subDirectory = FileKind.Unrecognized.GetAttributeOfType<DescriptionAttribute>().Description;
                OnFileFailed(new FileEventArgs(fileInfo.Name, subDirectory, e.Message));
            }
            var destinationPath = CreateDestinationPath(outputDirectory, dateTimeTaken, subDirectory, fileInfo.Name);
            if (File.Exists(destinationPath))
            {
                OnFileFailed(new FileEventArgs(fileInfo.Name, subDirectory, $"{fileInfo.Name} already exists in {subDirectory}"));
            }
            else
            {
                File.Copy(fileInfo.FullName, destinationPath, false);
                OnFileCopied(new FileEventArgs(fileInfo.Name, subDirectory));
            }
        }

        /// <summary>
        /// Detects if a file is of a certain metatype
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>File metatype</returns>
        private FileKind ClassifyFile(string fileExtension)
        {
            if (m_Configuration.FileTypes.RawFileRypes.Contains(fileExtension))
            {
                return FileKind.RawImage;
            }
            if (m_Configuration.FileTypes.NonRawFileRypes.Contains(fileExtension))
            {
                return FileKind.JpegImage;
            }
            return m_Configuration.FileTypes.VideoFileTypes.Contains(fileExtension) ? FileKind.Video : FileKind.Unrecognized;
        }

        private string CreateDestinationPath(string outputDirectory, DateTime dateTimeTaken, string subDirectory, string fileName)
        {
            var dateAsString = dateTimeTaken.Date.ToString("yyyy_MM_dd");
            var directory = Path.Combine(outputDirectory, dateAsString, subDirectory);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            return Path.Combine(directory, fileName);
        }
    }
}
