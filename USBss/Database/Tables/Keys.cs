using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace USBss.Database.Tables
{
    public class Keys : LiteTable
    {
        public Keys()
            : base("keys")
        {

        }

        public override void Create()
        {
            bool result = Insert(
                "create table if not exists keys (id INTEGER PRIMARY KEY, deviceId INTEGER NOT NULL, group TEXT NOT NULL, key TEXT NOT NULL)"
                );
        }

        public List<string> GetGroups(string deviceId)
        {
            var list = new List<string>();
            foreach (var group in Get(deviceId).Keys)
                list.Add(group);
            return list;
        }

        public Dictionary<string, string> Get(string deviceId)
        {
            Dictionary<string, string> fetch = new Dictionary<string, string>();
            var reader = Select("select * from keys where deviceId = '" + deviceId + "'");
            while (reader != null && reader.Read())
            {
                fetch.Add(reader["group"].ToString(), reader["key"].ToString());
            }
            return fetch;
        }
    }
}
