using System;
using System.Data.SQLite;

namespace USBss.Database
{
    public class LiteTable
    {
        public string Name { get; private set; }

        protected SQLiteConnection Connection {
            get
            {
                return LiteDatabase.singleton.Connection;
            }
        }

        public LiteTable(string name)
        {
            Name = name;
        }

        public virtual void Create()
        {

        }

        public bool Insert(string sql)
        {
            try
            {
                SQLiteCommand cmd_create = new SQLiteCommand(sql, Connection);
                cmd_create.ExecuteNonQuery();

                return true;
            }
            catch (Exception) { return false; }
        }

        public SQLiteDataReader Select(string sql)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(sql, Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                return reader;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
