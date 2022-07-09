using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    public class Drain : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        private Vector2 basepos;
        private bool cancelled;
        private IParticle p;

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            basepos = owner.Position;
            PlayAnimation(owner, "Spell2", startTime: 0.5f);
            p = AddParticleTarget(owner, target, "Drain.troy", owner, lifetime: 5.0f);
            for (var i = 0.0f; i < 5.0; i += 1f)
            {
                CreateTimer(i, () => { ApplyDrainDamage(owner, spell, target); });
            }
            CreateTimer(5.1f, () => { cancelled = false; });
        }

        private void ApplyDrainDamage(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            if (cancelled == true)
            {
                RemoveParticle(p);
            }
            if (owner.Position.X != basepos.X)
            {
                cancelled = true;
            }
            if (owner.Position.Y != basepos.Y)
            {
                cancelled = true;
            }
            if (target.Team != owner.Team)
            {
                var damage = 60;
                var ap = owner.Stats.AbilityPower.Total * 0.45f;
                if (!cancelled)
                {
                    target.TakeDamage(owner, ap + damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                }
            }
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

        public void OnSpellChannelCancel(ISpell spell, ChannelingStopSource source)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
            if (cancelled == true)
            {
                p.SetToRemove();
            }
        }
    }
}