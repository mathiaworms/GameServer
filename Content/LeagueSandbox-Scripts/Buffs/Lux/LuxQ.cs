using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;


namespace Buffs
{
    internal class LuxQ : IBuffGameScript
    {
        public BuffType BuffType => BuffType.STUN;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; }

        IParticle buff1;
        IParticle buff2;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var caster = ownerSpell.CastInfo.Owner;
            unit.StopMovement();
            if (unit is IObjAiBase ai)
            {
                ai.SetTargetUnit(null, true);
            }

            buff1 = AddParticleTarget(caster, unit, "LuxLightBinding_cas.troy", unit);
            buff2 = AddParticleTarget(caster, unit, "LuxLightBinding_tar.troy", unit);
            unit.Stats.SetActionState(ActionState.CAN_MOVE, false);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            RemoveParticle(buff1);
            RemoveParticle(buff2);
            unit.Stats.SetActionState(ActionState.CAN_MOVE, true);
            if (unit is IMonster ai && buff.SourceUnit is IChampion ch)
            {
                ai.SetTargetUnit(ch, true);
            }
        }

        public void OnUpdate(float diff)
        {

        }
    }
}
