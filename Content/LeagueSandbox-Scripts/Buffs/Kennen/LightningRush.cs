using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Missile;
namespace Buffs
{
    class KennenLightningRush : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;
        IObjAiBase Owner;
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();
        float DamageManaTimer;
        IAttackableUnit owner;
        ISpell originSpell;
        IBuff thisBuff;
        IParticle red;
        IParticle green;

        float SlowTimer;
        public ISpellSector AuraKennen1;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = unit;
            originSpell = ownerSpell;
            thisBuff = buff;


            red = AddParticleTarget(owner, unit, "kennen_lr_buf.troy", unit, buff.Duration); //Take a look at whi the particles disapear later
            green = AddParticleTarget(owner, unit, "Kennen_lr_tar.troy", unit, buff.Duration);

            StatsModifier.MoveSpeed.PercentBonus += 1.0f;
            unit.AddStatModifier(StatsModifier);

            ApiEventManager.OnSpellHit.AddListener(this, ownerSpell, TargetExecute, false);



            AuraKennen1 = ownerSpell.CreateSpellSector(new SectorParameters
            {
                BindObject = ownerSpell.CastInfo.Owner,
                Length = 80f,
                Tickrate = 1,
                CanHitSameTargetConsecutively = false,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                Type = SectorType.Area
            });

        }
        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {

            float ap = Owner.Stats.AbilityPower.Total * 0.2f;
            var damage = 5.0f + (35f * spell.CastInfo.SpellLevel) + ap;
            target.TakeDamage(Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);


            if (target.GetBuffWithName("KennenMarkOfStorm").StackCount == 3) //remove mos if stacks reach 3
            {
                target.RemoveBuffsWithName("KennenMarkOfStorm");
                AddBuff("Stun", 1.5f, 1, spell, target, Owner); //stun target for 1 second after 3 stacks

            }
            else
            {
                AddBuff("KennenMarkOfStorm", 6f, 1, spell, target, Owner);
            }



        }
        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {

            ApiEventManager.OnSpellHit.RemoveListener(this);
            RemoveParticle(red);
            RemoveParticle(green);
            AuraKennen1.SetToRemove();


        }

        public void OnUpdate(float diff)
        {


        }
    }
}
