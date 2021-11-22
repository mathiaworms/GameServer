using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    class PowerFist : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IBuff thisBuff;
        ISpell thisSpell;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            if (unit is IObjAiBase ai)
            {
                thisBuff = buff;
                thisSpell = ownerSpell;
                SealSpellSlot(ai, SpellSlotType.SpellSlots, 2, SpellbookType.SPELLBOOK_CHAMPION, true);
                // SetDodgePiercing(ai, true);
                ai.CancelAutoAttack(true);
                ai.SetAutoAttackSpell(ai.GetSpell((byte)SpellSlotType.ExtraSlots + 1), true);

                ApiEventManager.OnHitUnit.AddListener(this, ai, OnHitUnit, true);
            }
        }

        public void OnHitUnit(IAttackableUnit target, bool isCrit)
        {
            if (thisSpell == null)
            {
                return;
            }

            var ad = thisSpell.CastInfo.Owner.Stats.AttackDamage.Total;

            if (!(target is IBaseTurret))
            {
                // BreakSpellShields(target);
                AddBuff("PowerFistSlow", 0.5f, 1, thisSpell, target, thisSpell.CastInfo.Owner);
            }

            if (thisBuff != null)
            {
                thisBuff.DeactivateBuff();
            }
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            if (unit is IObjAiBase ai)
            {
                ai.CancelAutoAttack(true);
                ai.ResetAutoAttackSpell();
                SealSpellSlot(ai, SpellSlotType.SpellSlots, 2, SpellbookType.SPELLBOOK_CHAMPION, false);
                // SetDodgePiercing(ai, false);
                ApiEventManager.OnHitUnit.RemoveListener(this, ai);
            }
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
