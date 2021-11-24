﻿using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using System.Collections.Generic;
using System;

namespace Spells
{
    public class AkaliBasicAttack : ISpellScript
    {
        private IAttackableUnit _target = null;

        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            _target = target;
            ApiEventManager.OnLaunchAttack.AddListener(this, owner, OnLaunchAttack, true);
            AddBuff("AkaliTwilightShroudCD", 0.65f, 1, spell, owner, owner);
            RemoveBuff(owner, "AkaliTwilightShroud");

        }

        public void OnLaunchAttack(ISpell spell)
        {
            spell.CastInfo.Owner.SetAutoAttackSpell("AkaliBasicAttack2", false);

            var owner = spell.CastInfo.Owner;
            var MarkAPratio = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.5f;
            var MarkDamage = 45 + 25 * (owner.GetSpell("AkaliMota").CastInfo.SpellLevel - 1) + MarkAPratio;
            
            if (_target.HasBuff("AkaliMota"))
            {
                _target.TakeDamage(owner, MarkDamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_PROC, false);
                AddParticleTarget(owner, _target, "akali_mark_impact_tar.troy", _target, 1f);
                RemoveBuff(_target, "AkaliMota");
                owner.Stats.CurrentMana += (15f + (5 * owner.GetSpell(0).CastInfo.SpellLevel));
            }

            // Passive On-Hit
            float passiveDamagePercent = (6 + (int)Math.Floor(owner.Stats.AbilityPower.TotalBonus / 6)) / 100f;
            var passiveDamage = owner.Stats.AttackDamage.Total * passiveDamagePercent;
            _target.TakeDamage(owner, passiveDamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
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

    public class AkaliBasicAttack2 : ISpellScript
    {
        private IAttackableUnit _target = null;

        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true

            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            _target = target;
            ApiEventManager.OnLaunchAttack.AddListener(this, owner, OnLaunchAttack, true);
            AddBuff("AkaliTwilightShroudCD", 0.65f, 1, spell, owner, owner);
            RemoveBuff(owner, "AkaliTwilightShroud");
        }

        public void OnLaunchAttack(ISpell spell)
        {
            spell.CastInfo.Owner.SetAutoAttackSpell("AkaliBasicAttack", false);

            var owner = spell.CastInfo.Owner;
            var MarkAPratio = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.5f;
            var MarkDamage = 45 + 25 * (owner.GetSpell("AkaliMota").CastInfo.SpellLevel - 1) + MarkAPratio;

            if (_target.HasBuff("AkaliMota"))
            {
                _target.TakeDamage(owner, MarkDamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_PROC, false);
                AddParticleTarget(owner, _target, "akali_mark_impact_tar.troy", _target, 1f);
                RemoveBuff(_target, "AkaliMota");
                owner.Stats.CurrentMana += (15f + (5 * owner.GetSpell(0).CastInfo.SpellLevel));
            }

            // Passive On-Hit
            float passiveDamagePercent = (6 + (int)Math.Floor(owner.Stats.AbilityPower.TotalBonus / 6)) / 100f;
            var passiveDamage = owner.Stats.AttackDamage.Total * passiveDamagePercent;
            _target.TakeDamage(owner, passiveDamage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
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

