using System;
using System.Management;

namespace WinUSB
{
    public class UsbManager : IDisposable
    {
        private delegate void GetDiskInformationDelegate(UsbDisk disk);

        private WindowDriver window;
        private UsbStateChangedEventHandler handler;
        private bool isDisposed;
                
        public UsbManager()
        {
            this.window = null;
            this.handler = null;
            this.isDisposed = false;
        }
        
        ~UsbManager()
        {
            Dispose();
        }
        
        public void Dispose()
        {
            if (!isDisposed)
            {
                if (window != null)
                {
                    window.StateChanged -= new UsbStateChangedEventHandler(DoStateChanged);
                    window.Dispose();
                    window = null;
                }

                isDisposed = true;

                GC.SuppressFinalize(this);
            }
        }
        
        public event UsbStateChangedEventHandler StateChanged
        {
            add
            {
                if (window == null)
                {
                    // create the driver window once a consumer registers for notifications
                    window = new WindowDriver();
                    window.StateChanged += new UsbStateChangedEventHandler(DoStateChanged);
                }

                handler = (UsbStateChangedEventHandler)Delegate.Combine(handler, value);
            }

            remove
            {
                handler = (UsbStateChangedEventHandler)Delegate.Remove(handler, value);

                if (handler == null)
                {
                    // destroy the driver window once the consumer stops listening
                    window.StateChanged -= new UsbStateChangedEventHandler(DoStateChanged);
                    window.Dispose();
                    window = null;
                }
            }
        }

        public UsbDiskCollection GetAvailableDisks()
        {
            UsbDiskCollection disks = new UsbDiskCollection();

            // browse all USB WMI physical disks
            foreach (ManagementObject drive in
                new ManagementObjectSearcher(
                    "select DeviceID, Model from Win32_DiskDrive where InterfaceType='USB'").Get())
            {
                // associate physical disks with partitions
                ManagementObject partition = new ManagementObjectSearcher(String.Format(
                    "associators of {{Win32_DiskDrive.DeviceID='{0}'}} where AssocClass = Win32_DiskDriveToDiskPartition",
                    drive["DeviceID"])).First();

                if (partition != null)
                {
                    // associate partitions with logical disks (drive letter volumes)
                    ManagementObject logical = new ManagementObjectSearcher(String.Format(
                        "associators of {{Win32_DiskPartition.DeviceID='{0}'}} where AssocClass = Win32_LogicalDiskToPartition",
                        partition["DeviceID"])).First();

                    if (logical != null)
                    {
                        // finally find the logical disk entry to determine the volume name
                        ManagementObject volume = new ManagementObjectSearcher(String.Format(
                            "select FreeSpace, Size, VolumeName from Win32_LogicalDisk where Name='{0}'",
                            logical["Name"])).First();

                        UsbDisk disk = new UsbDisk(logical["Name"].ToString());
                        disk.Model = drive["Model"].ToString();
                        disk.Volume = volume["VolumeName"].ToString();
                        disk.FreeSpace = (ulong)volume["FreeSpace"];
                        disk.Size = (ulong)volume["Size"];

                        disks.Add(disk);
                    }
                }
            }

            return disks;
        }


        /// <summary>
        /// Internally handle state changes and notify listeners.
        /// </summary>
        /// <param name="e"></param>

        private void DoStateChanged(UsbStateChangedEventArgs e)
        {
            if (handler != null)
            {
                UsbDisk disk = e.Disk;

                // we can only interrogate drives that are added...
                // cannot see something that is no longer there!

                if ((e.State == UsbStateChange.Added) && (e.Disk.Name[0] != '?'))
                {
                    // the following Begin/End invokes looks strange but are required
                    // to resolve a "DisconnectedContext was detected" exception which
                    // occurs when the current thread terminates before the WMI queries
                    // can complete.  I'm not exactly sure why that would happen...

                    GetDiskInformationDelegate gdi = new GetDiskInformationDelegate(GetDiskInformation);
                    IAsyncResult result = gdi.BeginInvoke(e.Disk, null, null);
                    gdi.EndInvoke(result);
                }

                handler(e);
            }
        }


        /// <summary>
        /// Populate the missing properties of the given disk before sending to listeners
        /// </summary>
        /// <param name="disk"></param>

        private void GetDiskInformation(UsbDisk disk)
        {
            ManagementObject partition = new ManagementObjectSearcher(String.Format(
                "associators of {{Win32_LogicalDisk.DeviceID='{0}'}} where AssocClass = Win32_LogicalDiskToPartition",
                disk.Name)).First();

            if (partition != null)
            {
                ManagementObject drive = new ManagementObjectSearcher(String.Format(
                    "associators of {{Win32_DiskPartition.DeviceID='{0}'}}  where resultClass = Win32_DiskDrive",
                    partition["DeviceID"])).First();

                if (drive != null)
                {
                    disk.Model = drive["Model"].ToString();
                }

                ManagementObject volume = new ManagementObjectSearcher(String.Format(
                    "select FreeSpace, Size, VolumeName from Win32_LogicalDisk where Name='{0}'",
                    disk.Name)).First();

                if (volume != null)
                {
                    disk.Volume = volume["VolumeName"].ToString();
                    disk.FreeSpace = (ulong)volume["FreeSpace"];
                    disk.Size = (ulong)volume["Size"];
                }
            }
        }
    }
}
