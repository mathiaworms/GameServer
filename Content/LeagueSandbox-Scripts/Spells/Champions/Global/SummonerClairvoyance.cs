using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;

namespace Spells
{
    public class SummonerClairvoyance : ISpellScript
    {
        public ISpellSector DamageSector;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        
        {
            TriggersSpellCasts = true,
            CastingBreaksStealth = true,
            DoesntBreakShields = true,
            IsDamagingSpell = false,
            NotSingleTargetSpell = true,
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
             ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
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
               var owner = spell.CastInfo.Owner as IChampion;
            var targetPos = GetPointFromUnit(owner,25000.0f);
            
      
           

            var spellpos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);

                AddParticle(owner, null, "ClairvoyanceEyeLong.troy", spellpos, lifetime: 5f , reqVision: false);
               AddParticle(owner, null, "ClairvoyanceEye.troy", spellpos, lifetime: 5f , reqVision: false);
                AddParticle(owner, null, "ClairvoyanceEyeLong_green.troy", spellpos, lifetime: 5f , reqVision: false);
                AddParticle(owner, null, "ClairvoyanceEyeLong_red.troy", spellpos, lifetime: 5f , reqVision: false);
                 var tempMinion = AddMinion(owner, "TestCubeRender", "TestCubeRender", targetPos, ignoreCollision: true, targetable: false);
                AddBuff("ExpirationTimer", 5.0f, 1, spell, tempMinion, tempMinion);
                AddBuff("Clairvoyance", 5.0f, 1, spell, tempMinion, tempMinion);
               
        }
          public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
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

