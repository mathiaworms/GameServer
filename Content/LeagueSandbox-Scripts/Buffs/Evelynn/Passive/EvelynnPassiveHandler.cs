using System.Collections.Generic;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;

namespace Buffs
{
    class EvelynnPassiveHandler : IBuffGameScript
    {
        public BuffType BuffType => BuffType.INTERNAL;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();


        IObjAiBase Obj;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            if (unit is IObjAiBase obj)
            {
                Obj = obj;
                RemoveBuff(Obj, "EvelynnHateSpikeMarker");
                RemoveBuff(Obj, "EvelynnStealth");
                RemoveBuff(Obj, "EvelynnStealthMana");
                RemoveBuff(Obj, "EvelynnStealthMarker");
                RemoveBuff(Obj, "EvelynnStealthRing");
                RemoveBuff(Obj, "EvelynnWarning");
                RemoveBuff(Obj, "ShadowWalk");
                RemoveBuff(Obj, "ShadowWalkRestealthFadeout");
                RemoveBuff(Obj, "ShadowWalkRevealedWarning");
            }
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            if (unit is IObjAiBase obj)
            {
                AddBuff("EvelynnHateSpikeMarker", 25000.0f, 1, ownerSpell, obj, obj, true);
                AddBuff("EvelynnStealth", 25000.0f, 1, ownerSpell, obj, obj, true);
                AddBuff("EvelynnStealthMana", 25000.0f, 1, ownerSpell, obj, obj, true);
                AddBuff("EvelynnStealthMarker", 25000.0f, 1, ownerSpell, unit, obj, true);
                AddBuff("EvelynnStealthRing", 25000.0f, 1, ownerSpell, obj, obj, true);
                AddBuff("EvelynnWarning", 25000.0f, 1, ownerSpell, obj, obj, true);
                AddBuff("ShadowWalk", 25000.0f, 1, ownerSpell, obj, obj, true);
                AddBuff("ShadowWalkRestealthFadeout", 6.0f, 1, ownerSpell, obj, obj, true);
                AddBuff("ShadowWalkRevealedWarning", 25000.0f, 1, ownerSpell, obj, obj, true);
            }
        }

        public void OnUpdate(float diff)
        {
        }
    }
}