using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using Scp069.System;
using System;

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
                        response = $"{Plugin.Instance.Config.TranslateCommand.ArgumentEmpty}";
                        return false;
                    }
                    if (!(sender is PlayerCommandSender))
                    {
                        response = $"{Plugin.Instance.Config.TranslateCommand}";
                    }
                    else if (!Round.IsStarted)
                    {
                        response = $"{Plugin.Instance.Config.TranslateCommand.RoundDontStarted}";
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
                                        var plytogive = Player.Get(arguments.At(1));
                                        plytogive.GameObject.AddComponent<Component.Scp069Component>();
                                        response = $"{Plugin.Instance.Config.TranslateCommand.GiveCommand_GivingtoAnotherPlayer.Replace("{nick}", plytogive.Nickname)}";
                                        return true;
                                    }
                                    catch (Exception)
                                    {
                                        response = $"{Plugin.Instance.Config.TranslateCommand.SpecifiedPlayerDoesNotExist}";
                                        return false;
                                    }
                                }
                                else if (arguments.Count == 1)
                                {
                                    var plySender = Player.Get((sender as CommandSender).SenderId);
                                    plySender.GameObject.AddComponent<Component.Scp069Component>();
                                    response = $"\n{Plugin.Instance.Config.TranslateCommand.GiveCommand_Givingtoyou}";
                                    return true;
                                }
                                else
                                {
                                    response = $"{Plugin.Instance.Config.TranslateCommand.SpecifiedPlayerDoesNotExist}";
                                    return false;
                                }
                            }
                        case "remove":
                            {
                                if (arguments.Count == 2)
                                {
                                    try
                                    {
                                        var plytoremove = Player.Get(arguments.At(1));
                                        try
                                        {
                                            plytoremove.GameObject.TryGetComponent<Component.Scp069Component>(out var component);
                                            component.Destroy();
                                            response = $"{Plugin.Instance.Config.TranslateCommand.RemoveCommand_RemovingPlayer.Replace("{nick}", plytoremove.Nickname)}";
                                            return true;
                                        }
                                        catch (Exception)
                                        {

                                            response = $"{Plugin.Instance.Config.TranslateCommand.ThePlayerIsNot069}";
                                            return false;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        response = $"{Plugin.Instance.Config.TranslateCommand.SpecifiedPlayerDoesNotExist}";
                                        return false;
                                    }
                                }
                                else if (arguments.Count == 1)
                                {
                                    try
                                    {
                                        var plySender = Player.Get((sender as CommandSender).SenderId);
                                        try
                                        {
                                            plySender.GameObject.TryGetComponent<Component.Scp069Component>(out var component);
                                            component.Destroy();
                                            response = $"{Plugin.Instance.Config.TranslateCommand.RemoveCommand_RemovingYou}";
                                            return true;
                                        }
                                        catch (Exception)
                                        {
                                            response = $"{Plugin.Instance.Config.TranslateCommand.ThePlayerIsNot069}";
                                            return false;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        response = $"{Plugin.Instance.Config.TranslateCommand.SpecifiedPlayerDoesNotExist}";
                                        return false;
                                    }
                                }
                                else
                                {
                                    response = $"{Plugin.Instance.Config.TranslateCommand.SpecifiedPlayerDoesNotExist}";
                                    return false;
                                }
                            }
                        case "help":
                            {
                                string msg;
                                msg = $"{Plugin.Instance.Config.TranslateCommand.HelpCommand_Title}\n" +
                                    $"069 list | {Plugin.Instance.Config.TranslateCommand.HelpCommand_listDescription}\n" +
                                    $"069 give | {Plugin.Instance.Config.TranslateCommand.HelpCommand_giveDescription1}\n" +
                                    $"069 give [PlayerName/PlayerID] | {Plugin.Instance.Config.TranslateCommand.HelpCommand_giveDescription2}\n" +
                                    $"069 remove | {Plugin.Instance.Config.TranslateCommand.HelpCommand_removeDescription1}\n" +
                                    $"069 remove [PlayerName/PlayerID] | {Plugin.Instance.Config.TranslateCommand.HelpCommand_removeDescription2}\n";

                                response = msg;
                                return true;
                            }
                        default:
                            response = $"{Plugin.Instance.Config.TranslateCommand.ArgumentEmpty}";
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
            string msg = $"{Plugin.Instance.Config.TranslateCommand.Scp069ListTitle}\n";
            if (Handlers.MainHandler.scp069Players.Count > 0)
            {
                foreach (Player ply in Handlers.MainHandler.scp069Players)
                {
                    msg += $"{Plugin.Instance.Config.TranslateCommand.Scp069ListPerPerson.Replace("{id}", ply.Id.ToString()).Replace("{nick}", ply.Nickname)}\n";
                }
            }
            else
            {
                msg = $"{Plugin.Instance.Config.TranslateCommand.NoScp069InList}\n";
            }
            return msg;
        }
    }
}
