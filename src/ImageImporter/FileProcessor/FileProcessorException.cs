using System;
using System.Runtime.Serialization;

namespace ImageImporter.FileProcessor
{
    public class FileProcessorException : Exception
    {
        public FileProcessorException():base() { }
        
        public FileProcessorException(string message):base(message) { }

        public FileProcessorException(string message, Exception innerException) : base(message, innerException) { }

        public FileProcessorException(SerializationInfo info, StreamingContext context):base(info, context) { }

    }
}
