using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{

    public class LucianPassiveShot : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Target
            }
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        private void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector arg4)
        {
            var owner = spell.CastInfo.Owner;
            missile.SetToRemove();
            var ad = owner.Stats.AttackDamage.Total;
            target.TakeDamage(owner, ad / 4, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnLaunchAttack(ISpell spell)
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

    public class LucianBasicAttack : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnLaunchAttack.AddListener(this, owner, OnLaunchAttack, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnLaunchAttack(ISpell spell)
        {
            LogDebug("swag");
            if (spell.CastInfo.Targets[0].Unit.HasBuff("LucianWDebuff"))
            {
                AddBuff("LucianWBuff", 2.0f, 1, spell, spell.CastInfo.Owner, spell.CastInfo.Owner);
            }
            if (spell.CastInfo.Owner.HasBuff("LucianPassiveBuff"))
            {
                CreateTimer(0.25f, () => { SpellCast(spell.CastInfo.Owner, 6, SpellSlotType.ExtraSlots, true, spell.CastInfo.Targets[0].Unit, Vector2.Zero); });
                CreateTimer(0.25f, () => { PlayAnimation(spell.CastInfo.Owner, "ATTACK1", flags: AnimationFlags.Override); });
                spell.CastInfo.Owner.RemoveBuffsWithName("LucianPassiveBuff");
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
        }
    }
}