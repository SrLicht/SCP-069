using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using Scp069.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scp069.Handlers.Component
{
    public class Scp069Component : MonoBehaviour
    {
        #region Variables
        // You probably don't like that I write my variables with capital letters, but honestly I don't care about your opinion.
        Player SCP069;
        bool DamageEnabled = false;
        float DamagePerTick = Plugin.Instance.Config.Scp069.DamagePerTick;
        float DamageCooldown = Plugin.Instance.Config.Scp069.DamageEvery;
        RoleType CurrentRoleType;
        #endregion

        private void Awake()
        {

        }

        private void Start()
        {

        }

        public void Destroy()
        {
            try
            {
                Destroy(this);
            }
            catch (Exception e)
            {
                Log.Error($"Exception: {e}\n Couldn't destroy: {this}\nIs ReferenceHub null? {SCP069 is null}");
            }
        }

        #region Event Handlers

        public void LoadEvents()
        {
            Timing.RunCoroutine(DoDamage().CancelWith(this).CancelWith(gameObject));
        }

        public void UnLoadEvents()
        {

        }

        /// <summary>
        /// Where the magic happens. The 069 recovers health, changes shape, obtains its victim's inventory and obtains movement speed (if this is activated).
        /// </summary>
        /// <param name="ev"></param>
        public void OnKill(DyingEventArgs ev)
        {
            if (ev.Killer != SCP069 || ev.Target == SCP069 || ev.Target == null || ev.Target.IsScp) return;

            //Grace Period

            Timing.RunCoroutine(GracePeriod(Plugin.Instance.Config.Scp069.GracePeriodOnKill).CancelWith(this).CancelWith(gameObject));

            // Scale
            if (ev.Killer.Scale != ev.Target.Scale && ev.Target.Scale.magnitude < 27)
            {
                ev.Killer.Scale = ev.Target.Scale;
            }

            // Inventory
            var victimInventory = ev.Target.Items;
            ev.Killer.ResetInventory((List<ItemType>)victimInventory);
            ItemType t = ItemType.None;

            if (ev.Target.CurrentItem != null)
            {
                t = ev.Target.CurrentItem.Type;
            }
            ev.Target.ClearInventory();

            //Life Steal
            var amount = Plugin.Instance.Config.Scp069.Lifesteal;
            ev.Killer.Health = Mathf.Clamp(ev.Killer.Health + amount, 1, Plugin.Instance.Config.Scp069.MaxHealth);

            // Broadcast to Victim
            if (Plugin.Instance.Config.Broadcasting.BroadcastDuration > 0)
            {
                ev.Target.ClearBroadcasts();
                ev.Target.Broadcast(Plugin.Instance.Config.Broadcasting.BroadcastDuration, Plugin.Instance.Config.Broadcasting.Killbroadcast);
            }

            // Steal shape and name.
            CurrentRoleType = ev.Target.Role;
            string targetname = string.IsNullOrEmpty(ev.Target.DisplayNickname) ? ev.Target.Nickname : ev.Target.DisplayNickname;
            if (!MainHandler.victims.Contains(ev.Target))
            {
                Handlers.MainHandler.victims.Add(ev.Target);
            }
            ev.Killer.DisplayNickname = targetname;
            UpdateNameForVictims(targetname);
            ev.Killer.Change069Appearance(CurrentRoleType);
            //Movement speed
            if (Plugin.Instance.Config.Scp069.movementSpeedIntesify > 0)
            {
                ev.Killer.EnableEffect(EffectType.Scp207, Plugin.Instance.Config.Scp069.movementSpeedDuration, Plugin.Instance.Config.Scp069.movementSpeedShoulbeAccumulated);
                ev.Killer.TryGetEffect(EffectType.Scp207, out var playerEffect);
                playerEffect.Intensity = Plugin.Instance.Config.Scp069.movementSpeedIntesify;
            }
        }

        /// <summary>
        /// Preventing damage from SCP-207
        /// </summary>
        /// <param name="ev"></param>
        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Target == SCP069 && ev.Handler.Type == DamageType.Scp207)
            {
                ev.Amount = 0;
            }
        }

        /// <summary>
        /// If the plugin configuration says that 069 does not have to leave bodies, this will do exactly that.
        /// </summary>
        private void OnSpawnRagdoll(SpawningRagdollEventArgs ev)
        {
            //Broken.. Thanks nw.

            /*if (ev.Killer != scp069) return;
            if (!Plugin.Instance.Config.Scp069.spawnVictimsRagdolls)
            {
                ev.IsAllowed = false;
            }*/
        }

        /// <summary>
        /// Each time a player joins the server, the 069 appearance will be updated for all players, including the new player. This is to avoid problems with players not seeing the current 069 appearance or seeing an old one.
        /// </summary>
        private void OnPlayerVerify(VerifiedEventArgs ev)
        {
            SCP069.Change069Appearance(CurrentRoleType);
        }

        /// <summary>
        /// If 069 leaves the server, the component will be destroyed.
        /// </summary>
        private void OnPlayerLeft(DestroyingEventArgs ev)
        {
            if (ev.Player != SCP069) return;
            Destroy();
        }

        /// <summary>
        /// Updates the name that 069 victims see upon death.
        /// </summary>
        private void UpdateNameForVictims(string nicktodisplay)
        {
            try
            {
                foreach (Player player in Handlers.MainHandler.victims)
                {
                    Timing.CallDelayed(0.5f, () => player.SendFakeSyncVar(SCP069.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), $"{nicktodisplay} ({SCP069.Nickname} - SCP-069)"));
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.TargetSite} {e.Message}\n{e.StackTrace}");
            }

        }

        /// <summary>
        /// Removes the (SCP-069) tag from its victims when the component is destroyed.
        /// </summary>
        private void RemoveTagForVictims()
        {
            try
            {
                foreach (Player player in Handlers.MainHandler.victims)
                {
                    Timing.CallDelayed(0.5f, () => player.SendFakeSyncVar(SCP069.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), $"{SCP069.Nickname}"));
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.TargetSite} {e.Message}\n{e.StackTrace}");
            }

        }

        #endregion

        #region Coroutines

        /// <summary>
        /// The grace period when killing and starting the game is thanks to this.
        /// </summary>
        private IEnumerator<float> GracePeriod(float seconds = 1f)
        {
            DamageEnabled = false;
            yield return Timing.WaitForSeconds(seconds);
            DamageEnabled = true;
        }

        private IEnumerator<float> DoDamage()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);

                if (DamageEnabled)
                {
                    SCP069.Hurt("SCP-069 Degeneration", DamagePerTick);

                    DamagePerTick += Plugin.Instance.Config.Scp069.DamageEvery;
                }
            }
        }
        #endregion

    }
}
