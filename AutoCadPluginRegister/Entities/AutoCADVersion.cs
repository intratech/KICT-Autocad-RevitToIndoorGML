using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCadPluginRegister.Entities
{
    class AutoCADVersion
    {
        public string Version { get; set; }
        public string VersionCode { get; set; }
        public string Display { get; set; }
        public AutoCADVersion()
        {

        }
        public AutoCADVersion(string version, string code, string display)
        {
            this.Version = version;
            this.VersionCode = code;
            this.Display = display;
        }
        public override string ToString()
        {
            return this.Display;
        }
    }
}
