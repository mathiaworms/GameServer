using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Scripting.CSharp;
using System;

namespace Spells
{
    public class AkaliMota : ISpellScript
    {
        IObjAiBase _owner;
        float _prevModifier;
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Target
            },
            TriggersSpellCasts = true

            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            _owner = owner;
          //  ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            AddBuff("AkaliTwilightShroudCD", 0.65f, 1, spell, owner, owner);
            RemoveBuff(owner, "AkaliTwilightShroud");
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile)
        {
            var owner = spell.CastInfo.Owner;
            var AP = owner.Stats.AbilityPower.Total * 0.4f;
            var damage = 15f + spell.CastInfo.SpellLevel * 20f + AP;
            
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            AddBuff("AkaliMota", 6f, 1, spell, target, owner);
            missile.SetToRemove();
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
            // Passive
            var bonusAd = _owner.Stats.AttackDamage.TotalBonus;
            float modifier = (float)Math.Floor(6 + bonusAd / 6);
            modifier /= 100f;
            _owner.Stats.SpellVamp.FlatBonus -= _prevModifier;
            _owner.Stats.SpellVamp.FlatBonus += modifier;
            _prevModifier = modifier;
        }
    }
}
