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
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;

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
        Vector2 basepos;
        bool cancelled;
        IParticle p;
        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            basepos = owner.Position;
            PlayAnimation(owner, "Spell2", startTime: 0.5f);
            p = AddParticleTarget(owner, target, "Drain.troy", owner, lifetime: 5.0f);
            for (var i = 0.0f; i < 2.5; i += 0.25f)
            {
                CreateTimer(i, () => { ApplySpinDamage(owner, spell, target); });
            }
            CreateTimer(5.5f, () => { cancelled = false; });
        }

        private void ApplySpinDamage(IObjAiBase owner, ISpell spell, IAttackableUnit target)
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
                var damage = 35.0f;
                var ap = owner.Stats.AbilityPower.Total * 0.25f;
                var ad = owner.Stats.AttackDamage.Total * 0.37f;
                if (target is Minion) damage *= 0.75f;
                if (!cancelled)
                {
                    target.TakeDamage(owner, ap + ad + damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
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

        public void OnSpellChannelCancel(ISpell spell)
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
