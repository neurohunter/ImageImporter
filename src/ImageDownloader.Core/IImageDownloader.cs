using System;
using System.Collections.Generic;
using System.Text;

namespace ImageDownloader.Core
{
    public interface IImageDownloader
    {
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
        event EventHandler<ImportEventArgs> ImportDone;
    }
}
