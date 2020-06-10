using System.ComponentModel;

namespace PhotoCleaner.App.Domain
{
    public enum FileExtension
    {
        [Description("JPEG files | *.jpg")]
        JPEG,
        [Description("RAW files | *.NEF")]
        RAW
    }
}
