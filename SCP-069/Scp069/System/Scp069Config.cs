using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scp069.System
{
    public class Scp069Config
    {
        [Description("Determines if SCP-069 victims should leave bodies upon killing someone")]
        public bool spawnVictimsRagdolls { get; set; } = false;
        [Description("The intensity of SCP-207 that will be given to SCP-069 when killing, if the amount is 0 no movement speed will be given.")]
        public byte movementSpeedIntesify { get; set; } = 1;
        [Description("The duration of the movement speed to be given to SCP-069. This will be ignored if movementSpeedIntesify = 0.")]
        public float movementSpeedDuration { get; set; } = 15f;
        [Description("The duration of the movement speed should be accumulated ?")]
        public bool movementSpeedShoulbeAccumulated { get; set; } = true;
        [Description("SCP-069 took damage every X seconds. (X being the number specified below)")]
        public float ClonerDamageEvery { get; set; } = 10;
        [Description("For every second that passes, the damage increases by the amount you put here")]
        public float ClonerIncreaseDamageBy { get; set; } = 10;
        [Description("After this time, SCP-069 will begin to take damage for every second. Technically it is a Spawn protect.")]
        public float GracePeriodStart { get; set; } = 30;
        [Description("When SCP-069 kills someone, they will not take damage per second, for as long as you specify (In seconds obviously)")]
        public float GracePeriodOnKill { get; set; } = 15;
        [Description("The amount of Class-D required for 069 to appear")]
        public int ClonerRatsNeeded { get; set; } = 4;
        [Description("The probability that SCP-069 will appear, if the above requirement is met")]
        public int ClonerChance { get; set; } = 55;
        [Description("The amount of HP SCP-069 has")]
        public int ClonerHealth { get; set; } = 1540;
        [Description("The maximum HP that SCP-069 can achieve")]
        public int ClonerMaxHealth { get; set; } = 2000;
        [Description("As it says, the amount of life that is healed by killing.")]
        public int ClonerLifesteal { get; set; } = 150;

        public CommandTranslate TranslateCommand { get; set; } = new CommandTranslate();
    }
    public class BroadcastSetting
    {
        [Description("If this setting is greater than 0, the number you set will be the duration of the broadcast you send to the victim of SCP-069")]
        public ushort BroadcastDuration { get; set; } = 8;
        public string Killbroadcast { get; set; } = "<b>You were killed by <color=red>SCP-069</color></b>";
        public ushort SpawnBroadcastDuration { get; set; } = 8;
        public string SpawnBroadcast { get; set; } = "<b><size=25>You're <color=red>SCP-069</color>. When killing a human, you will steal it's shape, inventory and size. You will also receive {dmg} damage every few seconds until you find a new victim, also healing for {heal}hp on every kill.</size></b>";
    }
    public class CommandTranslate
    {
        public string ArgumentEmpty { get; set; } = "Type\n\"069 help\"";
        public string RoundDontStarted { get; set; } = "The round has to be started in order to execute this command";
        public string Scp069ListTitle { get; set; } = "\nList of SCP-069\n|--ID--|--Nickname--|\n";
        public string Scp069ListPerPerson { get; set; } = "{id} - {nick} is SCP-069\n";
        public string NoScp069InList { get; set; } = "\nThere is no SCP-069 in this round.";
        public string GiveCommand_GivingtoAnotherPlayer { get; set; } = "\nGiving SCP-069 to {nick}";
        public string GiveCommand_ErrorGettinAnotherPlayer { get; set; } = "\nError getting player\nMaybe it got disconnected or you misspelled its name or ID.";
        public string GiveCommand_Givingtoyou { get; set; } = "\nGiving you the SCP-069, have fun.";
        public string ErrorNameWithSpaces { get; set; } = "\nThe name does not have to have spaces.";
        public string RemoveCommand_RemovingPlayer { get; set; } = "Removing SCP-069 of {nick}";
        public string RemoveCommand_IsNotSCP069 { get; set; } = "The player is not SCP-069";
    }

}
