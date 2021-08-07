﻿using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using UnityEngine;
using Scp049 = Exiled.Events.Handlers.Scp049;
using ServerEvents = Exiled.Events.Handlers.Server;
using PlayerEvents = Exiled.Events.Handlers.Player;
using System.Linq;
using Mirror;
using Scp069.System;
using Scp069.EventHandlers;
using MEC;
using System.Collections.Generic;
using Exiled.API.Extensions;

namespace Scp069.Component
{
    public class Scp069Component : MonoBehaviour
    {
        private Player scp069;
        private float damageTimer, damageDeal = 0;
        private bool damageEnable = false;
        private CoroutineHandle enableDamage;
        public RoleType scp069roletype;
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
            scp069.Role = RoleType.Scp049;
            scp069.CustomInfo = $"<color=#E7205C>{scp069.Nickname}</color>\n<b><color=red>SCP-069</color></b>";
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
        private IEnumerator<float> EnableDamage(float seconds)
        {
            damageEnable = false;
            yield return Timing.WaitForSeconds(seconds);
            damageEnable = true;
        }
        private void DoDamage()
        {
            if (damageEnable && damageTimer <= Time.time)
            {
                scp069.Hurt(damageDeal, DamageTypes.RagdollLess, "SCP-069");
                damageDeal += Plugin.Instance.Config.Scp069.ClonerIncreaseDamageBy;
                damageTimer = Time.time + Plugin.Instance.Config.Scp069.ClonerDamageEvery;
            }
        }

        private void OnTryToRevive(StartingRecallEventArgs ev)
        {
            if (ev.Scp049 != scp069) return;
            ev.IsAllowed = false;
        }
        private void OnDeath(DyingEventArgs ev)
        {
            if (ev.Target != scp069) return;

            ev.Target.CustomInfo = null;
            ev.Target.DisplayNickname = null;
            Destroy();

        }
        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player != scp069) return;

            Destroy();
        }
        private void OnKill(DyingEventArgs ev)
        {
            if (ev.Killer != scp069 || ev.Target == scp069) return;

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
            // Steal shape 
            scp069roletype = ev.Target.Role;
            string targetname;
            if (string.IsNullOrEmpty(ev.Target.DisplayNickname))
            {
                targetname = ev.Target.Nickname;
            }
            else
            {
                targetname = ev.Target.DisplayNickname;
            }
            if (!Handlers.MainHandler.victims.Contains(ev.Target))
            {
                Handlers.MainHandler.victims.Add(ev.Target);
                Log.Info($"{Handlers.MainHandler.victims}");
            }
            ev.Killer.DisplayNickname = targetname;
            UpdateNickname(targetname);
            //UpdateItemOnHand(t); This not work for now.
            Log.Info("Antes de cambiar la forma");
            ev.Killer.Change069Appearance(scp069roletype);
            Log.Info("Despues de cambiar la forma");
            Log.Info($"Despues del steal shape");
            //Movement speed
            if (Plugin.Instance.Config.Scp069.movementSpeedIntesify > 0)
            {
                ev.Killer.EnableEffect(EffectType.Scp207, Plugin.Instance.Config.Scp069.movementSpeedDuration, Plugin.Instance.Config.Scp069.movementSpeedShoulbeAccumulated);
                ev.Killer.TryGetEffect(EffectType.Scp207, out var playerEffect);
                playerEffect.Intensity = Plugin.Instance.Config.Scp069.movementSpeedIntesify;
            }
        }
        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Target == scp069 && ev.HitInformations.GetDamageType() == DamageTypes.Scp207)
            {
                ev.Amount = 0;
            }
            else
            {
                return;
            }
        }
        private void OnSpawnRagdoll(SpawningRagdollEventArgs ev)
        {
            if (ev.Killer != scp069) return;
            if (!Plugin.Instance.Config.Scp069.spawnVictimsRagdolls)
            {
                ev.IsAllowed = false;
            }
        }

        private void OnPlayerVerify(VerifiedEventArgs ev)
        {
            scp069.Change069Appearance(scp069roletype);
        }
        private void OnPlayerLeft(DestroyingEventArgs ev)
        {
            if (ev.Player != scp069) return;
            Destroy();
        }
        private void UpdateNickname(string nicktodisplay)
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
        private void UpdateItemOnHand(ItemType it)
        {
            try
            {
                foreach (Player player in Player.List)
                {
                    player.SendFakeSyncVar(scp069.ReferenceHub.networkIdentity, typeof(Inventory),
                  nameof(Inventory._curItemSynced), (sbyte)it);
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.TargetSite} {e.Message}\n{e.StackTrace}");
            }
        }

    }
}