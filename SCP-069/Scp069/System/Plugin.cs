using System;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using Exiled.API.Features;
using Scp069.EventHandlers;

namespace Scp069.System
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance;

        MainHandlers handler;
        public override Version Version => new Version(1,0,0);
        public override Version RequiredExiledVersion => new Version(2,8,0);
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
                Player.Verified += handler.OnVerify;

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

                base.OnEnabled();
            }
            catch (Exception e)
            {
                Log.Error("Problem loading plugin: " + e);
            }

        }

        public override void OnDisabled()
        {

            Server.RoundEnded -= handler.RoundEnd;
            Server.RoundStarted -= handler.RoundStart;
            Player.Verified -= handler.OnVerify;
            handler = null;
            Instance = null;
            base.OnDisabled();

        }
    }
}
