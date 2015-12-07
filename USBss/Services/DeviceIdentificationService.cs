using System;
using System.IO;

namespace USBss.Services
{
    public class DeviceIdentificationResult
    {
        public bool FileExists { get; internal set; }
        public string Id { get; internal set; }
        public bool EntryExists { get; internal set; }
        public bool Owner { get; internal set; }

        public bool Ok
        {
            get { return (FileExists && string.IsNullOrEmpty(Id) == false && EntryExists); }
        }
                
    }

    public class DeviceIdentificationService
    {
        public static string ConfigurationFile = "deviceid.ss";

        public string Path { get; private set; }

        public string Id { get; private set; }

        public string Filename
        {
            get { return Path + "/" + ConfigurationFile; }
        }

        public DeviceIdentificationService(string path)
        {
            Path = path;
            Id = string.Empty;
            if (Directory.Exists(Path) == false)
            {
                throw new Exception("Invalid Device Path: " + Path);
            }
        }

        public DeviceIdentificationResult Identify()
        {
            var result = new DeviceIdentificationResult();
            result.FileExists = false;
            result.EntryExists = false;
            result.Id = string.Empty;
            result.Owner = false;
            
            if(File.Exists(Filename))
            {
                result.FileExists = true;

                StreamReader rd = new StreamReader(Filename);
                string file_content = rd.ReadLine();
                rd.Close();
                rd.Dispose();

                if (file_content.ToLower().StartsWith("deviceid:"))
                {
                    var pieces = file_content.Split(':');
                    if (pieces.Length == 2)
                    {
                        result.Id = pieces[1];
                    }
                }

                var table = Database.LiteDatabase.singleton.GetTable("devices");
                if(table != null && ((Database.Tables.Devices)table).Exists(result.Id))
                {
                    result.EntryExists = true;

                    result.Owner = ((Database.Tables.Devices)table).ImOwner("devices");
                }
            }      

            return result;
        }
    }
}
