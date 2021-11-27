using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    class ShadowWalkRevealedWarning : IBuffGameScript
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
        string pEveYikes = "evelynn-yikes.troy";

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            thisBuff = buff;
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(p);

            if (thisBuff != null)
            {
                thisBuff.DeactivateBuff();
            }
        }

        private void CheckEnemyProximity(IBuff buff, ISpell ownerSpell)
        {
            var unitsDetected = GetChampionsInRange(champ.Position, 1400, true);

            if (unitsDetected.Count >= 1)
            {
                foreach (var unit in unitsDetected)
                {
                    if (unit.Team != champ.Team)
                    {
                        p = AddParticleTarget(champ, champ, pEveYikes, champ, thisBuff.Duration, bone: "HEAD");
                    }
                }
            }
        }

        public void OnUpdate(float diff)
        {
            timeSinceLastTick += diff;

            if (timeSinceLastTick >= 10f)
            {
                CheckEnemyProximity(thisBuff, originSpell);
            }
        }
    }
}