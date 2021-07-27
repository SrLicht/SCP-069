using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Permissions.Extensions;
using Mirror;
using MEC;
using UnityEngine;
using System.Threading.Tasks;
using Scp069.System;
using Scp069.SCP_069;
using Exiled.API.Extensions;
using Random = System.Random;

namespace Scp069.EventHandlers
{
    public class MainHandlers
    {
        Plugin plugin = Plugin.Instance;
        Random random;
        /// <summary>
        /// SCP-069 Players
        /// </summary>
        public static List<Player> cloneGuy = new List<Player>();


        // I do not remember if this is really necessary, so for now I leave it like that and if I see any bug I will put it back.
        /*public void OnVerify(VerifiedEventArgs ev)
        {
            if (cloneGuy != null)
            {
                foreach(Player ply in cloneGuy)
                {
                    ply.ChangeAppearance(CloneGuy.cloneGuyRole);
                }
            }
        }*/

        public void RoundStart()
        {
            try
            {
                Timing.CallDelayed(1f, () => {
                    var list = Player.Get(RoleType.ClassD).ToList();
                    if (list.Count == 0 || list.Count() < plugin.Config.Scp069.ClonerRatsNeeded) return;

                    if (UnityEngine.Random.Range(1, 101) <= plugin.Config.Scp069.ClonerChance)
                    {
                        Player player = list[random.Next(list.Count())];

                        if (player == null)
                            return;

                        player.SetRole(RoleType.Scp049);
                        // The delay is necessary
                        Timing.CallDelayed(1.5f, () =>
                        {
                            player.GameObject.AddComponent<CloneGuy>();
                        });
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error("RoundStart Method: " + e.ToString());
            }

        }

        public void RoundEnd(RoundEndedEventArgs ev)
        {
            cloneGuy.Clear();
        }

    }
}
