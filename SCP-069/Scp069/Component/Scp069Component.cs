using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using Scp069.System;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerEvents = Exiled.Events.Handlers.Player;
using Scp049 = Exiled.Events.Handlers.Scp049;

namespace Scp069.Component
{
    public class Scp069Component : MonoBehaviour
    {
        #region Variables
        private Player scp069;
        private float damageTimer, damageDeal = 0;
        private bool damageEnable = false;
        private CoroutineHandle enableDamage;
        public RoleType scp069roletype;
        #endregion

        private void Awake()
        {
            RegisteringEvents();
        }
        private void Start()
        {
            InvokeRepeating("DoDamage", 0.1f, 1f);
            scp069.ClearBroadcasts();
            scp069.Broadcast(Plugin.Instance.Config.Broadcasting.SpawnBroadcastDuration, Plugin.Instance.Config.Broadcasting.SpawnBroadcast.Replace("{dmg}", Plugin.Instance.Config.Scp069.ClonerDamageEvery.ToString()).Replace("{heal}", Plugin.Instance.Config.Scp069.ClonerLifesteal.ToString()));

        }
        private void OnDestroy()
        {
            UnRegisteringEvents();
        }

        public void Destroy()
        {
            try
            {
                if (scp069 != null)
                {
                    Handlers.MainHandler.scp069Players.Remove(scp069);
                    scp069.CustomInfo = null;
                    scp069.DisplayNickname = null;
                    RemoveTagForVictims();
                }

                Destroy(this);

            }
            catch (Exception e)
            {
                Log.Error($"Exception: {e}\n Couldn't destroy: {this}\nIs ReferenceHub null? {scp069 is null}");
            }
        }

        public void RegisteringEvents()
        {
            if (!(Player.Get(gameObject) is Player ply))
            {
                Log.Error($"{this} Error Getting Player");
                Destroy();
                return;
            }
            scp069 = ply;
            Handlers.MainHandler.scp069Players.Add(scp069);
            if (scp069.Role != RoleType.Scp049) scp069.Role = RoleType.Scp049;

            ///https://github.com/Exiled-Team/SerpentsHand/blob/main/SerpentsHand/Logic.cs#L70-L82 | Thank you

            string roleName = string.Empty;

            if (!string.IsNullOrEmpty(Plugin.Instance.Config.Scp069.RankColor)){
                roleName += $"<color={Plugin.Instance.Config.Scp069.RankColor}>";
            }

            roleName += $"{scp069.Nickname}\nSCP-069";

            if (!string.IsNullOrEmpty(Plugin.Instance.Config.Scp069.RankColor))
            {
                roleName += "</color>";
            }

            scp069.CustomInfo = roleName;
            scp069.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            scp069.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;

            scp069.Health = Plugin.Instance.Config.Scp069.ClonerHealth;
            scp069.MaxHealth = Plugin.Instance.Config.Scp069.ClonerMaxHealth;

            enableDamage = Timing.RunCoroutine(EnableDamage(Plugin.Instance.Config.Scp069.GracePeriodStart));

            PlayerEvents.Dying += OnDeath;
            PlayerEvents.Dying += OnKill;
            PlayerEvents.Hurting += OnHurting;
            PlayerEvents.Verified += OnPlayerVerify;
            PlayerEvents.Destroying += OnPlayerLeft;
            PlayerEvents.SpawningRagdoll += OnSpawnRagdoll;
            PlayerEvents.ChangingRole += OnChangingRole;
            Scp049.StartingRecall += OnTryToRevive;
            Log.Debug($"{scp069.Nickname} Became SCP-069 Initializing component.", Plugin.Instance.Config.Debug);
        }
        public void UnRegisteringEvents()
        {
            Timing.KillCoroutines(enableDamage);
            PlayerEvents.Dying -= OnDeath;
            PlayerEvents.Dying -= OnKill;
            PlayerEvents.Hurting -= OnHurting;
            PlayerEvents.Verified -= OnPlayerVerify;
            PlayerEvents.Destroying -= OnPlayerLeft;
            PlayerEvents.SpawningRagdoll -= OnSpawnRagdoll;
            Scp049.StartingRecall -= OnTryToRevive;

            CancelInvoke("DoDamage");
            var nully = "player is null";
            Log.Debug($"Destroying {this} which was {(scp069 == null ? nully : scp069.Nickname)}", Plugin.Instance.Config.Debug);
        }

        /// <summary>
        /// The grace period when killing and starting the game is thanks to this.
        /// </summary>
        private IEnumerator<float> EnableDamage(float seconds)
        {
            damageEnable = false;
            yield return Timing.WaitForSeconds(seconds);
            damageEnable = true;
        }

        /// <summary>
        /// This takes care of the damage to 069 and increases that damage more and more.
        /// </summary>
        private void DoDamage()
        {
            if (damageEnable && damageTimer <= Time.time)
            {
                scp069.Hurt(damageDeal, DamageTypes.RagdollLess, "SCP-069");
                damageDeal += Plugin.Instance.Config.Scp069.ClonerIncreaseDamageBy;
                damageTimer = Time.time + Plugin.Instance.Config.Scp069.ClonerDamageEvery;
            }
        }

        /// <summary>
        /// Preventing it from creating zombies
        /// </summary>
        /// <param name="ev"></param>
        private void OnTryToRevive(StartingRecallEventArgs ev)
        {
            if (ev.Scp049 != scp069) return;
            ev.IsAllowed = false;
        }

        /// <summary>
        /// Destroys the component when 069 dies
        /// </summary>
        /// <param name="ev"></param>
        private void OnDeath(DyingEventArgs ev)
        {
            if (ev.Target != scp069) return;
            Destroy();

        }

        /// <summary>
        /// If 069 changes its role when it has the component, the component will be destroyed.
        /// </summary>
        /// <param name="ev"></param>
        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player != scp069) return;

            Destroy();
        }

        /// <summary>
        /// Where the magic happens. The 069 recovers health, changes shape, obtains its victim's inventory and obtains movement speed (if this is activated).
        /// </summary>
        /// <param name="ev"></param>
        private void OnKill(DyingEventArgs ev)
        {
            if (ev.Killer != scp069 || ev.Target == scp069 || ev.Target == null || ev.Target.IsScp) return;

            // Gracer Period
            enableDamage = Timing.RunCoroutine(EnableDamage(Plugin.Instance.Config.Scp069.GracePeriodOnKill));

            // Scale
            if (ev.Killer.Scale != ev.Target.Scale && ev.Target.Scale.magnitude < 27)
            {
                ev.Killer.Scale = ev.Target.Scale;
            }
            // Inventory
            ev.Killer.ResetInventory(ev.Target.Inventory.items.ToList());
            ItemType t = ItemType.None;
            if (ev.Target.CurrentItem != null)
            {
                t = ev.Target.CurrentItem.id;
            }
            ev.Target.ClearInventory();
            //Heal
            var amount = Plugin.Instance.Config.Scp069.ClonerLifesteal;
            ev.Killer.Health = Mathf.Clamp(ev.Killer.Health + amount, 1, Plugin.Instance.Config.Scp069.ClonerMaxHealth);
            // Damage to deal
            damageDeal = 10;
            // Broadcast to Victim
            if (Plugin.Instance.Config.Broadcasting.BroadcastDuration > 0)
            {
                ev.Target.ClearBroadcasts();
                ev.Target.Broadcast(Plugin.Instance.Config.Broadcasting.BroadcastDuration, Plugin.Instance.Config.Broadcasting.Killbroadcast);
            }
            // Steal shape and name.
            scp069roletype = ev.Target.Role;
            string targetname = string.IsNullOrEmpty(ev.Target.DisplayNickname) ? ev.Target.Nickname : ev.Target.DisplayNickname;
            if (!Handlers.MainHandler.victims.Contains(ev.Target))
            {
                Handlers.MainHandler.victims.Add(ev.Target);
            }
            ev.Killer.DisplayNickname = targetname;
            UpdateNameForVictims(targetname);
            //UpdateItemOnHand(t); This not work for now.
            ev.Killer.Change069Appearance(scp069roletype);
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
            if (ev.Target == scp069 && ev.HitInformations.GetDamageType() == DamageTypes.Scp207)
            {
                ev.Amount = 0;
            }
        }

        /// <summary>
        /// If the plugin configuration says that 069 does not have to leave bodies, this will do exactly that.
        /// </summary>
        private void OnSpawnRagdoll(SpawningRagdollEventArgs ev)
        {
            if (ev.Killer != scp069) return;
            if (!Plugin.Instance.Config.Scp069.spawnVictimsRagdolls)
            {
                ev.IsAllowed = false;
            }
        }

        /// <summary>
        /// Each time a player joins the server, the 069 appearance will be updated for all players, including the new player. This is to avoid problems with players not seeing the current 069 appearance or seeing an old one.
        /// </summary>
        private void OnPlayerVerify(VerifiedEventArgs ev)
        {
            scp069.Change069Appearance(scp069roletype);
        }

        /// <summary>
        /// If 069 leaves the server, the component will be destroyed.
        /// </summary>
        private void OnPlayerLeft(DestroyingEventArgs ev)
        {
            if (ev.Player != scp069) return;
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
                    Timing.CallDelayed(0.5f, () => player.SendFakeSyncVar(scp069.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), $"{nicktodisplay} ({scp069.Nickname} - SCP-069)"));
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
                    Timing.CallDelayed(0.5f, () => player.SendFakeSyncVar(scp069.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), $"{scp069.Nickname}"));
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.TargetSite} {e.Message}\n{e.StackTrace}");
            }

        }

        /// <summary>
        /// This is supposed to make the 069 hold the object the victim had when he died, it stopped working, no idea why. I'm not good at this Mirror stuff
        /// </summary>
        /// <param name="it"></param>
        private void UpdateItemOnHand(ItemType it)
        {
            try
            {
                foreach (Player player in Player.List)
                {
                    player.SendFakeSyncVar(scp069.ReferenceHub.networkIdentity, typeof(Inventory),
                  nameof(Inventory.Network_curItemSynced), (sbyte)it);
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.TargetSite} {e.Message}\n{e.StackTrace}");
            }
        }

    }
}
