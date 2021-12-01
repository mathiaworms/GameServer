using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using System;
namespace Spells
{
    public class NamiW : ISpellScript
    {
         IAttackableUnit Target;
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
            Target = target;
            //owner.SpellAnimation("SPELL2");
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
             var owner = spell.CastInfo.Owner;
            if (spell.CastInfo.Targets[0].Unit.Team != spell.CastInfo.Owner.Team)
            {
                //spell.AddProjectileTarget("LuluWTwo", spell.CastInfo.SpellCastLaunchPosition, spell.CastInfo.Targets[0].Unit);
            }
            else
            {
               PerformHeal(owner, spell, Target);
            }
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, ISpellMissile missile)
        {
           

        
              var ap = owner.Stats.AbilityPower.Total * 0.5f;
              var damage = 30 + (40 * (spell.CastInfo.SpellLevel)) + ap;
              //Graves_SmokeGrenade_Cloud_Team_Green.troy
              //Graves_SmokeGrenade_Cloud_Team_Red.troy
              target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
           //   AddBuff("Stun", 1.5f, 1, spell, target, owner);
           

            missile.SetToRemove();
        }
          private void PerformHeal(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {

            var ap = owner.Stats.AbilityPower.Total * 0.3f;
            float healthGain = 35 + (spell.CastInfo.SpellLevel * 25) + ap;
            if (target.HasBuff("HealCheck"))
            {
                healthGain *= 0.5f;
            }
            var newHealth = target.Stats.CurrentHealth + healthGain;
            target.Stats.CurrentHealth = Math.Min(newHealth, target.Stats.HealthPoints.Total);

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
