using System;
using System.ComponentModel;

namespace ImageImporter.Test.ImageImporter
{
    public class ImageImporterTestFileDescription
    {
        public string FileName{get;}

        public string Extension => FileName.Substring(FileName.LastIndexOf('.'));

        public DateTime DateDigitized {get;}

        public DateTime DateCreated {get;}

        public FileKind FileKind {get;}

        public ImageImporterTestFileDescription(string filename, DateTime dateDigitized, DateTime dateCreated, FileKind fileKind)
        {
            FileName = filename;
            DateDigitized = dateDigitized;
            DateCreated = dateCreated;
            FileKind = fileKind;
        }

        public string GetExpectedPath(FileKind requiredFileKind)
        {
            var path = System.IO.Path.Combine(
                (requiredFileKind == FileKind.JpegImage || requiredFileKind == FileKind.RawImage ? 
                DateDigitized.Date.ToString("yyyy_MM_dd") :
                DateCreated.Date.ToString("yyyy_MM_dd")),
                requiredFileKind.GetAttributeOfType<DescriptionAttribute>().Description,
                FileName
                );
            return path; 
        }
    }
}
