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
    public class MissFortuneScattershot : ISpellScript
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
        IBuff thisBuff;
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
            var targetPos = GetPointFromUnit(owner, 800.0f);
            SpellCast(owner, 1, SpellSlotType.ExtraSlots, targetPos, targetPos, false, Vector2.Zero);
            var spellpos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);    


                AddParticle(owner, null, "MissFortune_Base_E_cas.troy", spellpos, lifetime: 2.0f , reqVision: false);
                AddParticle(owner, null, "MissFortune_Base_E_Unit_Tar.troy", spellpos, lifetime: 2.0f , reqVision: false);
                AddParticle(owner, null, "MissFortune_Base_E_Unit_Tar_green.troy",spellpos, lifetime: 2.0f , reqVision: false);
		AddParticle(owner, null, "MissFortune_Base_E_Unit_Tar_red.troy",spellpos, lifetime: 2.0f , reqVision: false);
                DamageSector = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 200f,
                    Tickrate = 1,
                    CanHitSameTargetConsecutively = true,
                    OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                    Type = SectorType.Area,
                    Lifetime = 2.0f
                });
                

        }

        public void OnSpellChannel(ISpell spell)
        {
        }
          public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {   
            var owner = spell.CastInfo.Owner;
             //var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.125f;
           // var damage = 5 + (5 * spell.CastInfo.SpellLevel ) + ap;

           // target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
            AddBuff("MissFortuneE", 1f, 1, spell, target, owner);

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