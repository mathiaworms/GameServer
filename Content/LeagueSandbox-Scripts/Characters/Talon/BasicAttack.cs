﻿using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;
using System.Collections.Generic;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;


namespace Spells
{
    public class TalonBasicAttack : ISpellScript
    {
		private IAttackableUnit Target = null;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };
        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
			Target = target;
			if (owner.HasBuff("TalonNoxianDiplomacyBuff"))
            {
				OverrideAnimation(owner, "Spell1", "Attack1");
			}
			else
			{
				OverrideAnimation(owner, "Attack1", "Spell1");
			}
            ApiEventManager.OnLaunchAttack.AddListener(this, owner, OnLaunchAttack, true);
        }

        public void OnLaunchAttack(ISpell spell)
        {
			var owner = spell.CastInfo.Owner;
			var spellLevel = owner.GetSpell("TalonNoxianDiplomacy").CastInfo.SpellLevel;
            var ADratio = owner.Stats.AttackDamage.Total * 0.3f;
            var damage =(30 * spellLevel) + ADratio;
			var ELevel = owner.GetSpell("TalonCutthroat").CastInfo.SpellLevel;
			var damageamp = 0.03f * (ELevel - 1);         
			var damages = 0.03f * (ELevel - 1);
            var MarkADratio = owner.Stats.AttackDamage.Total * damages;
			if (owner.HasBuff("TalonNoxianDiplomacyBuff"))
            {
			    Target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
				if (Target.Team != owner.Team && !(Target is IObjBuilding || Target is IBaseTurret))
				{
				AddBuff("TalonBleedDebuff", 6f, 1, spell, Target, owner);
				}
			}
			else
			{
			}
			if (Target.HasBuff("TalonDamageAmp"))
            {
				Target.TakeDamage(owner, MarkADratio, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
			}
			//spell.CastInfo.Owner.SetAutoAttackSpell("TalonBasicAttack2", false);
        }

        public void OnSpellCast(ISpell spell)
        {
			var owner = spell.CastInfo.Owner;
        }

        public void OnSpellPostCast(ISpell spell)
        {
			var owner = spell.CastInfo.Owner;
        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell, ChannelingStopSource reason)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }

    public class TalonBasicAttack2 : ISpellScript
    {
		private IAttackableUnit Target = null;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };
        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
			Target = target;
			if (owner.HasBuff("TalonNoxianDiplomacyBuff"))
            {
				OverrideAnimation(owner, "Spell1", "Attack2");
			}
			else
			{
				OverrideAnimation(owner, "Attack2", "Spell1");
			}
            ApiEventManager.OnLaunchAttack.AddListener(this, owner, OnLaunchAttack, true);
        }

        public void OnLaunchAttack(ISpell spell)
        {
			var owner = spell.CastInfo.Owner;
			var spellLevel = owner.GetSpell("TalonNoxianDiplomacy").CastInfo.SpellLevel;
            var ADratio = owner.Stats.AttackDamage.Total * 0.3f;
            var damage =(30 * spellLevel) + ADratio;
			var ELevel = owner.GetSpell("TalonCutthroat").CastInfo.SpellLevel;
			var damageamp = 0.03f * (ELevel - 1);         
			var damages = 0.03f * (ELevel - 1);
            var MarkADratio = owner.Stats.AttackDamage.Total * damages;
			if (owner.HasBuff("TalonNoxianDiplomacyBuff"))
            {
			    Target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
			    if (Target.Team != owner.Team && !(Target is IObjBuilding || Target is IBaseTurret))
				{
				AddBuff("TalonBleedDebuff", 6f, 1, spell, Target, owner);
				}
			}
			else
			{
			}
			if (Target.HasBuff("TalonDamageAmp"))
            {
				Target.TakeDamage(owner, MarkADratio, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
			}
			//spell.CastInfo.Owner.SetAutoAttackSpell("TalonBasicAttack2", false);
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

        public void OnSpellChannelCancel(ISpell spell, ChannelingStopSource reason)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
	public class TalonCritAttack : ISpellScript
    {
		private IAttackableUnit Target = null;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };
        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
			owner.SetAutoAttackSpell("TalonBasicAttack2", false);
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
			Target = target;
			if (owner.HasBuff("TalonNoxianDiplomacyBuff"))
            {
				OverrideAnimation(owner, "Spell1", "Crit");
			}
			else
			{
				OverrideAnimation(owner, "Crit", "Spell1");
			}
            ApiEventManager.OnLaunchAttack.AddListener(this, owner, OnLaunchAttack, true);
        }

        public void OnLaunchAttack(ISpell spell)
        {
			var owner = spell.CastInfo.Owner;
			var spellLevel = owner.GetSpell("TalonNoxianDiplomacy").CastInfo.SpellLevel;
            var ADratio = owner.Stats.AttackDamage.Total * 0.3f;
            var damage =(30 * spellLevel) + ADratio;
			var ELevel = owner.GetSpell("TalonCutthroat").CastInfo.SpellLevel;
			var damageamp = 0.03f * (ELevel - 1);         
			var damages = 0.03f * (ELevel - 1);
			var damager =damage * 2;
            var MarkADratio = owner.Stats.AttackDamage.Total * damages;
			if (owner.HasBuff("TalonNoxianDiplomacyBuff"))
            {
			    Target.TakeDamage(owner, damager, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, true);
			    if (Target.Team != owner.Team && !(Target is IObjBuilding || Target is IBaseTurret))
				{
				AddBuff("TalonBleedDebuff", 6f, 1, spell, Target, owner);
				}
			}
			else
			{
			}
			if (Target.HasBuff("TalonDamageAmp"))
            {
				Target.TakeDamage(owner, MarkADratio, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
			}
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

        public void OnSpellChannelCancel(ISpell spell, ChannelingStopSource reason)
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
