using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class VayneSilveredBolts : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
        };
        IObjAiBase own;
        ISpell spl;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnLevelUpSpell.AddListener(this, spell, HideW, false);
            ApiEventManager.OnHitUnit.AddListener(this, owner, TargetExecute, false);
            own = owner;
            spl = spell;
        }
        IAttackableUnit AttackedUnit;
        private int _silverBoltsStacks;

        IParticle x;
        IParticle y;

        public void HideW(ISpell spell)
        {
            CreateTimer((float)0.1, () => { SealSpellSlot(own, SpellSlotType.SpellSlots, 1, SpellbookType.SPELLBOOK_CHAMPION, true); });
        }

        public void TargetExecute(IAttackableUnit unit, bool arg2)
        {

            if(AttackedUnit == null)
            {
                AttackedUnit = unit;
            }

            if(AttackedUnit != unit)
            {
                RemoveParticle(x);
                RemoveParticle(y);
                _silverBoltsStacks = 0;
                AttackedUnit = unit;
            }

            if (AttackedUnit == unit)
            {

                _silverBoltsStacks += 1;

                if (_silverBoltsStacks == 1)
                {
                    x = AddParticleTarget(own, AttackedUnit, "vayne_W_ring1.troy", AttackedUnit, lifetime: float.MaxValue);
                }
                else if (_silverBoltsStacks == 2)
                {
                    y = AddParticleTarget(own, AttackedUnit, "vayne_W_ring2.troy", AttackedUnit, lifetime: float.MaxValue);
                }
                else
                {
                    _silverBoltsStacks = 0; // We're at 3 stacks. Apply damage and reset to zero.
                    float healthRatio = (new float[] { 0.04f, 0.05f, 0.06f, 0.07f, 0.08f }[spl.CastInfo.SpellLevel - 1]) * AttackedUnit.Stats.HealthPoints.Total;
                    float damage = new float[] { 20, 30, 40, 50, 60 }[spl.CastInfo.SpellLevel - 1] + healthRatio;
                    AttackedUnit.TakeDamage(own, damage, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_RAW, false);
                    AddParticleTarget(own, AttackedUnit, "vayne_W_tar.troy", AttackedUnit);
                    RemoveParticle(x);
                    RemoveParticle(y);
                    AttackedUnit = null;

                }
            }
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
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
