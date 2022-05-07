using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets._Scripts.Dissonance;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using PlayerStatsSystem;
using UnityEngine;
using YamlDotNet.Serialization;

namespace Scp069.Handlers.Role
{
    [CustomRole(RoleType.Scp049)]
    public class Scp069Role : CustomRole
    {
        #region Variables
        [YamlIgnore]
        public bool CanDoDamage = true;
        [YamlIgnore]
        public bool InGraceSpawn = true;
        [YamlIgnore]
        public bool InGraceKill = false;
        public float Damage = 0;
        [YamlIgnore]
        public float GracePeriodKill = 0;
        #endregion

        /// <inheritdoc />
        public override uint Id { get; set; } = 69;

        /// <inheritdoc />
        [YamlIgnore]
        public override RoleType Role { get; set; } = RoleType.Scp049;

        /// <summary>
        /// role that SCP-069 will have visible to all players.
        /// </summary>
        [YamlIgnore]
        public RoleType VisibleRole { get; set; } = RoleType.Scp049;

        /// <inheritdoc />
        [YamlIgnore]
        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>();

        /// <inheritdoc />
        [YamlIgnore]
        public override SpawnProperties SpawnProperties { get; set; } = null;

        #region Config

        /// <inheritdoc />
        public override int MaxHealth { get; set; } = 2800;

        /// <inheritdoc />
        public override string Name { get; set; } = "SCP-069";

        /// <inheritdoc />
        public override string Description { get; set; } = "An SCP who slowly lost HP but can heal killing humans and stealh is shape";

        /// <inheritdoc />
        public override string CustomInfo { get; set; } = "SCP-069";

        /// <inheritdoc />
        public override bool KeepInventoryOnSpawn { get; set; } = false;

        [Description("Multiplier used to modify the player's movement speed (running and walking).")]
        public float MovementMultiplier { get; set; } = 3.75f;

        [Description("Each time SCP-069 kills someone it will heal this amount of HP.")]
        public int Lifesteal { get; set; } = 200;

        [Description("The damage that SCP-069 will receive per second.")]
        public int DamagePerTick { get; set; } = 5;

        [Description("The damage that will be added to DamagePerTick each time a tick is passed.")]
        public int DamageAddPerTick { get; set; } = 2;

        [Description("During this period SCP-069 will not receive damage from its passive.")]
        public int SpawnGracePeriod { get; set; } = 120;

        [Description("When killing SCP-069 it will not receive damage from its passive for the time you set here. Can be accumulated by making several kills")]
        public int GracePeriodOnKill { get; set; } = 90;

        [Description("The limit to how long the grace period will be when making multiple kills.")]
        public int GracePeriodLimit { get; set; } = 180;

        [Description("SCP-069 should copy the inventory of its victims ?")]
        public bool CopyInventory { get; set; } = true;

        [Description("SCP-069 can fire ?")]
        public bool CanShoot { get; set; } = false;

        [Description("If CanShoot = False this message will appear every time you try to shoot")]
        public string MessageOnTryToShoot { get; set; } = "Your fingers rotted and you cannot shoot";

        public string SpawnMessageBroadcast { get; set; } = "You are SCP-069, your life decays after {0} seconds if you find victims you can heal yourself and postpone your death for a longer time, you will also steal their form and name.";

        public string CassieRecontainment { get; set; } = "scp 0 6 9 has been successfully terminated . termination cause {0}";
        #endregion

        /// <inheritdoc />
        protected override void ShowMessage(Player player)
        {
            var message = string.Format(SpawnMessageBroadcast, SpawnGracePeriod);
            player.Broadcast(10, message, shouldClearPrevious:true);
            base.ShowMessage(player);
        }

        /// <inheritdoc />
        protected override void RoleAdded(Player player)
        {
            if(player.Role != Role)
            {
                player.SetRole(Role);
            }

            player.MaxHealth = MaxHealth;
            player.UnitName = "Scp069";
            player.SessionVariables.Add("IsSCP069", null);
            Damage = DamagePerTick;

            Timing.CallDelayed(1.5f, () =>
            {
                player.Position = RoleExtensions.GetRandomSpawnProperties(RoleType.Scp049).Item1;
                player.ChangeWalkingSpeed(MovementMultiplier);
                player.ChangeRunningSpeed(MovementMultiplier);
            });

            Timing.RunCoroutine(Degeneration(player), $"{player.UserId}-degeneration");
            Timing.RunCoroutine(UpdateShape(player), $"{player.UserId}-updateShape");

            base.RoleAdded(player);
        }

        /// <inheritdoc />
        protected override void RoleRemoved(Player player)
        {
            player.SessionVariables.Remove("IsSCP069");
            player.DisplayNickname = string.Empty;

            Timing.KillCoroutines($"{player.UserId}-degeneration");
            Timing.KillCoroutines($"{player.UserId}-updateShape");

            Timing.CallDelayed(1.5f, () =>
            {
                player.ChangeWalkingSpeed(1f);
                player.ChangeRunningSpeed(1f);
            });

            base.RoleRemoved(player);
        }

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Scp049.StartingRecall += OnTryToRevive;
            Exiled.Events.Handlers.Player.Dying += OnKill;
            Exiled.Events.Handlers.Player.Shooting += OnShoot;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Scp049.StartingRecall -= OnTryToRevive;
            Exiled.Events.Handlers.Player.Dying -= OnKill;
            Exiled.Events.Handlers.Player.Shooting -= OnShoot;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            base.UnsubscribeEvents();
        }

        private void OnTryToRevive(StartingRecallEventArgs ev)
        {
            if (Check(ev.Scp049) && Role == RoleType.Scp049)
                ev.IsAllowed = false;
        }

        private void OnKill(DyingEventArgs ev)
        {
            if (Check(ev.Killer))
            {
                if (InGraceKill)
                {
                    GracePeriodKill += GracePeriodOnKill;
                    if (GracePeriodKill > GracePeriodLimit)
                        GracePeriodKill = GracePeriodLimit;
                }
                else if (InGraceSpawn)
                {
                    InGraceKill = true;
                    GracePeriodKill = GracePeriodOnKill;
                }
                else
                {
                    InGraceKill = true;
                    GracePeriodKill = GracePeriodOnKill;
                }

                //Heal
                ev.Killer.Heal(Lifesteal);

                //Copy Shape and Name

                var targetname = string.IsNullOrEmpty(ev.Target.DisplayNickname) ? ev.Target.Nickname : ev.Target.DisplayNickname;

                ev.Killer.DisplayNickname = targetname;
                VisibleRole = ev.Killer.Role.Type;
                ev.Killer.ChangeAppearance(ev.Target.Role.Type);

                // Copy Scale
                if (ev.Killer.Scale != ev.Target.Scale && ev.Target.Scale.magnitude < 27)
                {
                    ev.Killer.Scale = ev.Target.Scale;
                }

                // Copy inventory
                if (CopyInventory)
                {
                    var targeinventory = ev.Target.Inventory.UserInventory;
                    var targetammo = ev.Target.Ammo;

                    ev.Killer.Inventory.UserInventory = targeinventory;
                    ev.Killer.Inventory.UserInventory.ReserveAmmo = targetammo;
                    ev.Killer.Inventory.SendAmmoNextFrame = true;
                    ev.Killer.Inventory.SendItemsNextFrame = true;
                }
            }
        }

        private void OnShoot(ShootingEventArgs ev)
        {
            if (Check(ev.Shooter) && !CanShoot)
            {
                ev.IsAllowed = false;
                ev.Shooter.ShowHint(MessageOnTryToShoot);
            }
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if(Check(ev.Player) && ev.NewRole != Role)
            {
                RemoveRole(ev.Player);
            }
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if(ev.Target != null && ev.Attacker != null && Check(ev.Target) && ev.Attacker.IsScp ||
               ev.Target != null && ev.Attacker != null && Check(ev.Target) && ev.Attacker.SessionVariables.ContainsKey("IsSerpentHand"))
            {
                ev.IsAllowed = false;
            }
            
        }

        private void OnRecointament(AnnouncingScpTerminationEventArgs ev)
        {
            if(ev.Role.roleId == RoleType.Scp049 && Check(ev.Player))
            {
                var m = string.Format(CassieRecontainment, ev.TerminationCause);
                ev.IsAllowed = false;
                Cassie.Message(m);
            }
        }
        private IEnumerator<float> UpdateShape(Player player)
        {
            for (;;)
            {
                yield return Timing.WaitForSeconds(20f);

                if(VisibleRole != Role)
                    player.ChangeAppearance(VisibleRole);
                player.CustomInfo = $"{player.DisplayNickname ?? player.Nickname} | SCP-069";
                player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
                player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;
                player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.PowerStatus;
                player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.UnitName;
            }
        }

        private IEnumerator<float> Degeneration(Player ply)
        {
            for (;;)
            {
                if (InGraceSpawn)
                {
                    yield return Timing.WaitForSeconds(SpawnGracePeriod);

                    InGraceSpawn = false;
                }
                if (InGraceKill)
                {
                    yield return Timing.WaitForSeconds(GracePeriodKill);

                    InGraceKill = false;
                }

                yield return Timing.WaitForSeconds(1f);

                if (CanDoDamage)
                {
                    ply.Hurt(new UniversalDamageHandler(Damage, DeathTranslations.Bleeding));

                    Damage += DamageAddPerTick;
                }
                
            }
        }
    }
}
