using System.IO;
using Numeria.IO;

namespace USBss.Services
{
    public abstract class ConfigurationService
    {
        public string Filename { get; protected set; }

        public bool HiddenFile { get; set; }

        public ConfigurationService(string filename)
        {
            Filename = filename;

            HiddenFile = true;
        }

        public virtual void Configure()
        {
            if (File.Exists(Filename) && HiddenFile)
            {
                File.SetAttributes(Filename, FileAttributes.Hidden);
            }
            else
                FileDB.CreateEmptyFile(Filename, false);
        }
    }
}
