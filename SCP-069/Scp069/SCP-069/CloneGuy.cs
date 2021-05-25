using System;
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

namespace Scp069.SCP_069
{
    public class CloneGuy : MonoBehaviour
    {

        private Player player;
        private float damageTimer, damageDealt = 0;
        private CoroutineHandle enableDamage;
        private bool damageEnabled = false;
        private RoleType cloneGuyRole;

        private void Update()
        {
            if (damageEnabled && damageTimer <= Time.time)
            {
                player.Hurt(damageDealt, DamageTypes.RagdollLess, "SCP-069");
                damageDealt += Plugin.Instance.Config.ClonerIncreaseDamageBy;
                damageTimer = Time.time + Plugin.Instance.Config.ClonerDamageEvery;
            }
        }

        private void Awake()
        {
            try
            {
                player = Player.Get(gameObject);

                if (player.Role != RoleType.Scp049)
                {
                    player.SetRole(RoleType.Scp049);
                }

                PlayerEvents.Dying += OnKill;
                PlayerEvents.Verified += OnVerify;
                PlayerEvents.Dying += OnDeath;
                PlayerEvents.ChangingRole += OnRoleChange;
                PlayerEvents.Destroying += OnLeave;
                PlayerEvents.SpawningRagdoll += OnSpawnRag;
                Scp049.StartingRecall += OnRecall;
            }
            catch (Exception e)
            {
                Log.Error("Awake Method: " + e.ToString());
            }
        }

        private void Start()
        {
            try
            {
                if (player == null)
                    player = Player.Get(gameObject);

                enableDamage = Timing.RunCoroutine(EnableDamage(Plugin.Instance.Config.GracePeriodStart));

                MainHandlers.cloneGuy.Add(player);
                player.CustomInfo = $"<color=#E7205C>{player.DisplayNickname}</color>\n<b><color=red>SCP-069</color></b>";

                player.Health = Plugin.Instance.Config.ClonerHealth;
                player.MaxHealth = Plugin.Instance.Config.ClonerMaxHealth;
                player.ClearBroadcasts();
                player.Broadcast(Plugin.Instance.Config.SpawnBroadcastDuration, Plugin.Instance.Config.SpawnBroadcast.Replace("{dmg}", Plugin.Instance.Config.ClonerDamageEvery.ToString()).Replace("{heal}", Plugin.Instance.Config.ClonerLifesteal.ToString()));
            }
            catch (Exception e)
            {
                Log.Error("Start Method: " + e.ToString());
            }
        }

        private void OnDestroy()
        {
            try
            {
                Log.Info("069 Component Destroyed");
                PlayerEvents.Dying -= OnKill;
                PlayerEvents.Verified -= OnVerify;
                PlayerEvents.Dying -= OnDeath;
                PlayerEvents.ChangingRole -= OnRoleChange;
                PlayerEvents.Destroying -= OnLeave;
                PlayerEvents.SpawningRagdoll -= OnSpawnRag;
                Scp049.StartingRecall -= OnRecall;
                player.CustomInfo = string.Empty;

                MainHandlers.cloneGuy.Remove(player);

                Timing.KillCoroutines(enableDamage);
            }
            catch (Exception e)
            {
                Log.Error("OnDestroy Method: " + e.ToString());
            }
        }

        private void OnLeave(DestroyingEventArgs ev)
        {
            try
            {
                if (ev.Player == player)
                    Destroy(this);
            }
            catch (Exception e)
            {
                Log.Error("OnLeave Method: " + e);
            }
        }
        private void OnVerify(VerifiedEventArgs ev)
        {
            try
            {
                ev.Player.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(CharacterClassManager),
                    nameof(CharacterClassManager.NetworkCurClass), (sbyte)cloneGuyRole);
            }
            catch (Exception e)
            {

                Log.Error("OnVerify Method: " + e);
            }
        }

        private void OnRoleChange(ChangingRoleEventArgs ev)
        {
            try
            {
                if (ev.Player != player)
                    return;

                if (ev.NewRole != RoleType.Scp049)
                {
                    ev.Player.CustomInfo = "";
                    ev.Player.DisplayNickname = null;
                    Destroy(this);
                }

            }
            catch (Exception e)
            {
                Log.Error("OnRoleChange Method: " + e.ToString());
            }
        }

        private IEnumerator<float> EnableDamage(float seconds)
        {
            damageEnabled = false;
            yield return Timing.WaitForSeconds(seconds);
            damageEnabled = true;
        }

        private void OnRecall(StartingRecallEventArgs ev)
        {
            if (ev.Scp049 != player)
                return;

            ev.IsAllowed = false;
        }

        private void OnDeath(DyingEventArgs ev)
        {
            try
            {
                if (ev.Target != player)
                    return;

                Destroy(this);
                ev.Target.CustomInfo = "";
                ev.Target.DisplayNickname = null;
            }
            catch (Exception e)
            {
                Log.Error("OnDeath Method: " + e.ToString());
            }
        }

        private void OnKill(DyingEventArgs ev)
        {
            try
            {
                if (ev.Killer != player || ev.Target == player)
                    return;

                enableDamage = Timing.RunCoroutine(EnableDamage(Plugin.Instance.Config.GracePeriodOnKill));

                ev.Killer.Scale = ev.Target.Scale;

                ev.Killer.ResetInventory(ev.Target.Inventory.items.ToList());

                ItemType t = ItemType.None;
                if (ev.Target.CurrentItem != null)
                {
                    t = ev.Target.CurrentItem.id;
                }

                ev.Target.ClearInventory();

                ev.Killer.Health += Plugin.Instance.Config.ClonerLifesteal;

                if (ev.Killer.Health > Plugin.Instance.Config.ClonerMaxHealth)
                {
                    ev.Killer.Health = Plugin.Instance.Config.ClonerMaxHealth;
                }
                damageDealt = 10;
                cloneGuyRole = ev.Target.Role;

                if (Plugin.Instance.Config.BroadcastDuration > 0 && !MainHandlers.cloneGuy.Contains(ev.Target))
                {
                    ev.Target.ClearBroadcasts();
                    ev.Target.Broadcast(Plugin.Instance.Config.BroadcastDuration, Plugin.Instance.Config.Killbroadcast);

                }

                foreach (Player p in Player.List)
                {
                    if (p.Side == Side.Scp) continue;

                    p.SendFakeSyncVar(ev.Killer.ReferenceHub.networkIdentity, typeof(CharacterClassManager),
                       nameof(CharacterClassManager.NetworkCurClass), (sbyte)cloneGuyRole);


                    p.SendFakeSyncVar(ev.Killer.ReferenceHub.networkIdentity, typeof(NicknameSync),
                        nameof(NicknameSync.Network_myNickSync), ev.Target.Nickname);

                    p.SendFakeSyncVar(ev.Killer.ReferenceHub.networkIdentity, typeof(Inventory),
                      nameof(Inventory._curItemSynced), (sbyte)t);
                      
                }
            }
            catch (Exception e)
            {
                Log.Error("OnKill Method: " + e.ToString());
            }
        }

        private void OnSpawnRag(SpawningRagdollEventArgs ev)
        {
            if (!Plugin.Instance.Config.spawnVictimsRagdolls)
            {
                if (ev.Killer == player)
                {
                    ev.IsAllowed = false;
                }
            }
        }
    }
}
