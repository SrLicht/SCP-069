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

                switch (ev.Name)
                {

                    case "069":

                        ev.Sender.SetRole(RoleType.Scp049);
                        Timing.CallDelayed(1f, () => ev.Sender.GameObject.AddComponent<CloneGuy>());
                        ev.ReplyMessage = "Wuuush you are gay now";
                        return;


                }
            }
            catch (Exception)
            {
                Log.Error("Oooops command error");
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


            if (UnityEngine.Random.Range(1, 101) <= plugin.Config.ClonerChance
                && plugin.Config.ClonerkRatsNeeded >= Player.Get(RoleType.ClassD).Count())
            {

                Player player = RoleType.ClassD.GetHubs().FirstOrDefault();

                if (player == null)
                    return;

                player.SetRole(RoleType.Scp049);
                Timing.CallDelayed(1.5f, () => {
                    player.GameObject.AddComponent<CloneGuy>();
                });
            }
        }

        public void RoundEnd(RoundEndedEventArgs ev)
        {
            cloneGuy = null;
        }

        public void ZombieStartRevive(StartingRecallEventArgs ev)
        {
            if (cloneGuy == ev.Scp049)
                return;

        }

        public void ZombieEndRevive(FinishingRecallEventArgs ev)
        {
            if (cloneGuy == ev.Scp049)
                return;
        }

    }
}
