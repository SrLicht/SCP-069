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
        public override Version Version => new Version(1,5,1);
        public override Version RequiredExiledVersion => new Version(2,11,0);
        public override string Author { get; } = "SrLicht & Beryl";

        public override void OnEnabled()
        {
            try
            {
                Log.Info("Initializing EventsHandler...");

                Instance = this;

                handler = new MainHandlers();

                Server.RoundEnded += handler.RoundEnd;
                Server.RoundStarted += handler.RoundStart;
                //Player.Verified += handler.OnVerify;

                #region Logo

                if (!Config.NotLogo)
                {

                    // I still think it's better than the one Joker did in Exiled. Don't hit me Joker.
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
                Log.Error($"{e.TargetSite} {e.Message}\n{e.StackTrace}");
            }

        }

        public override void OnDisabled()
        {

            Server.RoundEnded -= handler.RoundEnd;
            Server.RoundStarted -= handler.RoundStart;
            //Player.Verified -= handler.OnVerify;
            handler = null;
            Instance = null;
            base.OnDisabled();

        }
    }
}
