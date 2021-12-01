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
    public class ToxicShot : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
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
            CreateTimer((float)0.1, () => { SealSpellSlot(own, SpellSlotType.SpellSlots, 2, SpellbookType.SPELLBOOK_CHAMPION, true); });
        }

        public void TargetExecute(IAttackableUnit unit, bool arg2)
        {
            var ap = own.Stats.AbilityPower.Total;
            float damage = (float)(ap * 0.3 + own.GetSpell(2).CastInfo.SpellLevel * 10);
            unit.TakeDamage(own, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_PROC, false);
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
