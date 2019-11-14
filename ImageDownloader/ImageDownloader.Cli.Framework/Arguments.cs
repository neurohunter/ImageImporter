using System.Collections.Generic;

namespace ImageImporter.Cli.Framework
{
    internal class Arguments
    {
        public string InputDirectory { set; get; }
        public string OutputDirectory { set; get; }
        public string ConfigurationPath {set;get;}
        public List<string> RawFileExtensions{get;set;}
        public List<string> NonRawFileExtensions {get;set;}
        public List<string> VideoFileExtensions { get;set;}

        public Arguments()
        {
            RawFileExtensions = new List<string>();
            NonRawFileExtensions = new List<string>();
            VideoFileExtensions = new List<string>();
        }
    }
}
