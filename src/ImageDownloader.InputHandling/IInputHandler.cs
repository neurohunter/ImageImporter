using System;

namespace ImageDownloader.InputHandling
{
    public interface IInputHandler
    {
        object Parse(string[] args);
    }
}
