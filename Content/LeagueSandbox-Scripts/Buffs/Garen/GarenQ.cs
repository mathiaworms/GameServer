﻿using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    class GarenQBuff : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle pbuff;
        IParticle pbuff2;
        IBuff thisBuff;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            thisBuff = buff;
            var owner = ownerSpell.CastInfo.Owner as IChampion;

            SealSpellSlot(owner, SpellSlotType.SpellSlots, 0, SpellbookType.SPELLBOOK_CHAMPION, true);
            owner.CancelAutoAttack(true);
            StatsModifier.MoveSpeed.PercentBonus = 0.4f;
            unit.AddStatModifier(StatsModifier);
            ApiEventManager.OnPreAttack.AddListener(this, owner, OnPreAttack, true);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnPreAttack(ISpell spell)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            SpellCast(spell.CastInfo.Owner, 0, SpellSlotType.ExtraSlots, true, spell.CastInfo.Owner.TargetUnit, Vector2.Zero);
            if (thisBuff != null)
            {
                thisBuff.DeactivateBuff();
            }
        }
        public void OnUpdate(float diff)
        {
        }
    }
}
