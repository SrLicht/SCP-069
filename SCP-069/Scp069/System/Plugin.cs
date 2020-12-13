using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using Exiled.API.Features;
using Scp069.EventHandlers;
using Exiled.Events.EventArgs;

namespace Scp069.System
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance;

        MainHandlers handler;
        public override string Author { get; } = "SrLicht & Beryl";

        public override void OnEnabled()
        {
            try
            {
                Log.Debug("Initializing EventsHandler...");

                Instance = this;

                handler = new MainHandlers();

                Server.RoundEnded += handler.RoundEnd;
                Server.RoundStarted += handler.RoundStart;
                Server.SendingRemoteAdminCommand += handler.OnRACommand;

                Player.Joined += handler.JoinMessage;

                Log.Info("Plugin loaded correctly!");

                #region Logo

                if (!Config.NotLogo)
                {
                    

                    Log.Info("####################################################################");
                    Log.Info("");
                    Log.Debug(" ######   ######  ########            #####    #######   #######  ");
                    Log.Debug("##    ## ##    ## ##     ##          ##   ##  ##     ## ##     ## ");
                    Log.Debug("##       ##       ##     ##         ##     ## ##        ##     ## ");
                    Log.Debug(" ######  ##       ########  ####### ##     ## ########   ######## ");
                    Log.Debug("      ## ##       ##                ##     ## ##     ##        ## ");
                    Log.Debug("##    ## ##    ## ##                 ##   ##  ##     ## ##     ## ");
                    Log.Debug(" ######   ######  ##                  #####    #######   #######  ");
                    Log.Info("");
                    Log.Info("####################################################################");
                    
                }
                #endregion

            }
            catch (Exception e)
            {
                Log.Error("Problem loading plugin: " + e.StackTrace + "" + e.Message);
            }

        }


        public override void OnReloaded()
        {
            // Get uwu'd lolol
        }

        public override void OnDisabled()
        {

            Server.RoundEnded -= handler.RoundEnd;
            Server.RoundStarted -= handler.RoundStart;
            Server.SendingRemoteAdminCommand -= handler.OnRACommand;

            Player.Joined -= handler.JoinMessage;

            //Handlers

            handler = null;

        }
    }
}
