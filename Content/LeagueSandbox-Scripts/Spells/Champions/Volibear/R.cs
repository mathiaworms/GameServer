using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Enums;

namespace Spells
{
    public class VolibearR : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        IObjAiBase own;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnLevelUpSpell.AddListener(this, spell, HideE, false);
            ApiEventManager.OnHitUnit.AddListener(this, owner, TargetExecute, false);
            own = owner;
        }

        public void HideE(ISpell spell)
        {
        }
        bool on = false;
        public void TargetExecute(IAttackableUnit unit, bool arg2)
        {
            if (on != false)
            {
                LogDebug("LOL");
                var x = GetUnitsInRange(unit.Position, 500, true);
                foreach(var units in x)
                {
                    if(units.Team != own.Team)
                    {
                        float damage = new float[] { 75, 115, 155 }[own.Spells[3].CastInfo.SpellLevel - 1];
                        float ap = own.Stats.AbilityPower.Total * 0.3f;
                        AddParticle(own, units, "volibear_R_chain_lighting_01.troy", own.Position);
                        units.TakeDamage(own, damage + ap, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    }
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
            //SealSpellSlot(spell.CastInfo.Owner, SpellSlotType.SpellSlots, 2, SpellbookType.SPELLBOOK_CHAMPION, true);
        }

        public void OnSpellPostCast(ISpell spell)
        {
            on = true;
            LogDebug("Yo its NOT over");
            CreateTimer(12.0f, () => { on = false; });
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
