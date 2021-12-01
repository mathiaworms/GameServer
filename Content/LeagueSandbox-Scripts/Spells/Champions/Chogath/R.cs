using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class Feast : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Target
            }
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            var APratio = owner.Stats.AbilityPower.Total;
            var damage = 125 + spell.CastInfo.SpellLevel * 175 + APratio;
            var damagetrue = 1000.0f ;
            AddParticleTarget(owner, target, "Feast.troy", target, 1f);
            AddParticleTarget(owner, target, "Feast_tar_bloodless", target, 1f);
            AddParticleTarget(owner, target, "feast_tar_indicator", target, 1f);
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            
     //       AddBuff("IceBlast", 3f, 1, spell, target, owner);
             if (target is IMonster)
            {
            float mitdamage = target.Stats.GetPostMitigationDamage(damagetrue, DamageType.DAMAGE_TYPE_PHYSICAL, owner);
               target.TakeDamage(owner, damagetrue, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_SPELL, false);
               if (target.Stats.CurrentHealth - mitdamage < 0 || target.IsDead)
                    {
                        AddBuff("Feast", 25000f, 1, spell, owner, owner, true);
                        
                    }
            }
            if (target is IMinion)
            {
                float mitdamage = target.Stats.GetPostMitigationDamage(damagetrue, DamageType.DAMAGE_TYPE_PHYSICAL, owner);
               target.TakeDamage(owner, damagetrue, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_SPELL, false);
               if (target.Stats.CurrentHealth - mitdamage < 0 || target.IsDead)
                    {
                        AddBuff("Feast", 25000f, 1, spell, owner, owner, true);
                        
                    }
            }
            if (target is IChampion)
            {
                float mitdamage = target.Stats.GetPostMitigationDamage(damage, DamageType.DAMAGE_TYPE_PHYSICAL, owner);
               target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_SPELL, false);
               if (target.Stats.CurrentHealth - mitdamage < 0 || target.IsDead)
                    {
                        AddBuff("Feast", 25000f, 1, spell, owner, owner, true);
                        
                    }
            }
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
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
