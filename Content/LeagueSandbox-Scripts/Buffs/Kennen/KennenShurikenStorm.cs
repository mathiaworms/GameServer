using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
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
using GameServerCore.Domain.GameObjects.Spell.Sector;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Missile;

namespace Buffs
{
    class KennenShurikenStorm : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;
        IObjAiBase Owner;
        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IAttackableUnit owner;
        ISpell originSpell;
        IBuff thisBuff;
        IParticle red;
        IParticle green;
        float DamageManaTimer;
        float SlowTimer;
        public ISpellSector AuraKennen;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            Owner = ownerSpell.CastInfo.Owner;
            originSpell = ownerSpell;
            thisBuff = buff;

            var spellPos = owner.Position;


             red = AddParticleTarget(owner, unit, "kennen_ss_aoe_red.troy", unit, buff.Duration); //Take a look at whi the particles disapear later
             green = AddParticleTarget(owner, unit, "kennen_ss_aoe_green.troy", unit, buff.Duration);
            StatsModifier.Armor.FlatBonus += 20.0f * ownerSpell.CastInfo.SpellLevel ;
            StatsModifier.MagicResist.FlatBonus += 20.0f * ownerSpell.CastInfo.SpellLevel ;
            unit.AddStatModifier(StatsModifier);

             ApiEventManager.OnSpellHit.AddListener(this, ownerSpell, TargetExecute, false);



            AuraKennen = ownerSpell.CreateSpellSector(new SectorParameters
            {
                BindObject = ownerSpell.CastInfo.Owner,
                Length = 550f,
                Tickrate = 1,
                CanHitSameTargetConsecutively = true,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                Type = SectorType.Area
            });

        }
        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {   
                        var ap = Owner.Stats.AbilityPower.Total * 0.2f;
                           var damage = 5.0f + (35f * spell.CastInfo.SpellLevel ) + ap;
                           target.TakeDamage(Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);

                       
                    if(target.GetBuffWithName("KennenMarkOfStorm").StackCount == 3) //remove mos if stacks reach 3
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
        AuraKennen.SetToRemove();

        }

        public void OnUpdate(float diff)
        {




        }
    }
}
