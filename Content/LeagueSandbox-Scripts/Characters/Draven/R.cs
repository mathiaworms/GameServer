using GameServerCore;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Collections.Generic;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    public class DravenRCast : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
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
        static internal bool toggle = true;
        internal static ISpellMissile mis;
        public void OnSpellPostCast(ISpell spell)
        {
            toggle = !toggle;
            if (toggle == false)
            {
                var endPos = GetPointFromUnit(spell.CastInfo.Owner, 925);
                SpellCast(spell.CastInfo.Owner, 1, SpellSlotType.ExtraSlots, endPos, endPos, false, Vector2.Zero);
                spell.SetCooldown(0.0f, true);
            }
            if (toggle == true)
            {
                var misPos = mis.Position;
                mis.SetToRemove();
                SpellCast(spell.CastInfo.Owner, 1, SpellSlotType.ExtraSlots, spell.CastInfo.Owner.Position, spell.CastInfo.Owner.Position, true, misPos);
            }
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

    public class DravenR : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            }
            // TODO
        };
        IObjAiBase _owner;
        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            _owner = owner;
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
            ApiEventManager.OnLaunchMissileByAnother.AddListener(this, new KeyValuePair<IObjAiBase, ISpell>(owner, spell), CastSpell, false);
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

        public void CastSpell(ISpell spell, ISpellMissile missile)
        {
            DravenRCast.mis = missile;
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var ad = owner.Stats.AttackDamage.Total * 1.1f + (spell.CastInfo.Owner.GetSpell(3).CastInfo.SpellLevel * 100) + 40;
            target.TakeDamage(owner, (float)ad, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
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
            if (DravenRCast.mis != null)
            {
                if (DravenRCast.toggle == true)
                {
                    if (Extensions.IsVectorWithinRange(_owner.Position, DravenRCast.mis.Position, 100))
                    {
                        DravenRCast.mis.SetToRemove();
                    }
                }
                //LogDebug("yo");
            }
        }

    }
}