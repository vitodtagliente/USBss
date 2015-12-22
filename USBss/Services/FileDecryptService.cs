using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace USBss.Services
{
    public class FileDecryptService : FileCryptoService
    {
        public string Key { get; private set; }

        public FileDecryptService(string filename)
            : base(filename)
        {
            Key = string.Empty;
        }
        
        public bool Decrypt(string key, bool ownermode = true)
        {
            string password = key;

            var aglService = new FileAGLService(Filename);
            string decryptoAGL = string.Empty;
            if (aglService.Access(key, out decryptoAGL))
            {
                if (decryptoAGL.Contains(FileAGLService.SecurityPhrase + ":"))
                    password = decryptoAGL.Replace(FileAGLService.SecurityPhrase + ":", string.Empty);
            } else return false;

            int aglSize = aglService.Size();

            var stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // Ottieni la lunghezza dell'header
            stream.Seek(0, SeekOrigin.Begin);
            byte[] headerSizeBytes = new byte[sizeof(int)];
            stream.Read(headerSizeBytes, 0, headerSizeBytes.Length);

            int headerSize = BitConverter.ToInt32(headerSizeBytes, 0);

            // leggi l'header
            var headerBytes = new byte[headerSize];
            stream.Read(headerBytes, 0, headerBytes.Length);

            // decripta l'header
            byte[] decrypto;

            try
            {
                decrypto = RijndaelService.DecryptBytes(headerBytes, password);
                Key = password;
            }
            catch (Exception) { return false; }

            // se è riuscito a decriptarlo, sostituisci il file con quello originale, rimuovendo anchel'AGL

            var bytes = new byte[stream.Length - sizeof(int) - headerBytes.Length - aglSize];
            stream.Read(bytes, 0, bytes.Length);

            stream.Close();
            stream.Dispose();

            // Ripristina il file

            stream = new FileStream(Filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            // All'inizio del blocco specifica il numero dei byte adibiti all'header section
            stream.Write(decrypto, 0, decrypto.Length);
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            stream.Dispose();
            
            return true;
        }
    }
}
