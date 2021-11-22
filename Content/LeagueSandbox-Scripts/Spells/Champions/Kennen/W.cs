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
using System.Linq;
using GameServerCore;

namespace Spells
{
    public class KennenBringTheLight : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
                TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }
        IBuff thisBuff;
        public ISpellSector DamageSector;
        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            var ownerr = spell.CastInfo.Owner as IChampion;
            var spellLevel = ownerr.GetSpell("KennenBringTheLight").CastInfo.SpellLevel;
            
            var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.8f;
            var damage = 45 + spellLevel * 15 + ap;

                AddParticle(owner, null, "kennen_btl_beam.troy", ownerr.Position, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "kennen_btl_tar.troy", ownerr.Position, lifetime: 0.5f , reqVision: false);
               
                DamageSector = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 600f,
                    Tickrate = 2,
                    CanHitSameTargetConsecutively = false,
                    OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                    Type = SectorType.Area,
                    Lifetime = 0.5f
                });
        
            
            
            foreach (var unit in GetUnitsInRange(ownerr.Position, 600, true)
                .Where(x => x.Team == CustomConvert.GetEnemyTeam(ownerr.Team)))
            {
      
                if (unit is IAttackableUnit)
                {
                    if (unit.HasBuff("KennenMarkOfStorm") ) 
                       {
                        
                           unit.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                           AddBuff("KennenMarkOfStorm", 6f, 1, spell, unit, owner);
                        
                          if(unit.GetBuffWithName("KennenMarkOfStorm").StackCount == 3) //remove mos if stacks reach 3
                                {
                                unit.RemoveBuffsWithName("KennenMarkOfStorm");
                                AddBuff("Stun", 1.5f, 1, spell, unit, owner); //stun target for 1 second after 3 stacks

                                }

                       }
                
                }
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
