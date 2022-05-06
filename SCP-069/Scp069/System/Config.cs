using Exiled.API.Interfaces;
using System.ComponentModel;

namespace Scp069.System
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("True to disable the ASCII logo on the console, in case one should bother you")]
        public bool NotLogo { get; set; } = false;

        [Description("If the list of players is less than this amount, SCP-069 will not appear.")]
        public int PlayersNeededToSpawn { get; set; } = 10;

        [Description("What is the probability that a SCP-069 will appear?")]
        public int Scp069Chance { get; set; } = 45;

        public Scp069.Handlers.Role.Scp069Role Scp069Config { get; set; } = new Handlers.Role.Scp069Role();
    }


}
