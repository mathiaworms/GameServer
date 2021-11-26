using GameServerCore;
using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Enums;
using System.Numerics;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class Crystallize : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public float petTimeAlive = 0.00f;

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
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            var castrange = spell.GetCurrentCastRange();
           
            var wallduration = 5.0f; //TODO: Split into Active duration and Hidden duration when Invisibility is implemented
           
          //  var fearduration = 0.5f + (0.25 * (spell.CastInfo.SpellLevel - 1));
            var ownerPos = owner.Position;
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            if (Extensions.IsVectorWithinRange(ownerPos, spellPos, castrange))
            {
                IMinion m = AddMinion((IChampion)owner, "AniviaIceBlock", "AniviaIceBlock", spellPos);
                AddBuff("AniviaIceBlock", 5f, 1, spell, m, owner);
            //    AddParticle(owner, null, "JackintheboxPoof.troy", spellPos);
            
              //  var attackrange = m.Stats.Range.Total;

                
                    if (!m.IsDead)
                    {
                        CreateTimer(wallduration, () =>
                        {
                            if (!m.IsDead)
                            {
                                //TODO: Fix targeting issues
                                m.TakeDamage(m.Owner, 1000f, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, DamageResultType.RESULT_NORMAL);
                            }
                        });
                    }
                
            }
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
