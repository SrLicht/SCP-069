using Exiled.API.Interfaces;
using System.ComponentModel;

namespace Scp069.System
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("True to disable the ASCII logo on the console, in case one should bother you")]
        public bool NotLogo { get; set; } = false;
        [Description("Show some Logs.Debug, you should turn this on if something doesn't work properly.")]
        public bool Debug { get; set; } = false;
        public Scp069Config Scp069 { get; set; } = new Scp069Config();

    }


}
