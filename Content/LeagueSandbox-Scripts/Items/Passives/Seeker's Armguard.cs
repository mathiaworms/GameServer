using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain;
using System;
using GameServerCore.Enums;

namespace ItemPassives
{
    public class ItemID_3191 : IItemScript
    {
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
        IObjAiBase ItemOwner;
        int stacks = 0;
        public void OnActivate(IObjAiBase owner)
        {
            ItemOwner = owner;
            ApiEventManager.OnKillUnit.AddListener(this, owner, TargetExecute, false);
        }

        private void TargetExecute(IDeathData obj)
        {
            if(stacks != 30)
            {
                StatsModifier.AbilityPower.FlatBonus = (float)0.5;
                StatsModifier.Armor.FlatBonus = (float)0.5;
                ItemOwner.AddStatModifier(StatsModifier);
                stacks++;
            }
        }

        public void OnDeactivate(IObjAiBase owner)
        {
            ApiEventManager.OnKillUnit.RemoveListener(this);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
