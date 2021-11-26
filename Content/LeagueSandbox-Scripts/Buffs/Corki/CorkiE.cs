using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs

{
    internal class CorkiE: IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_OVERLAPS;
        public int MaxStacks => 8;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IObjAiBase owner;
        IAttackableUnit Unit;
        IParticle p;
        float damage;
        float timeSinceLastTick = 500f;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = ownerSpell.CastInfo.Owner;
            Unit = unit;
		 if ( Unit.HasBuff("CorkiE"))
                {       
           		var statslost = (1.0f * owner.GetSpell(2).CastInfo.SpellLevel) *  Unit.GetBuffWithName("CorkiE").StackCount ;
           		 StatsModifier.Armor.FlatBonus -= statslost;
          		  unit.AddStatModifier(StatsModifier);
		}
		else {
			var  statslost = (1.0f * owner.GetSpell(2).CastInfo.SpellLevel)   ;
           		 StatsModifier.Armor.FlatBonus -= statslost;
          		  unit.AddStatModifier(StatsModifier);
		}

            
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {

        }

        public void OnUpdate(float diff)
        {
           
        }
    }
}
