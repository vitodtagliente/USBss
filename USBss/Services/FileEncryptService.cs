﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace USBss.Services
{
    public class FileEncryptService : FileCryptoService
    {
        public FileEncryptService(string filename)
            : base(filename)
        {

        }
    }
}