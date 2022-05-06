using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs;
using Scp069.System;
using System.Linq;

namespace Scp069.Handlers
{
    public class MainHandler : Base.Handler
    {
        public override void Start()
        {
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
        }

        public override void Stop()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!ev.Player.GetCustomRoles().Any() && ev.NewRole == Plugin.Config.Scp069Config.Role)
            {
                if (Player.List.Count() < Plugin.Config.PlayersNeededToSpawn)
                    return;

                if (Plugin.random.Next(1, 100) <= Plugin.Config.Scp069Chance)
                {
                    if (CustomRole.TryGet(69, out var scp069))
                    {
                        scp069.AddRole(ev.Player);
                    }
                }
            }
        }
    }
}
