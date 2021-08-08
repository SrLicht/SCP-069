using System;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using Exiled.API.Features;
using Scp069.EventHandlers;
using Scp069.Base;
using System.Collections.Generic;

namespace Scp069.System
{
    public class Plugin : Plugin<Config>
    {
        #region Plugin Variables
        private List<Base.Handler> handlers = new List<Base.Handler>();

        private static readonly Plugin Singleton = new Plugin();
        private Plugin()
        {
        }

        public Random random;
        #endregion

        /// <summary>
        /// Gets the only existing instance of this plugin.
        /// </summary>
        public static Plugin Instance => Singleton;

        public override Version Version => new Version(2, 0, 9);
        public override Version RequiredExiledVersion => new Version(2, 11, 0);
        public override string Author { get; } = "SrLicht & Beryl";

        public override void OnEnabled()
        {
            try
            {
                Log.Info("Initializing EventsHandler...");
                RegisterEvents();

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

            UnRegisteringEvents();
            base.OnDisabled();

        }

        public void RegisterEvents()
        {
            try
            {
                Log.Info("Loading MainHandler...");
                handlers = new List<Base.Handler> { new Handlers.MainHandler() };
                foreach (var item in handlers)
                {
                    item.Start();
                    Log.Debug($"Iniciando Handler {item.GetType().Name}");
                }
                Log.Info("Plugin fully loaded.");
            }
            catch (Exception e)
            {
                Log.Error($"{e.TargetSite} {e.Message}\n{e.StackTrace}");
                return;
            }

        }
        public void UnRegisteringEvents()
        {
            foreach (var item in handlers)
            {
                item.Stop();
            }
            Log.Info("Good bye.");

            random = null;
            handlers = null;

        }
    }
}
