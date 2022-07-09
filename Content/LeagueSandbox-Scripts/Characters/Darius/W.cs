using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    public class DariusNoxianTacticsONH : ISpellScript
    {
        private IObjAiBase Owner;

        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            Owner = owner;
            ApiEventManager.OnPreAttack.AddListener(this, owner, ChangeAnim, false);
            ApiEventManager.OnHitUnitByAnother.AddListener(this, owner, TargetExecute, false);
        }

        public void ChangeAnim(ISpell spell)
        {
            if (Applied == 0)
            {
                spell.CastInfo.Owner.PlayAnimation("Spell2", 1.5f, flags: AnimationFlags.Override);
                CreateTimer(0.5f, () => { spell.CastInfo.Owner.StopAnimation("Spell2", fade: true); });
            }
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            Applied = 0;
        }

        internal static int Applied = 1;

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            AddBuff("DariusNoxianTacticsActive", 4f, 1, spell, Owner, Owner, false);
        }

        public void TargetExecute(IAttackableUnit unit, bool arg2)
        {
            var owner = Owner;
            var ADratio = owner.Stats.AttackDamage.PercentBonus * 0.3f;
            LogDebug("yo");
            if (Applied != 1)
            {
                //unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                Applied = 1;
                //CreateTimer((float)6, () => { Applied = 1; });
            }
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell, ChannelingStopSource source)
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