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
    public class CurseoftheSadMummy : ISpellScript
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
            var ownerr = spell.CastInfo.Owner as IChampion;
            var spellLevel = ownerr.GetSpell("CurseoftheSadMummy").CastInfo.SpellLevel;
            
            var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.8f;
            var damage = 50 + spellLevel * 100 + ap;

         

        
            
            
            foreach (var enemy in GetUnitsInRange(ownerr.Position, 550, true)
                .Where(x => x.Team == CustomConvert.GetEnemyTeam(ownerr.Team)))
            {
      
                if (enemy is IObjAiBase)
                {
                    enemy.TakeDamage(ownerr, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

                     AddBuff("Stun", 1.5f, 1, spell, target, ownerr);
                    AddParticleTarget(enemy, enemy, "Amumu_Sadrobot_Ultwrap.troy", enemy, 1f);
                }
            }
            
        }

        public void OnSpellCast(ISpell spell)
        {
             var owner = spell.CastInfo.Owner as IChampion;
             PlayAnimation(owner, "SPELL4");
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
