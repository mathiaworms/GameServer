using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;


namespace Buffs
{
    internal class KennenMarkOfStorm : IBuffGameScript
    {

        public BuffType BuffType => BuffType.COMBAT_DEHANCER;

        public BuffAddType BuffAddType => BuffAddType.STACKS_AND_RENEWS;
        public int MaxStacks => 3;
        public bool IsHidden => false;


        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle mos;
        IParticle mos2;
        IParticle mos3;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            switch (unit.GetBuffWithName("KennenMarkOfStorm").StackCount) //switch using enemy target mark of storm buff stack count
            {
                case 1:
                    mos = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "kennen_mos1.troy", unit, buff.Duration);
                    break;
                case 2:
                    RemoveParticle(mos); //remove mark of storm1 particle and replace with mos2 particle
                    mos2 = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "kennen_mos2.troy", unit, buff.Duration);
                    break;
                case 3:
                    RemoveParticle(mos2);
                    mos3 = AddParticleTarget(ownerSpell.CastInfo.Owner, unit, "kennen_mos_tar.troy", unit, buff.Duration);

                    break;
            }
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)

        {
            RemoveParticle(mos); //remove all particles upon deactivation
            RemoveParticle(mos2);
            RemoveParticle(mos3);

        }

        public void OnPreAttack(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
