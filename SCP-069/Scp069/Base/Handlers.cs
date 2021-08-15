using Scp069.System;

namespace Scp069.Base
{
    /// <summary>
    /// by Cerberus Team: Beryl, SrLicht, ImUrX
    /// </summary>
    public abstract class Handler
    {
        /// <summary>
        /// Plugin Singleton instance.
        /// </summary>
        protected Plugin Plugin => Plugin.Instance;

        /// <summary>
        /// Activated when you start the Plugin, use it to initialize your variables and the class.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Activated when the plugin is deactivated or when the server is restarted, use it to clear variables and class.
        /// </summary>
        public abstract void Stop();
    }
}
