using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace USBss.Database
{
    public class LiteDatabase
    {
        public static LiteDatabase singleton { get; private set; }

        public List<LiteTable> Tables { get; private set; }

        public string Filename { get; private set; }
        public SQLiteConnection Connection { get; private set; }
        
        public LiteDatabase(string filename)
        {
            if (singleton == null)
                singleton = this;

            Filename = filename;
            if (File.Exists(Filename) == false)
            {
                SQLiteConnection.CreateFile(Filename);
            }
            Tables = new List<LiteTable>();
        }

        public void Open()
        {
            Connection = new SQLiteConnection("Data Source=" + Filename + ";Version=3;");
            Connection.Open();

            foreach (var table in Tables)
                table.Create();
        }

        public LiteTable GetTable(string name)
        {
            foreach (var table in Tables)
                if (table.Name == name)
                    return table;
            return null;
        }

        public void Close()
        {
            Connection.Clone();
        }
    }
}
