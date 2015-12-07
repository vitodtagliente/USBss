using System;
using System.IO;
using System.Text;

namespace USBss.Services
{
    public class DeviceSetupService
    {
        string Filename;
        public string Id { get; private set; }

        public DeviceSetupService(string filename)
        {
            Filename = filename;
        }

        /// <summary>
        /// Questo metodo viene usato per installare il modulo ss sul dispositivo
        /// in modalità propretario
        /// </summary>
        /// <returns></returns>
        public bool Setup()
        {
            Id = GenerateId();
            return Setup(Id, true);
        }

        /// <summary>
        /// Questo metodo viene usato per identificare e registrare il dispositivo nel database
        /// </summary>
        /// <param name="id">Identificativo del dispositivo</param>
        /// <returns></returns>
        public bool Setup(string id, bool owner = false)
        {
            Id = id;
            var table = Database.LiteDatabase.singleton.GetTable("devices");
            if (table != null)
            {
                Database.Tables.Devices devices = table as Database.Tables.Devices;
                var result = devices.Insert("Insert into devices (id, name, owner) VALUES ( NULL, '" + 
                    id + "', '" + ((owner)?"1":"0") + "')");
                if (result)
                {
                    if(owner)
                    {
                        StreamWriter wr = new StreamWriter(Filename);
                        wr.WriteLine("DeviceId:" + id);
                        wr.Close();
                        wr.Dispose();

                        File.SetAttributes(Filename, FileAttributes.Hidden);
                    }
                    
                    return true;
                }
            }

            return false;
        }

        string GenerateId()
        {
            StringBuilder str = new StringBuilder();
            var now = DateTime.Now;
            str.Append(string.Format("d{0}{1}{2}", now.Day, now.Month, now.Year));
            str.Append(string.Format("t{0}{1}{2}", now.Hour, now.Minute, now.Second));
            Random random = new Random();
            str.Append("i");
            str.Append(random.Next(0, 1000).ToString());
            return str.ToString();
        }
    }
}
