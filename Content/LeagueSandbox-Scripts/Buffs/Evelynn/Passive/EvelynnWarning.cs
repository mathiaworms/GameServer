using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    class EvelynnWarning : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        float timeSinceLastTick;
        IBuff thisBuff;
        IChampion champ;
        ISpell originSpell;
        IParticle p;
        IParticle p2;
        string pRedEye = "evelynn_redeye.troy";
        string pYellowEye = "evelynn_yelloweye.troy";

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            champ = unit as IChampion;
            thisBuff = buff;
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);
            RemoveParticle(p2);

            if (thisBuff != null)
            {
                thisBuff.DeactivateBuff();
            }
        }

        private void CheckEnemyProximity(IBuff buff, ISpell ownerSpell)
        {
            var unitsDetected = GetChampionsInRange(champ.Position, 1400, true); //700 radius on detection
            var unitsUndetected = GetChampionsInRange(champ.Position, 2000, true); //plus 300 for warning

            if (unitsUndetected.Count >= 1)
            {
                foreach (var unit in unitsUndetected)
                {
                    if (unit.Team != champ.Team)
                    {
                        if (champ.Team == TeamId.TEAM_BLUE)
                        {
                            p = AddParticleTarget(champ, unit, pYellowEye, unit, thisBuff.Duration, bone: "HEAD",
                                                  teamOnly: TeamId.TEAM_BLUE);
                        }
                        else
                        {
                            p = AddParticleTarget(champ, unit, pYellowEye, unit, thisBuff.Duration, bone: "HEAD",
                                                  teamOnly: TeamId.TEAM_PURPLE);
                        }
                    }
                }
            }

            if (unitsDetected.Count >= 1)
            {
                foreach (var unit in unitsDetected)
                {
                    if (unit.Team != champ.Team)
                    {
                        if (champ.Team == TeamId.TEAM_BLUE)
                        {
                            p = AddParticleTarget(champ, unit, pRedEye, unit, buff.Duration, bone: "HEAD",
                                                  teamOnly: TeamId.TEAM_BLUE);
                        }
                        else
                        {
                            p = AddParticleTarget(champ, unit, pRedEye, unit, buff.Duration, bone: "HEAD",
                                                  teamOnly: TeamId.TEAM_PURPLE);
                        }
                    }
                }
            }
        }

        public void OnUpdate(float diff)
        {
            timeSinceLastTick += diff;
            //League Checks are usually done each 0.25 seconds, check if this is accurate
            if (timeSinceLastTick >= 250f)
            {
                CheckEnemyProximity(thisBuff, originSpell);
                timeSinceLastTick = 0;
            }
        }
    }
}