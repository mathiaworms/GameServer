using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer;

namespace ItemPassives
{
    public class ItemID_3010 : IItemScript
    {
        IChampion Owner;
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
        public float timers = 0.00f;
        public void OnActivate(IObjAiBase owner)
        {
            Owner = owner as IChampion;
           // StatsModifier.CooldownReduction.PercentBonus += 10.0f;
           // owner.AddStatModifier(StatsModifier);
           if (Owner.LevelUp() == true)
           {
        
                for (timers = 0.0f; timers < 5.0f; timers += 1.0f)
                                {
                                    CreateTimer(timers, () => {
                                       StatsModifier.HealthRegeneration.FlatBonus = 18.75f;
                                     StatsModifier.ManaRegeneration.FlatBonus = 25.0f;
                                        Owner.AddStatModifier(StatsModifier);
                                    });
                                }
            
             
           }
        }
        public void OnDeactivate(IObjAiBase owner)
        {
        }
        public void OnUpdate(float diff)
        {
        }
    }
}
