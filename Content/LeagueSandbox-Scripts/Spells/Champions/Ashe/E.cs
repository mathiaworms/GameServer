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
     public class AsheSpiritOfTheHawkCast : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
           //  AddBuff("AsheSpiritOfTheHawk", 2500000f, 1, spell, owner, owner);
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            AddParticleTarget(owner, owner, "Ashe_Base_E_mis.troy", owner, bone: "L_HAND");
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            float distance = 1750.0f + spell.CastInfo.SpellLevel * 750.0f;
            var targetPos = GetPointFromUnit(owner, distance);
            
        SpellCast(owner, 2, SpellSlotType.ExtraSlots, targetPos, targetPos, false, Vector2.Zero);
           
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
    public class AsheSpiritOfTheHawk : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
           MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            },
            IsDamagingSpell = false


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
          //  RemoveBuff(Owner, "AsheSpiritOfTheHawk");
            var owner = spell.CastInfo.Owner;
            var spellpos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);


               
                AddParticle(owner, null, "Ashe_Base_E_tar_explode.troy", spellpos, lifetime: 7f , reqVision: false);
                AddParticle(owner, null, "Ashe_Base_E_tar_linger.troy", spellpos, lifetime: 7f , reqVision: false);
                
                DamageSector = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 250f,
                  //  VisionRadius = 250f,
                    Tickrate = 2,
                    CanHitSameTargetConsecutively = true,
                    OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                    Type = SectorType.Area,
                    Lifetime = 5.0f
                });

            

        }

        public void OnSpellChannel(ISpell spell)
        {
        }
          public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {  
                missile.SetToRemove();

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
