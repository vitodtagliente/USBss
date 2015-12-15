using System;
using System.Collections.Generic;
using System.IO;

namespace USBss.Services
{
    public class FileEncryptService : FileCryptoService
    {
        public FileEncryptService(string filename)
            : base(filename)
        {

        }

        public void Encrypt(string key)
        {
            
        }

        /// <summary>
        /// Ricordiamoci che l'accesso deve essere gerarchico
        /// un può essere acceduto con piu di un una chiave
        /// </summary>
        /// <param name="keys"></param>
        public void Encrypt(List<string> keys)
        {
            if (keys.Count == 0)
                return;

            // Non fare niente se il file presenta già un protocollo di sicurezza
            var aglService = new FileAGLService(Filename);
            if (aglService.Exists() == true)
                return;

            string currentPassword = string.Empty;

            foreach (var key in keys)
                currentPassword += key;

            var stream = new FileStream(Filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            // decido quanti byte di inizio file criptare
            // ovviamente devo tener conto della dimensione del file
            int headerSize = 254;
            if (headerSize > stream.Length)
                headerSize = (int)stream.Length / 4;

            var firstBytes = new byte[headerSize];
            stream.Read(firstBytes, 0, firstBytes.Length);

            // crittografa il primo blocco di N bytes
            var crypto = RijndaelService.EncryptBytes(firstBytes, currentPassword);

            // leggi tutto il contenuto restante
            byte[] bytes = new byte[stream.Length - firstBytes.Length];
            stream.Read(bytes, 0, bytes.Length);

            stream.Close();
            stream.Dispose();

            // LA situazione ora è definita da
            // crypto: primi N bytes criptati
            // bytes: i restanti bytes del file, normali

            stream = new FileStream(Filename, FileMode.Create);
            // All'inizio del blocco specifica il numero dei byte adibiti all'header section
            stream.Write(BitConverter.GetBytes(crypto.Length), 0, sizeof(int));
            stream.Write(crypto, 0, crypto.Length);
            stream.Write(bytes, 0, bytes.Length);            
            stream.Close();
            stream.Dispose();

            // Ora resta da scrivere, sul file, la parte riguardante l'AGL
            string securityPhrase = FileAGLService.SecurityPhrase;
            if (keys.Count > 1)
                securityPhrase += ":" + currentPassword;

            aglService = new FileAGLService(Filename);
            aglService.Write(securityPhrase, keys);
        }
    }
}
