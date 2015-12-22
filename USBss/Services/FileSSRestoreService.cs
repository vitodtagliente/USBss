using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBss.Services
{
    /// <summary>
    /// Questa classe viene utilizzata sono nella modalità utente, ovvero quando il dispositivo USB non è proprio.
    /// Si occupa di gestire la password e l'agl del file, precedenti alla fase di decrypt
    /// </summary>
    public class FileSSRestoreService
    {
        public static readonly string DataDirectory = "Data";

        public string Filename { get; private set; }
        public string Name { get; private set; }
        public string DeviceName { get; private set; }

        public string Path { get; private set; }

        public string Key { get; private set; }

        public FileSSRestoreService(string deviceName, string filename)
        {
            DeviceName = deviceName.Replace(":", string.Empty);
            Name = filename;

            Path = DataDirectory + "/" + DeviceName;
            if (Directory.Exists(Path) == false)
                Directory.CreateDirectory(Path);

            Filename = Path + "/" + Name;
        }

        public void Rename(string filename)
        {
            Name = filename;
            // etc...
        }

        public void Store(byte[] aglBytes)
        {
            var stream = new FileStream(Filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            stream.Write(aglBytes, 0, aglBytes.Length);
            stream.Close();
            stream.Dispose();
        }

        public void SetKey(string key)
        {
            Key = key;
        }

        public byte[] Restore()
        {
            var stream = new FileStream(Filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            stream.Dispose();
            return bytes;
        }
    }
}
