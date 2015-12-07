using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace USBss.Services
{
    public abstract class FileCryptoService
    {
        public string Filename { get; private set; }

        public FileCryptoService(string filename)
        {
            Filename = filename;
        }
    }
}
