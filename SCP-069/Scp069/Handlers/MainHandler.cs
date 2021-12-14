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
            scp069Players.Clear();
            victims.Clear();
        }

        public void ChanginRole(ChangingRoleEventArgs ev)
        {
            if (!scp069Players.Contains(ev.Player) && victims.Contains(ev.Player) && ev.Player != null && ev.NewRole != RoleType.Spectator)
            {
                victims.Remove(ev.Player);
            }
        }

        public void OnDestroying(DestroyingEventArgs ev)
        {
            if (victims.Contains(ev.Player))
            {
                victims.Remove(ev.Player);
            }
        }
        public void RoundStart()
        {
            try
            {
                Timing.CallDelayed(2.5f, () =>
                {

                    int classd = Player.Get(RoleType.ClassD).Count();

                    if (classd >= Plugin.Config.Scp069.ClassdNeeded)
                    {

                        if (UnityEngine.Random.Range(0, 101) <= Plugin.Config.Scp069.SpawnChance)
                        {
                            var plist = Player.List.Where(p => !p.IsScp);
                            Player scp069 = plist.Random();
                            if (scp069 == null)
                            {
                                return;
                            }

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
