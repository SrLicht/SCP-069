using System;
using CommandSystem;
using Exiled.API.Features;
using Scp069.SCP_069;
using Scp069.System;
using Exiled.API.Enums;
using Exiled.API.Extensions;

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
                    if (arguments.IsEmpty())
                    {
                        response = $"Type\n\"069 help\"";
                        return false;
                    }
                    switch (arguments.At(0).ToLower())
                    {
                        case "list":
                            {
                                string r = AllScps069();
                                response = r;
                                return true;
                            }
                        case "give":
                            {
                                if (arguments.At(1) != null)
                                {
                                    try
                                    {
                                        Player plytogive = Player.Get(arguments.At(1));
                                        plytogive.GameObject.AddComponent<Component.SCP_069_Component>();
                                        response = $"\nGiving SCP-069 to {plytogive.Nickname}";
                                        return true;
                                    }
                                    catch (Exception)
                                    {
                                        response = "\nError getting player\nMaybe it got disconnected or you misspelled its name or ID.";
                                        return false;
                                    }
                                }
                                else
                                {
                                    Player plySender = Player.Get((sender as CommandSender).SenderId);
                                    plySender.GameObject.AddComponent<Component.SCP_069_Component>();
                                    response = $"\nGiving you the SCP-069, have fun.";
                                    return true;
                                }
                            }
                        case "remove":
                            {
                                if (arguments.At(1) != null)
                                {
                                    try
                                    {
                                        Player plytoremove = Player.Get(arguments.At(1));
                                        plytoremove.GameObject.TryGetComponent<Component.SCP_069_Component>(out var component);
                                        component.Destroy();
                                        response = $"\nRemoving SCP-069 to {plytoremove.Nickname}";
                                        return true;
                                    }
                                    catch (Exception)
                                    {
                                        response = "\nError getting player or obtaining SCP-069\nMaybe it got disconnected or you misspelled its name or ID or maybe it doesn't even have the 069 to remove it.";
                                        return false;
                                    }
                                }
                                else
                                {
                                    Player plySender = Player.Get((sender as CommandSender).SenderId);
                                    plySender.GameObject.TryGetComponent<Component.SCP_069_Component>(out var component);
                                    component.Destroy();
                                    response = $"\nRemoving you the SCP-069.";
                                    return true;
                                }
                            }
                        case "help":
                            {
                                string msg;
                                msg = $"Commands you can use:\n" +
                                    $"069 list | Gives you a list of players that are SCP-069." +
                                    $"069 give | Gives you the SCP-069" +
                                    $"069 give [PlayerName/PlayerID] | It gives the specified player the SCP-069, there can be more than 1, there should not be any problem" +
                                    $"069 remove | Removes SCP-069" +
                                    $"069 remove [PlayerName/PlayerID] | Remove the SCP-069 from the specified player, check first if he really has it.";

                                response = msg;
                                return true;
                            }
                        default:
                            response = "\nUse \"069 help\"";
                            return false;
                    }
                }
                else
                {
                    response = "\nWhat ? Sender null...";
                    return false;
                }
            }
            catch (Exception e)
            {

                Log.Error($"{e.TargetSite} {e.Message}\n{e.StackTrace}");
                response = "Error: " + e;
                return false;
            }

        }
        private string AllScps069()
        {
            string msg = "\nList of SCP-069\n";
            foreach (Player ply in Handlers.MainHandler.scp069Players)
            {
                msg += $"{ply.Id} - {ply.Nickname} is SCP-069\n";
            }
            return msg;
        }
    }
}
