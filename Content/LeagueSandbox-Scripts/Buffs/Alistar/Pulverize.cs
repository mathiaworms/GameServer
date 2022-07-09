using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using System;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
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
     class Pulverize : IBuffGameScript
    {
        public IBuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData 
        {
            BuffType = BuffType.COMBAT_DEHANCER
        };
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var randOffset = (float)new Random().NextDouble();
            var randPoint = new Vector2(unit.Position.X + (80.0f * randOffset), unit.Position.Y + 80.0f * randOffset);

            var xy = unit as IObjAiBase;
            xy.SetTargetUnit(null);

            ForceMovement(unit, "", randPoint, 90.0f, 80.0f, 20.0f, 0.0f );
            buff.SetStatusEffect(StatusFlags.CanAttack | StatusFlags.CanCast | StatusFlags.CanMove, false);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            buff.SetStatusEffect(StatusFlags.CanMove | StatusFlags.CanAttack | StatusFlags.CanCast, true);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}