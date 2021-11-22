using GameServerCore.Enums;
using System;
using System.Numerics;
using GameServerCore.Domain.GameObjects;
using LeagueSandbox.GameServer.GameObjects.Stats;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;


namespace Buffs
{
    internal class FizzMalison : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_RENEWS;
        public int MaxStacks => 1;
        public bool IsHidden => false;
        IObjAiBase Owner;
        float damage;
        ISpell daspell;
        IObjAiBase daowner;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            Owner = ownerSpell.CastInfo.Owner;
            daowner = Owner;
            daspell = ownerSpell;

            ApiEventManager.OnHitUnit.AddListener(this, ownerSpell.CastInfo.Owner, TargetTakePoison, false);
        }

        public void TargetTakePoison(IAttackableUnit target, bool isCrit)
        {
            AddBuff("FizzSeastoneTrident", 4f, 1, daspell, target, daowner);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {

        }

        public void OnUpdate(float diff)
        {
        }
    }
}

