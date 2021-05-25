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

namespace Scp069.EventHandlers
{
    public class MainHandlers
    {
        Plugin plugin = Plugin.Instance;

        /// <summary>
        /// SCP-069 Players
        /// </summary>
        public static List<Player> cloneGuy;
       

        public void OnVerify(VerifiedEventArgs ev)
        {
            if (cloneGuy != null)
            {
                foreach(Player ply in cloneGuy)
                {
                    ply.ChangeAppearance(RoleType.Scp049);
                }
            }
        }

        public void RoundStart()
        {
            try
            {
                Timing.CallDelayed(1f, () => {
                    var list = Player.Get(RoleType.ClassD).ToList();

                    if (UnityEngine.Random.Range(1, 101) <= plugin.Config.ClonerChance
                     && list.Count() >= plugin.Config.ClonerRatsNeeded)
                    {

                        list.Shuffle();

                        Player player = list.FirstOrDefault();

                        if (player == null)
                            return;

                        player.SetRole(RoleType.Scp049);
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
