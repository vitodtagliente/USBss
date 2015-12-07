namespace USBss.Database.Tables
{
    public class Devices : LiteTable
    {
        public Devices()
            : base("devices")
        {
            
        }

        public override void Create()
        {
            // owner: 0 = false, 1 = true
            bool result = Insert("create table if not exists devices (id INTEGER PRIMARY KEY, name TEXT UNIQUE, owner INTEGER )");
        }

        public bool Exists(string name)
        {
            var reader = Select("select * from devices where name = '" + name + "'");
            if(reader != null && reader.Read())
            {
                return true;
            }
            return false;
        }

        public bool ImOwner(string name)
        {
            var reader = Select("select * from devices where name = '" + name + "'");
            if (reader !=null && reader.Read())
            {
                string owner = reader["owner"].ToString();
                if (owner.Contains("1"))
                    return true;
            }
            return false;
        }
    }
}
