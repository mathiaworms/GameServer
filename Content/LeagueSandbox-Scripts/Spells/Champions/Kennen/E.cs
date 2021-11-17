using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class KennenLightningRush : ISpellScript
    {
        IAttackableUnit Unit;
        ISpell Spell;
        IObjAiBase Owner;

        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            Spell = spell;
            Owner = owner;

        }
        public void TakeDamage(IAttackableUnit unit, IAttackableUnit source)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            AddBuff("KennenLightningRush", 3f, 1, spell, owner, owner);
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

    public class KennenLRCancel : ISpellScript
    {
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
        

        }
        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
         AddParticleTarget(owner, owner, "kennen_lr_cas.troybin", owner, 1f);
        }
          private void ApplyAuraDamage(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
             
            
             AddParticleTarget(owner, owner, "Kennen_lr_tar.troybin", owner, 1f);
            var spellLevel = owner.GetSpell("KennenLightningRush").CastInfo.SpellLevel;
            var units = GetUnitsInRange(owner.Position, 150, true);
            foreach (var unit in units)
            {
                if (unit.Team != owner.Team)
                {
                   var ap = owner.Stats.AbilityPower.Total * 0.8f;
                    var damage = 40 + spell.CastInfo.SpellLevel * 40 +  ap;
                   
                    unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL,
                        false);
                }
            }
        }
        public void OnSpellPostCast(ISpell spell)
        {
           var owner = spell.CastInfo.Owner;

            RemoveBuff(owner, "KennenLightningRush");
            owner.SetSpell("KennenLightningRush", 2, true);
          
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
