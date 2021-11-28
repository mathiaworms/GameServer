using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class VolibearQ : ISpellScript
    {

        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            NotSingleTargetSpell = true
            // TODO
        };
        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnPreAttack.AddListener(this, owner, ChangeAnim, false);
        }

        public void ChangeAnim(ISpell spell)
        {
            if (VolibearQAttack.Applied == 0)
            {
                spell.CastInfo.Owner.PlayAnimation("Spell2", 0.5f, flags: AnimationFlags.Override);
                CreateTimer(0.5f, () => { spell.CastInfo.Owner.StopAnimation("Spell1", fade: true); });
            }
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            //var owner = spell.CastInfo.Owner as IChampion;
            LogDebug("yo");
            AddBuff("VoliQBuffSwag", 6.0f, 1, spell, owner, owner);
        }

        public void OnSpellCast(ISpell spell)
        {

        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }

    }
    public class VolibearQAttack : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = false,
            NotSingleTargetSpell = true
            // TODO
        };

        ISpell originspell;
        IObjAiBase ownermain;
        static internal int Applied = 1;
        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            originspell = spell;
            ownermain = owner;
            ApiEventManager.OnHitUnit.AddListener(this, owner, TargetExecute, false);
        }

        public void TargetExecute(IAttackableUnit unit, bool arg2)
        {
            var owner = ownermain;
            
            var ADratio = owner.Stats.AttackDamage.PercentBonus * 0.3f;
            var damage = 40f + (30f * (originspell.CastInfo.SpellLevel - 1)) + ADratio;
            if (Applied != 1)
            {
                var x = GetPointFromUnit(owner, -200);
                unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                ForceMovement(unit, "Spell1", x, 500, 0, 20, 0, GameServerCore.Enums.ForceMovementType.FIRST_WALL_HIT);
                Applied = 1;
                //CreateTimer((float)6, () => { Applied = 1; });
            }
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            Applied = 0;
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}

