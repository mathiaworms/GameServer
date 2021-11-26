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


namespace Spells
{
    public class PhosphorusBomb : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            CastingBreaksStealth = true,
            DoesntBreakShields = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true,


        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
             ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public ISpellSector DamageSector;

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
            var owner = spell.CastInfo.Owner;
            var targetPos = GetPointFromUnit(owner, 825.0f);
            SpellCast(owner, 1, SpellSlotType.ExtraSlots, targetPos, targetPos, false, Vector2.Zero);
            var spellpos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);        
            var wallduration = 6.0f;

                AddParticle(owner, null, "corki_phosphorous_bomb_cas.troy", spellpos, lifetime: 0.5f , reqVision: false);
                AddParticle(owner, null, "corki_phosphorous_bomb_tar.troy", spellpos, lifetime: 0.5f , reqVision: false);

                DamageSector = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 250f,
                    Tickrate = 2,
                    CanHitSameTargetConsecutively = false,
                    OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                    Type = SectorType.Area,
                    Lifetime = 0.5f
                });

          
                var ward8 = AddMinion(owner, "TestCubeRender", "TestCubeRender", spellpos, ignoreCollision: true, targetable: false);
                AddBuff("YellowTriket", 6f, 1, spell, ward8, ward8);
                   if (!ward8.IsDead)
                    {
                        CreateTimer(wallduration, () =>
                        {
                            if (!ward8.IsDead)
                            {
                                //TODO: Fix targeting issues
                                ward8.TakeDamage(ward8.Owner, 1000f, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, DamageResultType.RESULT_NORMAL);
                            }
                        });
                    }
     // ajouter test cube avec ward buff pour avoir la vision pendant 6 seconde 
         //   ward = AddMinion(owner, "YellowTrinket", "YellowTrinket", truecoords);
         //   AddBuff("YellowTriket", 6f, 1, spell, ward, ward);

        }

        public void OnSpellChannel(ISpell spell)
        {
        }
          public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {  
            var owner = spell.CastInfo.Owner;
		        var ad = spell.CastInfo.Owner.Stats.AttackDamage.Total * 0.5f;
                 var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.5f;
                var damage = 30.0f + (50.0f * spell.CastInfo.SpellLevel ) + ap + ad;
                target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
               // AddBuff("Blaze", 4f, 1, spell, target, owner);

          

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
