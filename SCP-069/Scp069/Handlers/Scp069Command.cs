using System;
using CommandSystem;
using Exiled.API.Features;
using Scp069.System;
using Exiled.API.Enums;
using Exiled.API.Extensions;

namespace Scp069.EventHandlers
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Scp069Command : ICommand
    {
        public string Command => "069";

        public string[] Aliases => new string[] { };

        public string Description => "<color=red>SCP-069 base command</color>";

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
                    else if (!Round.IsStarted)
                    {
                        response = $"The round has to be started in order to execute this command";
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
                                if (arguments.Count == 2)
                                {
                                    try
                                    {
                                        Player plytogive = Player.Get(arguments.At(1));
                                        plytogive.GameObject.AddComponent<Component.Scp069Component>();
                                        response = $"\nGiving SCP-069 to {plytogive.Nickname}";
                                        return true;
                                    }
                                    catch (Exception)
                                    {
                                        response = "\nError getting player\nMaybe it got disconnected or you misspelled its name or ID.";
                                        return false;
                                    }
                                }
                                else if (arguments.Count == 1)
                                {
                                    Player plySender = Player.Get((sender as CommandSender).SenderId);
                                    plySender.GameObject.AddComponent<Component.Scp069Component>();
                                    response = $"\nGiving you the SCP-069, have fun.";
                                    return true;
                                }
                                else
                                {
                                    response = $"The name does not have to have spaces.";
                                    return false;
                                }
                            }
                        case "remove":
                            {
                                if (arguments.Count == 2)
                                {
                                    try
                                    {
                                        Player plytoremove = Player.Get(arguments.At(1));
                                        try
                                        {
                                            plytoremove.GameObject.TryGetComponent<Component.Scp069Component>(out var component);
                                            component.Destroy();
                                            response = $"Removing SCP-069 of {plytoremove.Nickname}";
                                            return true;
                                        }
                                        catch (Exception)
                                        {

                                            response = $"\nThe player is not SCP-069";
                                            return false;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        response = $"\nThe player is not SCP-069";
                                        return false;
                                    }
                                }
                                else if (arguments.Count == 1)
                                {
                                    try
                                    {
                                        Player plySender = Player.Get((sender as CommandSender).SenderId);
                                        try
                                        {
                                            plySender.GameObject.TryGetComponent<Component.Scp069Component>(out var component);
                                            component.Destroy();
                                            response = $"Removing you the SCP-069.";
                                            return true;
                                        }
                                        catch (Exception)
                                        {
                                            response = $"\nThe player is not SCP-069";
                                            return false;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        response = "\nError getting player or obtaining SCP-069\nMaybe it got disconnected or you misspelled its name or ID or maybe it doesn't even have the 069 to remove it.";
                                        return false;
                                    }
                                }
                                else
                                {
                                    response = $"The name does not have to have spaces.";
                                    return false;
                                }
                            }
                        case "help":
                            {
                                string msg;
                                msg = $"Commands you can use:\n" +
                                    $"069 list | Gives you a list of players that are SCP-069.\n" +
                                    $"069 give | Gives you the SCP-069\n" +
                                    $"069 give [PlayerName/PlayerID] | It gives the specified player the SCP-069, there can be more than 1, there should not be any problem\n" +
                                    $"069 remove | Removes SCP-069\n" +
                                    $"069 remove [PlayerName/PlayerID] | Remove the SCP-069 from the specified player, check first if he really has it.\n";

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
            string msg = "\nList of SCP-069\n|--ID--|--Nickname--|\n";
            if (Handlers.MainHandler.scp069Players.Count > 0)
            {
                foreach (Player ply in Handlers.MainHandler.scp069Players)
                {
                    msg += $"{ply.Id} - {ply.Nickname} is SCP-069\n";
                }
            }
            else
            {
                msg = "\nThere is no SCP-069 in this round.";
            }
            return msg;
        }
    }
}
