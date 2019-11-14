﻿using System;
using System.ComponentModel;
using System.IO;

namespace ImageImporter.FileProcessor
{
    /// <inheritdoc />
    public class GenericFileProcessor:FileProcessor
    {
        /// <inheritdoc />
        public override string Process(FileInfo inputFile, FileKind fileKind, string outputDirectory)
        {
            return CreateDestinationPath(outputDirectory, DateTime.Now.Date.ToString("yyyy_MM_dd"), fileKind.GetAttributeOfType<DescriptionAttribute>().Description, inputFile.Name);
        }
    }
}
