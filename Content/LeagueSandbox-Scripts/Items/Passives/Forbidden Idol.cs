using GameServerCore.Domain.GameObjects;
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
    public class ItemID_3114 : IItemScript
    {
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IObjAiBase owner)
        {
           // StatsModifier.CooldownReduction.PercentBonus += 10.0f;
           // owner.AddStatModifier(StatsModifier);
        }
        public void OnDeactivate(IObjAiBase owner)
        {
        }
        public void OnUpdate(float diff)
        {
        }
    }
}