using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain;


namespace Buffs
{
    internal class NasusQAttack : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IBuff thisBuff;
        IObjAiBase Unit;
        IParticle p;
        IParticle p2;
        IObjAiBase Owner;
        ISpell spelll;
        IDeathData killerunit;
        IAttackableUnit AtOwner;
        bool cangatherstacks = false;
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            spelll = ownerSpell;
            Owner = ownerSpell.CastInfo.Owner;
            AtOwner = ownerSpell.CastInfo.Owner;
            thisBuff = buff;
            if (unit is IObjAiBase ai)
            {
                Unit = ai;

                ApiEventManager.OnHitUnit.AddListener(this, ai, TargetExecute, true);
                ai.CancelAutoAttack(true);

                p = AddParticleTarget(ownerSpell.CastInfo.Owner, ownerSpell.CastInfo.Owner, "Nasus_Base_Q_Buf.troy", unit, buff.Duration, 1, "BUFFBONE_CSTM_WEAPON_1");
                p2 = AddParticleTarget(ownerSpell.CastInfo.Owner, ownerSpell.CastInfo.Owner, "Nasus_Base_Q_Wpn_trail.troy", unit, buff.Duration, 1, "BUFFBONE_CSTM_WEAPON_1");
                ai.SkipNextAutoAttack();
            }

            StatsModifier.Range.FlatBonus += 50;
            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            //ApiEventManager.OnHitUnit.RemoveListener(this);
            //ApiEventManager.OnDeath.RemoveListener(this);

            RemoveParticle(p);
            RemoveParticle(p2);
        }
        public void TargetExecute(IAttackableUnit target, bool Iscrit)
        {
            StopAnimation(Owner, "Attack1", true);
            PlayAnimation(Owner, "Spell1", 0.8f);
            AddParticleTarget(Owner, target, "Nasus_Base_Q_Tar.troy", target);
            if (!thisBuff.Elapsed() && thisBuff != null && Unit != null)
            {
                if ( Unit.HasBuff("NasusQStacks"))
                {       
                float StackDamage = Unit.GetBuffWithName("NasusQStacks").StackCount;
                    float ownerdamage = spelll.CastInfo.Owner.Stats.AttackDamage.Total;
                float damage = 15 + 25 * Unit.GetSpell(0).CastInfo.SpellLevel + StackDamage;
                float mitdamage = target.Stats.GetPostMitigationDamage(damage, DamageType.DAMAGE_TYPE_PHYSICAL, AtOwner);
                target.TakeDamage(Unit, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);

                if (target.Stats.CurrentHealth - mitdamage < 0 || target.IsDead)
                    {
                        AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                        AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                        AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                        if (target is IChampion)
                        {
                            AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                            AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                            AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                        }
                    }
                thisBuff.DeactivateBuff();

                }
                else 
                {
                    float damage = 15 + 25 * Unit.GetSpell(0).CastInfo.SpellLevel ;
                    float ownerdamage = spelll.CastInfo.Owner.Stats.AttackDamage.Total;
                    float mitdamage = target.Stats.GetPostMitigationDamage(damage, DamageType.DAMAGE_TYPE_PHYSICAL, AtOwner);
                    target.TakeDamage(Unit, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
                    if (target.Stats.CurrentHealth - mitdamage < 0 || target.IsDead)
                    {
                        AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                        AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                        AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                        if (target is IChampion)
                        {
                            AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                            AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                            AddBuff("NasusQStacks", 25000f, 1, spelll, AtOwner, Owner, true);
                        }
                    }
                thisBuff.DeactivateBuff();
                }
            }
        }


        public void OnUpdate(float diff)
        {

        }
    }
}

