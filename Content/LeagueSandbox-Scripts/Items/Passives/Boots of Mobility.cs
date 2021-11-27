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
    public class ItemID_3117 : IItemScript
    {
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
         public float timers = 0.00f;
         IObjAiBase Owner;
        public void OnActivate(IObjAiBase owner)
        {  Owner = owner;
            StatsModifier.MoveSpeed.FlatBonus += 25.0f;

            owner.AddStatModifier(StatsModifier);
         //  ApiEventManager.OnBeingHit.AddListener(this, owner, OnBeingHit, false);
        }
/*
        public void OnBeingHit(IAttackableUnit owner, IAttackableUnit attacker)
        {
                StatsModifier.MoveSpeed.FlatBonus -= 105.0f;
                owner.AddStatModifier(StatsModifier);
                for (timers = 0.0f; timers < 5.0f; timers += 1.0f)
                                {
                                    CreateTimer(timers, () => {
                                       
                                    });
                                }
                this.OnActivate(Owner);
        }*/
        public void OnDeactivate(IObjAiBase owner)
        {
        }
        public void OnUpdate(float diff)
        {
        }
    }
}
