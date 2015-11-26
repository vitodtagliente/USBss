using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBss.Services
{
    public class DeviceConfigurationService : ConfigurationService
    {
        public DeviceConfigurationService(string filename)
            : base(filename)
        {

        }

        public override void Configure()
        {
            base.Configure();
        }
    }
}
