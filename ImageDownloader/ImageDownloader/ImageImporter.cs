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
            m_FileProcessorFactory = new FileProcessorFactory();
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
        /// <param name="inputFile">File to process</param>
        /// <param name="outputDirectory">Directory to put processed file to</param>
        private void ProcessSingleFile(FileInfo inputFile, string outputDirectory)
        {
            FileKind fileKind = ClassifyFile(inputFile.Extension.ToLower());
            string subDirectory = fileKind.GetAttributeOfType<DescriptionAttribute>().Description;
            string destinationPath = string.Empty;
            try
            {
                destinationPath = m_FileProcessorFactory.ProvideProcessorForFile(fileKind).Process(inputFile, fileKind, outputDirectory);
            }
            catch (FileProcessorException)
            {
                destinationPath = m_FileProcessorFactory.ProvideProcessorForFile(FileKind.Unrecognized).Process(inputFile, FileKind.Unrecognized, outputDirectory);
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
    }
}
