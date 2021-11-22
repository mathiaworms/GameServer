using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;


namespace ShatterAuraSelf
{
    internal class ShatterAuraSelf : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IObjAiBase owner;
        IAttackableUnit Unit;
        ISpell spell;
        //float timeSinceLastTick = 1000.0f;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner;
            Unit = unit;
            spell = ownerSpell;
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
            //timeSinceLastTick += diff;

            
                var units = GetUnitsInRange(owner.Position, 400, true);
                for (int i = units.Count - 1; i >= 0; i--)
                {
                    if (units[i].Team == owner.Team && units[i] != owner && !(units[i] is IBaseTurret || units[i] is IObjBuilding || units[i] is IInhibitor || units[i] is IMinion))
                    {
                        AddBuff("ShatterAura", 1.5f, 1, spell, units[i], owner);
                    }
                }
            
        }
    }
}
