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

        public string Key { get; private set; }

        public FileSSRestoreService(string filename)
        {
            Filename = filename;
        }

        public void Rename(string filename)
        {
            Filename = filename;
            // etc...
        }

        public void Store()
        {

        }

        public bool Restore(string deviceName, string key)
        {
            deviceName = deviceName.Replace(":", string.Empty);
            var path = DataDirectory + "/" + deviceName;
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            var aglService = new FileAGLService(Filename);
            if (aglService.Exists() == false)
                return false;

            string decryptoAGL = string.Empty;
            string password = key;
            if (aglService.Access(key, out decryptoAGL))
            {
                if (decryptoAGL.Contains(FileAGLService.SecurityPhrase + ":"))
                    password = decryptoAGL.Replace(FileAGLService.SecurityPhrase + ":", string.Empty);
            }
            else return false;



            return true;
        }
    }
}
