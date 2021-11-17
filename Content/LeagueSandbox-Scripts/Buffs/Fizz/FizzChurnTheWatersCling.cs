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
    class FizzChurnTheWatersCling : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;

        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IObjAiBase Owner;
        ISpell Spell;
        IAttackableUnit Target;
        IMinion Fish;
        float ticks = 0;
        float damage;
        float true1 = 0;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            Target = unit;
            Owner = ownerSpell.CastInfo.Owner;
            Spell = ownerSpell;
            var APratio = Owner.Stats.AbilityPower.Total * 0.2f;
            damage = 24f + (14f * (ownerSpell.CastInfo.SpellLevel - 1)) + APratio;

            StatsModifier.MoveSpeed.PercentBonus -= 0.1f + 0.1f * ownerSpell.CastInfo.SpellLevel;
            StatsModifier.AttackSpeed.PercentBonus -= 0.25f;
            unit.AddStatModifier(StatsModifier);

            var units = GetUnitsInRange(Target.Position, 350f, true);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {


        }

        

        public void OnPreAttack(ISpell spell)
        {

        }

        public void OnUpdate(float diff)
        {
            var owner = Spell.CastInfo.Owner;

            var targetPos = GetPointFromUnit(owner, 1150f);

            ticks += diff;
            if (true1 == 0)
            {
                if (ticks >= 1450.0f && ticks <= 1500.0f)
                {
                    var units = GetUnitsInRange(Target.Position, 350f, true);
                    for (int i = units.Count - 1; i >= 0; i--)
                    {
                        if (units[i].Team != Spell.CastInfo.Owner.Team && !(units[i] is IObjBuilding || units[i] is IBaseTurret) && units[i] is IObjAiBase ai)
                        {
                            units[i].TakeDamage(Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                            units.RemoveAt(i);
                        }
                    }
                    IMinion Fish = AddMinion(owner, "FizzShark", "FizzShark", Target.Position, owner.SkinID, false, false);
                    Fish.TakeDamage(Fish.Owner, 10000f, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, DamageResultType.RESULT_NORMAL);
                    Fish.PlayAnimation("SPELL4", 1.0f);
                    Fish.SetToRemove();
                    AddParticleTarget(Owner, Target, "Fizz_SharkSplash.troy", Target);
                    AddParticleTarget(Owner, Target, "Fizz_SharkSplash_Ground.troy", Target);
                    true1 = 1; 

                }
            }
            
        }
    }
}