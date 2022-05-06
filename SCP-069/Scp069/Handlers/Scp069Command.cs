
/*namespace Scp069.EventHandlers
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
                                string r = AllScps069();
                                response = r;
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
                                    if (Handlers.MainHandler.scp069Players.Contains(plytogive))
                                    {
                                        response = $"{Plugin.Instance.Config.TranslateCommand.GiveCommand_PlayerAlreadyis}";
                                        return false;
                                    }

                                    plytogive.GameObject.AddComponent<Component.Scp069Component>();
                                    response = $"{Plugin.Instance.Config.TranslateCommand.GiveCommand_GivingtoAnotherPlayer.Replace("{nick}", plytogive.Nickname)}";
                                    return true;

                                }
                                else
                                {
                                    var plySender = Player.Get((sender as CommandSender).SenderId);

                                    if (Handlers.MainHandler.scp069Players.Contains(plySender))
                                    {
                                        response = $"{Plugin.Instance.Config.TranslateCommand.GiveCommand_PlayerAlreadyis}";
                                        return false;
                                    }
                                    plySender.GameObject.AddComponent<Component.Scp069Component>();
                                    response = $"\n{Plugin.Instance.Config.TranslateCommand.GiveCommand_Givingtoyou}";
                                    return true;
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
                                else
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
    */