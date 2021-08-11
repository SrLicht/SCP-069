using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using Scp069.System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scp069.Handlers
{
    public class MainHandler : Base.Handler
    {
        /// <summary>
        /// SCP-069 Players List | Do not add manually, the component automatically adds players that have SCP-069.
        /// </summary>

        public static List<Player> scp069Players = new List<Player>();

        /// <summary>
        /// List of victims who will be shown the message "(scp-069)" when they are a spectator
        /// </summary>
        public static List<Player> victims = new List<Player>();
        public override void Start()
        {
            Exiled.Events.Handlers.Server.RoundEnded += RoundEnd;
            Exiled.Events.Handlers.Server.RoundStarted += RoundStart;
            Exiled.Events.Handlers.Player.ChangingRole += ChanginRole;
        }

        public override void Stop()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= RoundEnd;
            Exiled.Events.Handlers.Server.RoundStarted -= RoundStart;
            Exiled.Events.Handlers.Player.ChangingRole -= ChanginRole;
            foreach (Player plys in scp069Players)
            {
                if (plys.GameObject.TryGetComponent<Component.Scp069Component>(out var comp))
                {
                    comp.Destroy();
                }
            }
            scp069Players = null;
            victims = null;
        }

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

        public void ChanginRole(ChangingRoleEventArgs ev)
        {
            if (victims.Contains(ev.Player) && ev.NewRole != RoleType.Spectator)
            {
                victims.Remove(ev.Player);
            }
        }
        public void RoundStart()
        {
            try
            {
                Timing.CallDelayed(2.2f, () =>
                {


                    int classd = RoleType.ClassD.GetHubs().Count();

                    if(classd >= plugin.Config.Scp069.ClonerRatsNeeded)
                    {

                        if (UnityEngine.Random.Range(0, 101) <= plugin.Config.Scp069.ClonerChance)
                        {
                            var plist = Player.List.Where(p => !p.IsScp);
                            Player scp069 = plist.Random();
                            if (scp069 == null)
                                return;

                            scp069.GameObject.AddComponent<Component.Scp069Component>();
                            scp069.Role = RoleType.Scp049;
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error($"{e.TargetSite} {e.Message}\n{e.StackTrace}");
            }

        }

        public void RoundEnd(RoundEndedEventArgs ev)
        {
            scp069Players.Clear();
            victims.Clear();
        }
    }
}
