using System;
using CommandSystem;
using Exiled.API.Features;
using Scp069.SCP_069;

namespace Scp069.EventHandlers
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Commad : ICommand
    {
        public string Command => "069";

        public string[] Aliases => new string[] { };

        public string Description => "<color=red> Test SCP-069 on you</color>";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (sender != null)
                {
                    Player ply = Player.Get((sender as CommandSender).SenderId);
                    if (!ply.IsVerified)
                    {
                        response = "Player not verify, please reconnect";
                        return false;
                    }
                    if (ply.RemoteAdminAccess)
                    {
                        ply.GameObject.AddComponent<CloneGuy>();
                        response = "You've become SCP-069.";
                        ply.ShowHint(response);
                        return true;

                    }
                    else
                    {
                        response = "Secret ending, you dont have to play SCP: SL so you don't die of bad optimization.";
                        return false;
                    }
                }
                else
                {
                    response = "What ? Sender null...";
                    return false;
                }
            }
            catch (Exception e)
            {

                Log.Error("SCP-069 Command Error: " + e);
                response = "Error: " + e;
                return false;
            }
            
        }
    }
}
