using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;

namespace Spells
{
    public class TalonShadowAssault : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ownermain = owner;
        }
        bool attackUlt = false;
        bool procced = false;
        IObjAiBase ownermain;
        private void ExecuteUlt(IAttackableUnit target, bool arg3)
        {
            var owner = ownermain;
            attackUlt = true;
            if (attackUlt == true)
            {
                for (int bladeCount = 0; bladeCount <= 7; bladeCount++)
                {
                    if(procced == false)
                    {
                        var targetPosReturn = GetPointFromUnit(owner, 1150f, (bladeCount * 45f));
                        SpellCast(owner, 3, SpellSlotType.ExtraSlots, target.Position, target.Position, true, targetPosReturn);
                        CreateTimer(0.1f, () => { procced = true; });
                    }
                }
                CreateTimer(2.5f, () => { procced = false; });
                CreateTimer(2.5f, () => { attackUlt = false; });
            }
        }
        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            //PlayAnimation(owner, "Spell1");
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            AddBuff("TalonDisappear", 2.5f, 1, spell, owner, owner);
            AddParticleTarget(spell.CastInfo.Owner, spell.CastInfo.Owner, "talon_invis_cas.troy", spell.CastInfo.Owner, 2.5f);
            ApiEventManager.OnHitUnit.AddListener(this, owner, ExecuteUlt, false);
            CreateTimer(2.5f, () => { ApiEventManager.OnHitUnit.RemoveListener(this); });
            CreateTimer(2.5f, () => { owner.SetStatus(StatusFlags.Targetable, true); });
            owner.SetStatus(StatusFlags.Targetable, false);

            var Champs = GetChampionsInRange(owner.Position, 50000, true);
            foreach (IChampion player in Champs)
            {
                CreateTimer(2.5f, () => { owner.SetHealthbarVisibility((int)player.GetPlayerId(), owner, true); });
                if (player.Team.Equals(owner.Team))
                {
                    CreateTimer(2.5f, () => { owner.SetInvisible((int)player.GetPlayerId(), owner, 1f, 0.1f); });
                    owner.SetInvisible((int)player.GetPlayerId(), owner, 0.5f, 0.1f);
                    AddParticleTarget(owner, owner, "Khazix_Base_R_Cas.troy", owner);
                }
                if (!(player.Team.Equals(owner.Team)))
                {
                    if (player.IsAttacking)
                    {
                        player.CancelAutoAttack(false);
                    }
                    CreateTimer(2.5f, () => { owner.SetInvisible((int)player.GetPlayerId(), owner, 1f, 0.1f); });
                    owner.SetInvisible((int)player.GetPlayerId(), owner, 0f, 0.1f);
                    owner.SetHealthbarVisibility((int)player.GetPlayerId(), owner, false);
                }
            }

            PlayAnimation(owner, "Spell4");
            AddParticleTarget(owner, owner, "talon_ult_cas.troy", owner, 1f);
            AddParticleTarget(owner, owner, "talon_ult_sound.troy", owner, 1f);
            for (int bladeCount = 0; bladeCount <= 7; bladeCount++)
            {
                var bladecountTimer = bladeCount;
                var targetPos = GetPointFromUnit(owner, 100f, bladeCount * 45f);
                SpellCast(owner, 3, SpellSlotType.ExtraSlots, targetPos, targetPos, true, Vector2.Zero);
                CreateTimer(2.5f, () => 
                { 
                    if(attackUlt == false)
                    {
                        var targetPosReturn = GetPointFromUnit(owner, 1150f, (bladecountTimer * 45f));
                        SpellCast(owner, 3, SpellSlotType.ExtraSlots, owner.Position, owner.Position, true, targetPosReturn);
                    }
                });
            }
        }

        public void OnSpellPostCast(ISpell spell)
        {

        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }

    public class TalonShadowAssaultMisOne : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            },
            IsDamagingSpell = true,
            TriggersSpellCasts = true

            // TODO
        };
        public List<IAttackableUnit> UnitsHit = new List<IAttackableUnit>();

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        private void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile arg3, ISpellSector arg4)
        {
            var owner = spell.CastInfo.Owner;
            var spellLevel = owner.GetSpell("TalonShadowAssault").CastInfo.SpellLevel;
            var ADratio = owner.Stats.AttackDamage.TotalBonus * 0.75f;
            var damage = 120 + 50f * (spellLevel - 1) + ADratio;

            if (!UnitsHit.Contains(target) && target != spell.CastInfo.Owner)
            {
                UnitsHit.Add(target);
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
                AddParticleTarget(owner, target, "talon_w_tar.troy", target, 1f);
            }
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }
        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            UnitsHit.Clear();
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
