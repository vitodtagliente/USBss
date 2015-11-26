using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Numeria.IO;

namespace USBss.Services
{
    public class AppConfigurationService : ConfigurationService
    {


        public AppConfigurationService(string filename)
            : base(filename)
        {

        }

        public override void Configure()
        {
            base.Configure();
        }
    }
}
