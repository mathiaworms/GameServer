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
    public class VolibearW : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
        };

        IObjAiBase own;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnHitUnit.AddListener(this, owner, TargetExecute, false);
            ApiEventManager.OnLevelUpSpell.AddListener(this, spell, HideW, false);
            own = owner;
        }
        int hit = 0;
        public void TargetExecute(IAttackableUnit unit, bool arg2)
        {
            if(hit < 3) {
                hit++;
                LogDebug(hit.ToString());
            }
            if(hit == 3)
            {
                LogDebug("unlock W");
                SealSpellSlot(own, SpellSlotType.SpellSlots, 1, SpellbookType.SPELLBOOK_CHAMPION, false);
            }
        }

        public void HideW(ISpell spell)
        {
            CreateTimer((float)0.1, () => { SealSpellSlot(own, SpellSlotType.SpellSlots, 1, SpellbookType.SPELLBOOK_CHAMPION, true); });
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(ISpell spell)
        {
            var ap = own.Stats.HealthPoints.FlatBonus * 0.15f;
            LogDebug("HP: " + ap.ToString());
            float damage = (float)(ap + 35 + (own.Spells[1].CastInfo.SpellLevel * 45));
            spell.CastInfo.Targets[0].Unit.TakeDamage(own, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_PROC, false);
            SealSpellSlot(own, SpellSlotType.SpellSlots, 1, SpellbookType.SPELLBOOK_CHAMPION, true);
            hit = 0;
            //SealSpellSlot(spell.CastInfo.Owner, SpellSlotType.SpellSlots, 2, SpellbookType.SPELLBOOK_CHAMPION, true);
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
