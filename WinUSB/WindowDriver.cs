using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinUSB
{
    // La classe NativeWindow 
    // fornisce un'inclusione di basso livello di un handle di finestra e di una routine di finestra.

    internal class WindowDriver : NativeWindow, IDisposable
    {
        // Contiene informazioni riguardanti un Volume logico, definito in Win32.
        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_VOLUME
        {
            public int dbcv_size;           // size of the struct
            public int dbcv_devicetype;     // DBT_DEVTYP_VOLUME
            public int dbcv_reserved;       // reserved; do not use
            public int dbcv_unitmask;       // Bit 0=A, bit 1=B, and so on (bitmask)
            public short dbcv_flags;        // DBTF_MEDIA=0x01, DBTF_NET=0x02 (bitmask)
        }


        private const int WM_DEVICECHANGE = 0x0219;             // device state change
        private const int DBT_DEVICEARRIVAL = 0x8000;           // detected a new device
        private const int DBT_DEVICEQUERYREMOVE = 0x8001;       // preparing to remove
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;    // removed 
        private const int DBT_DEVTYP_VOLUME = 0x00000002;       // logical volume


        public WindowDriver()
        {
            // Crea una finestra generica
            base.CreateHandle(new CreateParams());
        }


        public void Dispose()
        {
            base.DestroyHandle();
            GC.SuppressFinalize(this);
        }


        public event UsbStateChangedEventHandler StateChanged;

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if ((message.Msg == WM_DEVICECHANGE) && (message.LParam != IntPtr.Zero))
            {
                DEV_BROADCAST_VOLUME volume = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(
                    message.LParam, typeof(DEV_BROADCAST_VOLUME));

                if (volume.dbcv_devicetype == DBT_DEVTYP_VOLUME)
                {
                    switch (message.WParam.ToInt32())
                    {
                        case DBT_DEVICEARRIVAL:
                            SignalDeviceChange(UsbStateChange.Added, volume);
                            break;

                        case DBT_DEVICEQUERYREMOVE:
                            SignalDeviceChange(UsbStateChange.Removing, volume);
                            break;

                        case DBT_DEVICEREMOVECOMPLETE:
                            SignalDeviceChange(UsbStateChange.Removed, volume);
                            break;
                    }
                }
            }
        }

        // segnala il cambio bi stato del dispositivo 
        void SignalDeviceChange(UsbStateChange state, DEV_BROADCAST_VOLUME volume)
        {
            string name = ToUnitName(volume.dbcv_unitmask);

            if (StateChanged != null)
            {
                UsbDisk disk = new UsbDisk(name);
                StateChanged(new UsbStateChangedEventArgs(state, disk));
            }
        }

        // Traduci il dbcv_unitmask bitmask nella lettera di sistema assegnata al dispositivo
        string ToUnitName(int mask)
        {
            int offset = 0;
            while ((offset < 26) && ((mask & 0x00000001) == 0))
            {
                mask = mask >> 1;
                offset++;
            }

            if (offset < 26)
            {
                return String.Format("{0}:", Convert.ToChar(Convert.ToInt32('A') + offset));
            }

            return "?:";
        }
    }
}
