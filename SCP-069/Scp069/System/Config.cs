using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scp069.System
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("True to disable the ASCII logo on the console, in case one should bother you")]
        public bool NotLogo { get; set; } = false;
        #region SCP-069
        [Description("SCP-069 took damage every X seconds. (X being the number specified below)")]
        public float ClonerDamageEvery { get; set; } = 10;
        [Description("For every second that passes, the damage increases by the amount you put here")]
        public float ClonerIncreaseDamageBy { get; set; } = 10;
        [Description("The amount of Class-D required for 069 to appear")]
        public int ClonerkRatsNeeded { get; set; } = 4;
        [Description("The probability that SCP-069 will appear, if the above requirement is met")]
        public int ClonerChance { get; set; } = 55;
        [Description("The amount of HP SCP-069 has")]
        public int ClonerkHealth { get; set; } = 1540;
        [Description("As it says, the amount of life that is healed by killing.")]
        public int ClonerLifesteal { get; set; } = 150;
        [Description("If this setting is greater than 0, the number you set will be the duration of the broadcast you send to the victim of SCP-069")]
        public ushort BroadcastDuration { get; set; } = 8;
        public string Killbroadcast { get; set; } = "<b>You were killed by <color=red>SCP-069</color></b>";
        public ushort SpawnBroadcastDuration { get; set; } = 8;
        public string SpawnBroadcast { get; set; } = "<b><size=25>Eres el <color=red>SCP-069</color> al matar a un humano, adoptaras su tamaño, forma y inventario recibiras {dmg} de daño segundo pero te curaras {heal} por cada victima que asimiles</size></b>";
        #endregion

    }


}
