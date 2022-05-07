using CommandSystem;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
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
                        response = $"{Plugin.Instance?.Config.TranslateCommand.IfSenderIsNotaPlayer}";
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
                                var msg = GetAll069();

                                response = msg;
                                return true;
                            }
                        case "give":
                            {
                                
                                if (arguments.Count > 2)
                                {
                                    response = Plugin.Instance.Config.TranslateCommand.TooManyArguments;
                                    return false;
                                }

                                if (arguments.Count == 2)
                                {
                                    Player plytogive;

                                    try
                                    {
                                        plytogive = Player.Get(arguments.At(1));
                                    }
                                    catch (Exception)
                                    {
                                        response = $"{Plugin.Instance.Config.TranslateCommand.SpecifiedPlayerDoesNotExist}";
                                        return false;
                                    }
                                    if(CustomRole.TryGet(69, out var role))
                                    {
                                        role.AddRole(plytogive);

                                        response = $"{Plugin.Instance.Config.TranslateCommand.GiveCommand_GivingtoAnotherPlayer.Replace("{nick}", plytogive.Nickname)}";
                                        return true;
                                    }
                                    else
                                    {
                                        response = "Error on get SCP069Role";
                                        return false;
                                    }
 
                                }
                                else
                                {
                                    var plySender = Player.Get(sender);

                                    if (CustomRole.TryGet(69, out var role))
                                    {
                                        role.AddRole(plySender);

                                        response = $"\n{Plugin.Instance.Config.TranslateCommand.GiveCommand_Givingtoyou}";
                                        return true;
                                    }
                                    else
                                    {
                                        response = "Error on get SCP069Role";
                                        return false;
                                    }
                                   
                                }
                            }
                        case "remove":
                            {
                                if (arguments.Count > 2)
                                {
                                    response = Plugin.Instance.Config.TranslateCommand.TooManyArguments;
                                    return false;
                                }
                                if (arguments.Count == 2)
                                {
                                    Player plytoremove;

                                    try
                                    {
                                        plytoremove = Player.Get(arguments.At(1));
                                    }
                                    catch (Exception)
                                    {
                                        response = $"{Plugin.Instance.Config.TranslateCommand.SpecifiedPlayerDoesNotExist}";
                                        return false;
                                    }

                                    try
                                    {
                                        if (CustomRole.TryGet(69, out var role))
                                        {
                                            role.RemoveRole(plytoremove);

                                            response = $"{Plugin.Instance.Config.TranslateCommand.RemoveCommand_RemovingPlayer.Replace("{nick}", plytoremove.Nickname)}";
                                            return true;
                                        }
                                        else
                                        {
                                            response = "Error on get SCP069Role";
                                            return false;
                                        }
                                        
                                    }
                                    catch (Exception)
                                    {

                                        response = $"{Plugin.Instance.Config.TranslateCommand.ThePlayerIsNot069}";
                                        return false;
                                    }

                                }
                                else
                                {

                                    var plySender = Player.Get(sender);
                                    try
                                    {

                                        if (CustomRole.TryGet(69, out var role))
                                        {
                                            role.RemoveRole(plySender);

                                            response = $"{Plugin.Instance.Config.TranslateCommand.RemoveCommand_RemovingYou}";
                                            return true;
                                        }
                                        else
                                        {
                                            response = "Error on get SCP069Role";
                                            return false;
                                        }
                                        
                                    }
                                    catch (Exception)
                                    {
                                        response = $"{Plugin.Instance.Config.TranslateCommand.ThePlayerIsNot069}";
                                        return false;
                                    }

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

        private string GetAll069()
        {
            string msg = $"{Plugin.Instance.Config.TranslateCommand.Scp069ListTitle}\n";
            var scpcount = 0;

            foreach (var ply in Player.List)
            {
                if (ply.SessionVariables.ContainsKey("IsSCP069"))
                {
                    msg += $"{Plugin.Instance.Config.TranslateCommand.Scp069ListPerPerson.Replace("{id}", ply.Id.ToString()).Replace("{nick}", ply.Nickname)}\n";
                    scpcount++;
                }
            }

            if(scpcount == 0)
            {
                return $"{Plugin.Instance.Config.TranslateCommand.NoScp069InList}\n";
            }
            else
            {
                return msg;
            }
        }
    }
}
