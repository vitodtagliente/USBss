using System.Management;

namespace WinUSB
{
    public static class WmiExtensions
    {
        // Utile per il fetch del primo elemento della collezione.
        public static ManagementObject First(this ManagementObjectSearcher searcher)
        {
            ManagementObject result = null;
            foreach (ManagementObject item in searcher.Get())
            {
                result = item;
                break;
            }
            return result;
        }
    }
}
