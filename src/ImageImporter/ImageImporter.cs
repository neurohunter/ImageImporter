using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageImporter.FileProcessor;

namespace ImageImporter
{
    public class ImageImporter : IImageImporter
    {
        private readonly FileProcessorFactory m_FileProcessorFactory;
        public Configuration Configuration {get; private set;}
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
            m_FileProcessorFactory = new FileProcessorFactory();
            m_ConfigurationProvider = new ConfigurationProvider();
            Configuration = m_ConfigurationProvider.Initialize();
        }
        
        /// <inheritdoc/>
        public void Initialize(string configurationFilePath)
        {
            Configuration = File.Exists(configurationFilePath) ?
                m_ConfigurationProvider.Initialize(configurationFilePath) :
                m_ConfigurationProvider.Initialize();        
        }        
        
        /// <inheritdoc/>
        public void Initialize(string configurationFilePath, string outputDirectory, IEnumerable<string> rawFiles, IEnumerable<string> nonRawFiles, IEnumerable<string> videoFiles, string pattern)
        {
            Initialize(configurationFilePath);
            if (!string.IsNullOrEmpty(outputDirectory))
            {
                Configuration.Destination = QualifyPath(outputDirectory);
            }
            if (rawFiles != null && rawFiles.Any())
            {
                Configuration.FileTypes.RawFileTypes = rawFiles.ToArray();
            }
            if (nonRawFiles != null && nonRawFiles.Any())
            {
                Configuration.FileTypes.NonRawFileTypes = nonRawFiles.ToArray();
            }
            if (videoFiles != null && videoFiles.Any())
            {
                Configuration.FileTypes.VideoFileTypes = videoFiles.ToArray();
            }
            if (!string.IsNullOrEmpty(pattern))
            {
                Configuration.Pattern = pattern;
            }
        }

        /// <inheritdoc/>
        public void Import(string inputDirectory)
        {
            if (!Directory.Exists(Configuration.Destination))
            {
                Directory.CreateDirectory(Configuration.Destination);
            }

            var filesToImport = Directory.GetFiles(inputDirectory).ToList();

            OnImportStarted(new ImportEventArgs(inputDirectory, Configuration.Destination, filesToImport.Count));
            foreach(var file in Directory.GetFiles(inputDirectory))
            {
                var fileInfo = new FileInfo(file);
                ProcessSingleFile(fileInfo, Configuration.Destination);
            }
            OnImportFinished(new ImportEventArgs(inputDirectory, Configuration.Destination, filesToImport.Count));
        }

        /// <summary>
        /// Creates a fully qualified path
        /// </summary>
        /// <param name="path">Original path</param>
        /// <returns>Fully qualified path created from original</returns>
        private string QualifyPath(string path)
        {
            return Path.IsPathRooted(path) ? path : Path.Combine(Environment.CurrentDirectory, path);
        }

        /// <summary>
        /// Processes a single file in an input directory
        /// </summary>
        /// <param name="inputFile">File to process</param>
        /// <param name="outputDirectory">Directory to put processed file to</param>
        private void ProcessSingleFile(FileInfo inputFile, string outputDirectory)
        {
            FileKind fileKind = ClassifyFile(inputFile.Extension.ToLower());
            string subDirectory = fileKind.GetAttributeOfType<DescriptionAttribute>().Description;
            string destinationPath = string.Empty;
            try
            {
                destinationPath = m_FileProcessorFactory.ProvideProcessorForFile(fileKind).Process(inputFile.FullName, fileKind, outputDirectory);
            }
            catch (FileProcessorException)
            {
                destinationPath = m_FileProcessorFactory.ProvideProcessorForFile(FileKind.Unrecognized).Process(inputFile.FullName, FileKind.Unrecognized, outputDirectory);
            }
            catch (Exception e)
            {
                OnFileFailed(new FileEventArgs(inputFile.Name, subDirectory, e.Message));
            }

            if (File.Exists(destinationPath))
            {
                OnFileFailed(new FileEventArgs(inputFile.Name, subDirectory, $"{inputFile.Name} already exists in {subDirectory}"));
            }
            else
            {
                File.Copy(inputFile.FullName, destinationPath, false);
                OnFileCopied(new FileEventArgs(inputFile.Name, subDirectory));
            }
        }

        /// <summary>
        /// Detects if a file is of a certain metatype
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>File metatype</returns>
        private FileKind ClassifyFile(string fileExtension)
        {
            if (Configuration.FileTypes.RawFileTypes.Contains(fileExtension))
            {
                return FileKind.RawImage;
            }
            if (Configuration.FileTypes.NonRawFileTypes.Contains(fileExtension))
            {
                return FileKind.JpegImage;
            }
            return Configuration.FileTypes.VideoFileTypes.Contains(fileExtension) ? FileKind.Video : FileKind.Unrecognized;
        }
    }
}
