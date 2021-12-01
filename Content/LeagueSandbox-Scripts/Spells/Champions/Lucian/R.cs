using System;
using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class LucianR : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            //var spawnPos = GetPointFromUnit(owner, 100);
            //var m = AddMinion(owner, "TeemoMushroom", "TeemoMushroom", spawnPos);
            //owner.SetTargetUnit(m);
            //PlayAnimation(owner, "Spell1");

            AddBuff("LucianPassiveBuff", 3.5f, 1, spell, spell.CastInfo.Owner, spell.CastInfo.Owner);

            owner.CancelAutoAttack(true);
            SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero);
            CreateTimer(0.1f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(0.2f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(0.3f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(0.4f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(0.5f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(0.6f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(0.7f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(0.8f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(0.9f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(1.0f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(1.1f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(1.2f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(1.3f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(1.4f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(1.5f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(1.6f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(1.7f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(1.8f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(1.9f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(2.0f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(2.1f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(2.2f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(2.3f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(2.4f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(2.5f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(2.6f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(2.7f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(2.8f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(2.9f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            CreateTimer(3.0f, () => { SpellCast(owner, 1, SpellSlotType.ExtraSlots, spellPos, spellPos, true, Vector2.Zero); });
            owner.StopMovement();

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
    public class LucianRMissile : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            },
            IsDamagingSpell = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        private void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector arg4)
        {
            var owner = spell.CastInfo.Owner;
            missile.SetToRemove();
            var ad = owner.Stats.AttackDamage.Total * 0.08;
            float damage = (float)(ad + 30 + spell.CastInfo.SpellLevel * 10);

            LogDebug(damage.ToString());
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
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
