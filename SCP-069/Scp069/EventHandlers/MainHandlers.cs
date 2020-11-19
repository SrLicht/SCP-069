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
using CommandSystem;

namespace Scp069.EventHandlers
{
    public class MainHandlers
    {
        Plugin plugin;

        /// <summary>
        /// Can be null
        /// </summary>
        public static Player cloneGuy = null;
        public static RoleType cloneGuyRole = RoleType.Scp049;


        public void OnRACommand(SendingRemoteAdminCommandEventArgs ev)
        {
            try
            {
                if(ev.Name.Equals("069", StringComparison.OrdinalIgnoreCase)) 
                {
                    ev.Sender.SetRole(RoleType.Scp049);
                    Timing.CallDelayed(1f, () => ev.Sender.GameObject.AddComponent<CloneGuy>());
                    ev.ReplyMessage = "You've become SCP-069.";
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error("OnRACommand Method: " + e.ToString());
            }
        }

        public void JoinMessage(JoinedEventArgs ev)
        {
            if (cloneGuy != null)
            {
                ev.Player.ReferenceHub.SendCustomSyncVar(cloneGuy.ReferenceHub.networkIdentity, typeof(CharacterClassManager), (targetwriter) =>
                {
                    targetwriter.WritePackedUInt64(16UL);
                    targetwriter.WriteSByte((sbyte)cloneGuyRole);
                });
            }
        }

        public void RoundStart()
        {
            try 
            {
                if(UnityEngine.Random.Range(1, 101) <= plugin.Config.ClonerChance
                                && plugin.Config.ClonerkRatsNeeded >= Player.Get(RoleType.ClassD).Count()) 
                {

                    Player player = Player.Get(RoleType.ClassD).FirstOrDefault();

                    if(player == null)
                        return;

                    player.SetRole(RoleType.Scp049);
                    Timing.CallDelayed(1.5f, () => 
                    {
                        player.GameObject.AddComponent<CloneGuy>();
                    });
                }
            } catch(Exception e) 
            {
                Log.Error("RoundStart Method: " + e.ToString());
            }
            
        }

        public void RoundEnd(RoundEndedEventArgs ev)
        {
            cloneGuy = null;
            cloneGuyRole = RoleType.Scp049;
        }

    }
}
