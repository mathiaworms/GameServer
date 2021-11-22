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
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class ZacW : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
           TriggersSpellCasts = true
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
             var ownerr = spell.CastInfo.Owner as IChampion;
            var spellLevel = ownerr.GetSpell("ZacW").CastInfo.SpellLevel;
            var ap = owner.Stats.AbilityPower.Total ;
            
            AddParticle(owner, null, "Zac_W_cas.troy", ownerr.Position, lifetime: 0.5f , reqVision: false);
            AddParticle(owner, null, "Zac_W_Chunk_Splat.troy", ownerr.Position, lifetime: 0.5f , reqVision: false);
            AddParticle(owner, null, "Zac_W_Chunk_Timeout.troy", ownerr.Position, lifetime: 0.5f , reqVision: false);
        
            
            
            foreach (var enemy in GetUnitsInRange(ownerr.Position, 350, true)
                .Where(x => x.Team == CustomConvert.GetEnemyTeam(ownerr.Team)))
            {
      
                if (enemy is IObjAiBase)
                {
                    float lvlmaxhp = ( ((  0.02f * ((ap/100)*2) ) + ( 0.03f + 0.01f *  spell.CastInfo.SpellLevel )) * target.Stats.HealthPoints.Total ) ; 
                    var damage = 3 + spell.CastInfo.SpellLevel * 1 + lvlmaxhp;
                    enemy.TakeDamage(ownerr, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    AddParticleTarget(owner, enemy, "Zac_W_tar.troy", enemy);
                    AddBuff("Pulverize", 1.0f, 1, spell, target, ownerr);
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
          public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
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
