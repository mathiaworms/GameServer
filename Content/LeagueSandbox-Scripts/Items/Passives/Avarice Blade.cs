﻿using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.API;

namespace ItemPassives
{
    public class ItemID_3093 : IItemScript
    {
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
        float timeSinceLastTick = 500f;
        IObjAiBase Owner;
        public void OnActivate(IObjAiBase owner)
        {   Owner = owner;

        }
        public void OnDeactivate(IObjAiBase owner)
        {
        }
        public void OnUpdate(float diff)
        {
            timeSinceLastTick += diff;
            if (timeSinceLastTick >= 5000f && Owner != null)
            {
                Owner.Stats.Gold += 3.0f;
                timeSinceLastTick = 0;
            }
        }
    }
}