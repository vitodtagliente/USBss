using System.Collections.ObjectModel;
using System.Linq;

namespace WinUSB
{
    public class UsbDiskCollection : ObservableCollection<UsbDisk>
    {
        
        // Controlla se il disco specificato appartiene alla collezione.
        public bool Contains(string name)
        {
            return this.AsQueryable<UsbDisk>().Any(d => d.Name == name) == true;
        }

        // Rimuove il disco specificato dalla collezione
        public bool Remove(string name)
        {
            UsbDisk disk =
                (this.AsQueryable<UsbDisk>()
                .Where(d => d.Name == name)
                .Select(d => d)).FirstOrDefault<UsbDisk>();

            if (disk != null)
            {
                return this.Remove(disk);
            }

            return false;
        }
    }
}
