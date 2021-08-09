using Exiled.API.Extensions;
using Exiled.API.Features;
using System.Linq;

namespace Scp069.System
{
    public static class Extensions
    {
        // It is called Translate, because it is actually a method I use on my server to translate roles into Spanish.
        public static string Translate(this RoleType type)
        {
            switch (type)
            {
                case RoleType.None:
                    return "In Lobby";
                case RoleType.Scp173:
                    return "SCP-173";
                case RoleType.ClassD:
                    return "Class-D";
                case RoleType.Spectator:
                    return "Spectator";
                case RoleType.Scp106:
                    return "SCP-106";
                case RoleType.NtfScientist:
                    return "MTF/NTF Scientist";
                case RoleType.Scp049:
                    return "SCP-049";
                case RoleType.Scientist:
                    return "Scientific";
                case RoleType.Scp079:
                    return "SCP-079";
                case RoleType.ChaosInsurgency:
                    return "Chaos Insurgent";
                case RoleType.Scp096:
                    return "SCP-096";
                case RoleType.Scp0492:
                    return "SCP-049-2";
                case RoleType.NtfLieutenant:
                    return "MTF/NTF Lieutenant";
                case RoleType.NtfCommander:
                    return "MTF/NTF Commander";
                case RoleType.NtfCadet:
                    return "MTF/NTF Cadet";
                case RoleType.Tutorial:
                    return "Tutorial";
                case RoleType.FacilityGuard:
                    return "Facility Guard";
                case RoleType.Scp93953:
                    return "SCP-939-53";
                case RoleType.Scp93989:
                    return "SCP-939-89";
                default:
                    return "No deberia de salir esto, pero ok.";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="type"></param>
        public static void Change069Appearance(this Player player, RoleType type)
        {
            foreach (var target in Player.List.Where(p => p != player && !p.IsScp))
            {
                target.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)type);
            }

        }
    }
}
