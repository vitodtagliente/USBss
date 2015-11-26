using System;

namespace WinUSB
{
    public delegate void UsbStateChangedEventHandler(UsbStateChangedEventArgs e);

    public class UsbStateChangedEventArgs : EventArgs
    {
        public UsbStateChangedEventArgs(UsbStateChange state, UsbDisk disk)
        {
            this.State = state;
            this.Disk = disk;
        }

        public UsbDisk Disk
        {
            get;
            private set;
        }

        public UsbStateChange State
        {
            get;
            private set;
        }
    }
}
