using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    class EvelynnStealthRing : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IBuff thisBuff;
        IChampion champ;
        IParticle p;
        string pEveRing = "evelynn_ring_green.troy";

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var owner = ownerSpell.CastInfo.Owner;
            champ = unit as IChampion;
            thisBuff = buff;

            if (owner.Team == TeamId.TEAM_BLUE)
            {
                p = AddParticleTarget(owner, champ, pEveRing, champ, thisBuff.Duration,
                                      teamOnly: TeamId.TEAM_BLUE);
            }
            else
            {
                p = AddParticleTarget(owner, champ, pEveRing, champ, thisBuff.Duration,
                                      teamOnly: TeamId.TEAM_PURPLE);
            }
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);

            if (thisBuff != null)
            {
                thisBuff.DeactivateBuff(); 
            }
        }

        public void OnUpdate(float diff)
        {
        }
    }
}