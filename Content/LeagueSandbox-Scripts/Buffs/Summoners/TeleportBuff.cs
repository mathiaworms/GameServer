using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using Spells;

namespace Buffs
{
     class TeleportBuff : IBuffGameScript
    {
    public IBuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
    {
        BuffType = BuffType.INTERNAL
    };
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var test = buff.SourceUnit;
            AddParticle(unit, null, "global_ss_teleport_blue.troy", unit.Position, lifetime:4.0f);
            AddParticle(test, null, "global_ss_teleport_target_blue.troy", test.Position, lifetime: 4.0f);
            if(test is IMinion)
            {
                test.SetStatus(StatusFlags.CanMove, false);
                test.StopMovement();
            }
            unit.StopMovement();
            unit.SetStatus(StatusFlags.CanMove, false);
            unit.SetStatus(StatusFlags.CanCast, false);
            unit.SetStatus(StatusFlags.CanAttack, false);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var test = buff.SourceUnit;
            TeleportTo(unit as IObjAiBase, SummonerTeleport.endpos.X, SummonerTeleport.endpos.Y);
            AddParticle(unit, null, "global_ss_teleport_flash_blue.troy", unit.Position, lifetime: 4.0f);
            AddParticle(unit, null, "global_ss_teleport_sparkleslinger.troy", unit.Position, lifetime: 4.0f);
            AddParticle(unit, null, "global_ss_teleport_arrive_blue.troy", unit.Position, lifetime: 4.0f);
            if (test is IMinion)
            {
                test.SetStatus(StatusFlags.CanMove, true);
                //test.StopMovement();
            }
            unit.SetStatus(StatusFlags.CanMove, true);
            unit.SetStatus(StatusFlags.CanCast, true);
            unit.SetStatus(StatusFlags.CanAttack, true);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}