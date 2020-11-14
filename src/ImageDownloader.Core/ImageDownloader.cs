using System;

namespace ImageDownloader.Core
{
    public class ImageDownloader : IImageDownloader
    {
        public ImageDownloader(object arguments)
        {

        }

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
        public event EventHandler<ImportEventArgs> ImportDone;
        private void OnImportDone(ImportEventArgs e)
        {
            ImportDone?.Invoke(this, e);
        } 
        #endregion
    }
}
