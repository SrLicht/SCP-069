using Exiled.API.Extensions;
using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;

namespace Scp069.System
{
    public static class Extensions
    {

        public static void Change069Appearance(this Player player, RoleType type)
        {
            foreach (var target in Player.List.Where(p => p != player && !p.IsScp))
            {
                target.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)type);
            }

        }

        public static T Random<T>(this IEnumerable<T> list)
        {
            return list.ToArray().Random();
        }

        public static T Random<T>(this T[] array)
        {
            return array[Plugin.Instance.random.Next(0, array.Length - 1)];
        }
    }
}
