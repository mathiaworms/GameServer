using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class HeadButt : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            CastingBreaksStealth = true,
            DoesntBreakShields = true,
            TriggersSpellCasts = true,
            IsDamagingSpell = false,
            NotSingleTargetSpell = true
            // TODO
        };
         public static IAttackableUnit _target = null;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
           
           
      //    var current = new Vector2(owner.Position.X, owner.Position.Y);
      //   var to = Vector2.Normalize(new Vector2(target.Position.X, target.Position.Y) - current);
       //    var range = to * 650;
        //   var range2 = range * 1300;
       //   var trueCoords = current + range;
        //   var trueCoords2 = current + range2;
         //    ForceMovement(owner, "Spell2", target.Position, 1200, 1.0f, 5.0f, 0);
         //   AddBuff("Stun", 1.0f, 1, spell, target, owner);

         //   AddParticleTarget(owner, target, "HeadButt_tar.troy", target);

         //   ForceMovement(target, "Spell2", trueCoords2, 1200, 0, 5.0f, 0);

          
         //   var ap = owner.Stats.AbilityPower.Total * 0.7f;
         //   var damage = 0 + spell.CastInfo.SpellLevel * 55 + ap;
         //   target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL,
        //        DamageSource.DAMAGE_SOURCE_SPELL, false);
            _target = target;
       //     AddBuff("Charging", 0.395f - spell.CastInfo.SpellLevel * 0.012f, 1, spell, owner, owner);
       //     PlayAnimation(owner, "SPELL2", 0.395f, 0f , -10f);


        }

        public void OnSpellCast(ISpell spell)
        {
           //  var owner = spell.CastInfo.Owner;
        //    var trueCoords = GetPointFromUnit(owner, 425f);

        //    ForceMovement(owner, "Spell2", trueCoords, 1200, 0, 0, 0);
           
            
            
        }

        public void OnSpellPostCast(ISpell spell)
        {
          
        }
         public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, ISpellMissile missile)
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

