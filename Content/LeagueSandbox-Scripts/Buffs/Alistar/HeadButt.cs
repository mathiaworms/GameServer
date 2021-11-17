using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using System.Numerics;
using GameServerCore.Scripting.CSharp;

namespace Buffs
{
    internal class HeadButt : IBuffGameScript
    {
        public BuffType BuffType => BuffType.INTERNAL;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => true;

        public IStatsModifier StatsModifier { get; private set; }

        private readonly IAttackableUnit target = Spells.FizzPiercingStrike._target;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var owner = ownerSpell.CastInfo.Owner;
            var time = 0.6f - ownerSpell.CastInfo.SpellLevel * 0.1f;
            var damage = 50f + ownerSpell.CastInfo.SpellLevel * 20f + unit.Stats.AbilityPower.Total * 0.6f;
            
            AddParticleTarget(owner, target, "HeadButt_tar.troy", target);
            var to = Vector2.Normalize(target.Position - unit.Position);
            ForceMovement(unit, null, new Vector2(target.Position.X + to.X * 250f, target.Position.Y + to.Y * 250f), 800f + unit.Stats.MoveSpeed.Total * 0.6f, 0, 0, 0); ; ;
            target.TakeDamage(unit, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {

        }

        public void OnUpdate(float diff)
        {

        }
    }
}
