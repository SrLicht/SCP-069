using System.ComponentModel;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Scp069.System
{
    public class Scp069Config
    {
        [Description("The color that the SCP-069 Tag will have")]
        public string RankColor { get; set; } = "#F11F1F";
        [Description("Determines if SCP-069 victims should leave bodies upon killing someone | This is broken since update 11.1 give thanks to NW")]
        public bool spawnVictimsRagdolls { get; set; } = false;
        [Description("The intensity of SCP-207 that will be given to SCP-069 when killing, if the amount is 0 no movement speed will be given.")]
        public byte movementSpeedIntesify { get; set; } = 1;
        [Description("The duration of the movement speed to be given to SCP-069. This will be ignored if movementSpeedIntesify = 0.")]
        public float movementSpeedDuration { get; set; } = 15f;
        [Description("The duration of the movement speed should be accumulated ?")]
        public bool movementSpeedShoulbeAccumulated { get; set; } = true;
        [Description("SCP-069 takes X damage every second. (X is the numerical value that you will have to specify below).")]
        public float DamageEvery { get; set; } = 10;
        [Description("The initial damage that SCP-069 will receive.")]
        public float DamagePerTick { get; set; } = 5f;
        [Description("For every second that passes, the damage increases by the amount you put here")]
        public float IncreaseDamageBy { get; set; } = 10;
        [Description("After this time, SCP-069 will begin to take damage for every second. Technically it is a Spawn protect but from its passive..")]
        public float GracePeriodStart { get; set; } = 30;
        [Description("When SCP-069 kills someone, they will not take damage per second, for as long as you specify (In seconds obviously)")]
        public float GracePeriodOnKill { get; set; } = 15;
        [Description("The amount of Class-D required for 069 to appear")]
        public int ClassdNeeded { get; set; } = 4;
        [Description("The probability that SCP-069 will appear, if the above requirement is met")]
        public int SpawnChance { get; set; } = 55;
        [Description("The amount of HP SCP-069 has")]
        public int Health { get; set; } = 1540;
        [Description("The maximum HP that SCP-069 can achieve")]
        public int MaxHealth { get; set; } = 2000;
        [Description("As it says, the amount of life that is healed by killing.")]
        public int Lifesteal { get; set; } = 150;
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
        [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string ArgumentEmpty { get; set; } = "Type\n\"069 help\"";
        public string TooManyArguments { get; set; } = "Too many arguments, use .069 help";
        public string IfSenderIsNotaPlayer { get; set; } = $"This command can only be executed from RemoteAdmin.";
        public string ThePlayerIsNot069 { get; set; } = "The player is not SCP-069";
        [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string SpecifiedPlayerDoesNotExist { get; set; } = "\nError getting player or obtaining SCP-069\nMaybe it got disconnected or you misspelled its name or ID";
        public string RoundDontStarted { get; set; } = "The round has to be started in order to execute this command";
        [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string Scp069ListTitle { get; set; } = "\nList of SCP-069\n|--ID--|--Nickname--|\n";
        public string Scp069ListPerPerson { get; set; } = "{id} - {nick} is SCP-069\n";
        [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string NoScp069InList { get; set; } = "\nThere is no SCP-069 in this round.";
        [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string GiveCommand_GivingtoAnotherPlayer { get; set; } = "\nGiving SCP-069 to {nick}";
        [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string GiveCommand_Givingtoyou { get; set; } = "\nGiving you the SCP-069, have fun.";
        public string GiveCommand_PlayerAlreadyis { get; set; } = "The player is already SCP-069";
        public string RemoveCommand_RemovingPlayer { get; set; } = "Removing SCP-069 of {nick}";
        public string RemoveCommand_RemovingYou { get; set; } = "Removing you the SCP-069.";
        public string HelpCommand_Title { get; set; } = "Commands you can use:";
        public string HelpCommand_listDescription { get; set; } = "Gives you a list of players that are SCP-069.";
        public string HelpCommand_giveDescription1 { get; set; } = "Gives you the SCP-069";
        public string HelpCommand_giveDescription2 { get; set; } = "It gives the specified player the SCP-069, there can be more than 1, there should not be any problem";
        public string HelpCommand_removeDescription1 { get; set; } = "Removes SCP-069";
        public string HelpCommand_removeDescription2 { get; set; } = "Remove the SCP-069 from the specified player, check first if he really has it.";



    }

}
