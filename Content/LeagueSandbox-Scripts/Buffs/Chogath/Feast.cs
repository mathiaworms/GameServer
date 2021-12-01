using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain;


namespace Buffs
{
    internal class Feast : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_OVERLAPS;
        public int MaxStacks => 6;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

  
        IParticle p;
        IParticle p2;
        IObjAiBase Owner;
        IAttackableUnit AtOwner;
        public static float Qstacks = 0f;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
             Owner = ownerSpell.CastInfo.Owner ;

            var HealthBuff = 60f + 40f * ownerSpell.CastInfo.SpellLevel ;
            var sizemax = unit.GetBuffWithName("Feast").StackCount;
            if (sizemax > 6 ) 
            {
                 sizemax = 6;
            }
            StatsModifier.Size.PercentBonus = StatsModifier.Size.PercentBonus + 0.15f * sizemax ;
            StatsModifier.HealthPoints.BaseBonus += HealthBuff;
              unit.AddStatModifier(StatsModifier);
            unit.Stats.CurrentHealth += HealthBuff;



          
            //ApiEventManager.OnKillUnit.AddListener(this, AtOwner, NasusMoreStacks, true);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            //ApiEventManager.OnHitUnit.RemoveListener(this);

        }

        public void OnUpdate(float diff)
        {

        }
    }
}

