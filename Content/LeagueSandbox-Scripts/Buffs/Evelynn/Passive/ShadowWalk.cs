using System.Collections.Generic;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    class ShadowWalk : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IBuff thisBuff;
        IChampion champ;
        string aHide = "ShadowWalk_phase_in";
        string aReveal = "ShadowWalk_reveal";

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            champ = unit as IChampion;
            thisBuff = buff;

            PlayAnimation(champ, aHide);
            SetAnimStates(champ, new Dictionary<string, string> { { "RUN", "RUN2" } });

            //Add model transparency
            //Add screen tint
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            PlayAnimation(champ, aReveal);
            SetAnimStates(champ, new Dictionary<string, string> { { "RUN2", "RUN" } });

            //Remove model transparency
            //Remove screen tint

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