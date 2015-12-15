using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace USBss.Services
{
    public class FileAGLService // Access Group List
    {
        public string Filename { get; private set; }

        static readonly string BlockIdString = "ssAGL";

        private const string initVector = "USBssAGLService0"; // deve essere di 16 caratteri
        // This constant is used to determine the keysize of the encryption algorithm
        private const int keysize = 256;

        public static readonly string SecurityPhrase = "ssCanDecrypt";

        public FileAGLService(string filename)
        {
            Filename = filename;
        }

        /// <summary>
        /// Struttura AGL:
        /// blocchi costituiti da -> size(crypt(testo, key)) + crypt(testo, key)
        /// + size(AGLblocks) + securityIdBlock
        /// </summary>
        /// <param name="data"></param>
        
        public void Write(string phrase, List<string> keys)
        {
            List<byte> AGLbytes = new List<byte>();
            foreach (var key in keys)
            {
                var enc = RijndaelService.Encrypt(phrase, key);
                AGLbytes.AddRange(BitConverter.GetBytes(enc.Length));
                AGLbytes.AddRange(enc);
            }

            var ssBytes = Encoding.UTF8.GetBytes(BlockIdString);

            AGLbytes.AddRange(BitConverter.GetBytes(AGLbytes.Count + sizeof(int) + ssBytes.Length));
            AGLbytes.AddRange(ssBytes);

            var bytesArr = AGLbytes.ToArray();

            var stream = new FileStream(Filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            stream.Write(bytesArr, 0, bytesArr.Length);
            stream.Close();
            stream.Dispose();
        }

        public bool Exists()
        {
            var result = true;
            FileStream stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var ssBytes = Encoding.UTF8.GetBytes(BlockIdString);
            // il file è piu piccolo della stringa di riconoscimento
            if (ssBytes.Length > stream.Length)
                return false;
            stream.Seek(-ssBytes.Length, SeekOrigin.End);
            var ss = new byte[ssBytes.Length];
            stream.Read(ss, 0, ssBytes.Length);
            if (Encoding.UTF8.GetString(ssBytes) != Encoding.UTF8.GetString(ss))
                result = false;
            stream.Close();
            stream.Dispose();
            return result;
        }
        
        public int Size()
        {
            if (!Exists())
                return -1;

            FileStream stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var ssBytes = Encoding.UTF8.GetBytes(BlockIdString);
            stream.Seek(-sizeof(int) - ssBytes.Length, SeekOrigin.End);
            byte[] aglSizeBytes = new byte[sizeof(int)];
            stream.Read(aglSizeBytes, 0, sizeof(int));

            int size = BitConverter.ToInt32(aglSizeBytes, 0);

            stream.Close();
            stream.Dispose();
            return size;
        }

        public bool Access(string password, out string decryptedText)
        {
            decryptedText = string.Empty;

            if (!Exists()) return false;

            int aglSize = Size();

            var stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            /*            
            // verifica che nel file ci sia l'AGL
            var ssBytes = Encoding.UTF8.GetBytes(BlockIdString);
            stream.Seek(-ssBytes.Length, SeekOrigin.End);
            var ss = new byte[ssBytes.Length];
            stream.Read(ss, 0, ssBytes.Length);
            if (Encoding.UTF8.GetString(ssBytes) != Encoding.UTF8.GetString(ss))
                return false;

            // Ottieni la lunghezza dell'AGL
            stream.Seek(-sizeof(int) - ssBytes.Length, SeekOrigin.End);
            byte[] aglSizeBytes = new byte[sizeof(int)];
            stream.Read(aglSizeBytes, 0, sizeof(int));
            
            int aglSize = BitConverter.ToInt32(aglSizeBytes, 0);
            */
            var current = stream.Seek(-aglSize, SeekOrigin.End);

            int amount = 1;

            while (amount != 0 || stream.Position != stream.Length)
            {
                var lengthBytes = new byte[sizeof(int)];
                stream.Read(lengthBytes, 0, sizeof(int));
                int length = BitConverter.ToInt32(lengthBytes, 0);

                var data = new byte[length];
                amount = stream.Read(data, 0, length);
                try
                {
                    decryptedText = RijndaelService.Decrypt(data, password);

                    return true;
                }
                catch (Exception)
                {

                }
            }

            return false;
        }

        public byte[] GetBytes()
        {
            if (!Exists()) return null;

            int aglSize = Size();

            var stream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var bytes = new byte[aglSize];
            stream.Seek(-aglSize, SeekOrigin.End);

            stream.Read(bytes, 0, bytes.Length);

            stream.Close();
            stream.Dispose();

            return bytes;
        }
        
    }
}
