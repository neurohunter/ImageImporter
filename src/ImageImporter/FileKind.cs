using System.ComponentModel;

namespace ImageImporter
{
    /// <summary>
    /// FIle metatype
    /// </summary>
    public enum FileKind
    {
        /// <summary>
        /// Unidentified
        /// </summary>
        [Description("MISC")] Unrecognized = 0,

        /// <summary>
        /// RAW image
        /// </summary>
        [Description("RAW")] RawImage = 1,

        /// <summary>
        /// JPG (non-RAW) image
        /// </summary>
        [Description("JPG")] JpegImage = 2,

        /// <summary>
        /// Video file
        /// </summary>
        [Description("VIDEO")] Video = 3,
    }
}