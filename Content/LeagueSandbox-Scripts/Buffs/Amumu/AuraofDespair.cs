using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    class AuraofDespair : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IAttackableUnit owner;
        ISpell originSpell;
        IBuff thisBuff;
        IParticle red;
        IParticle green;
        float DamageManaTimer;
        float SlowTimer;
        float manaCost = 8.0f;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            owner = unit;
            originSpell = ownerSpell;
            thisBuff = buff;

            var spellPos = owner.Position;

            originSpell.SetCooldown(1.0f, true);

            SetTargetingType((IObjAiBase)unit, SpellSlotType.SpellSlots, 3, TargetingType.Self);
          
               red = AddParticle(owner, null, "Despairpool_tar.troy", spellPos, lifetime: buff.Duration, reqVision: false);

        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            ownerSpell.SetCooldown(1.0f);

            SetTargetingType((IObjAiBase)unit, SpellSlotType.SpellSlots, 3, TargetingType.Area);

            RemoveParticle(red);


            if (ownerSpell.Script is Spells.AuraofDespair spellScript)
            {
                spellScript.DamageSector.ExecuteTick();
                spellScript.DamageSector.SetToRemove();

            }
        }

        public void OnUpdate(float diff)
        {



            if (owner != null && thisBuff != null && originSpell != null)
            {
                DamageManaTimer += diff;

                if (DamageManaTimer >= 500f)
                {
                    if (manaCost > owner.Stats.CurrentMana)
                    {
                        RemoveBuff(thisBuff);
                    }
                    else
                    {
                        owner.Stats.CurrentMana -= manaCost;

                    }

                    DamageManaTimer = 0;
                }
           

                

            }
        }
    }
}
