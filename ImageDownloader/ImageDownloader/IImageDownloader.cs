namespace ImageDownloader
{
    public interface IImageDownloader
    {
        void Download(string inputDirectory, string outputDirectory);
    }
}
