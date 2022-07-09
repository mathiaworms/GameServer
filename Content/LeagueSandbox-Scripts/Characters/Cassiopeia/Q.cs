using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    public class CassiopeiaNoxiousBlast : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            CastingBreaksStealth = true,
            DoesntBreakShields = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true,
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            _owner = owner;
            _spell = spell;
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public ISpellSector DamageSector;

        IObjAiBase _owner;
        ISpell _spell;

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            //AddBuff("CassiopeiaDeadlyCadence", float.MaxValue, 1, spell, owner, owner, true);
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            var targetPos = GetPointFromUnit(owner, 850.0f);
            SpellCast(owner, 0, SpellSlotType.ExtraSlots, targetPos, targetPos, false, Vector2.Zero);
            var spellpos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);

            AddParticle(owner, null, "Cassiopeia_Base_Q_Hit_Green.troy", spellpos, lifetime: 0.5f);
            AddParticle(owner, null, "Cassiopeia_Base_Q_Hit_Red.troy", spellpos, lifetime: 0.5f);
            AddParticle(owner, null, "CassNoxiousBlast_cas.troy", spellpos, lifetime: 0.5f);
            AddParticle(owner, null, "CassNoxiousBlast_glow.troy", spellpos, lifetime: 0.5f);
            AddParticle(owner, null, "CassNoxiousBlast_tar.troy", spellpos, lifetime: 0.5f);
            DamageSector = spell.CreateSpellSector(new SectorParameters
            {
                Length = 75f,
                Tickrate = 2,
                CanHitSameTargetConsecutively = false,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                Type = SectorType.Area,
                Lifetime = 0.5f
            });
        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;

            AddBuff("CassiopeiaPoisonTicker", 4f, 1, spell, target, owner);
            if (target is IChampion)
            {
                AddBuff("CassiopeiaNoxiousBlastHaste", 3f, 1, spell, owner, owner);
            }
        }

        public void OnSpellChannelCancel(ISpell spell, ChannelingStopSource source)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
            if (_owner.HasBuff("CassiopeiaDeadlyCadence")){
                var sc = _owner.GetBuffWithName("CassiopeiaDeadlyCadence").StackCount;
                if(sc >= 100)
                {
                    if (!_owner.HasBuff("Passive100"))
                    {
                        AddBuff("Passive100", 5.0f, 1, _spell, _owner, _owner, false);
                    }
                }
                if (sc >= 250)
                {
                    if (!_owner.HasBuff("Passive250"))
                    {
                        AddBuff("Passive250", 5.0f, 1, _spell, _owner, _owner, false);
                    }
                }
                if (sc >= 500)
                {
                    if (!_owner.HasBuff("Passive500"))
                    {
                        AddBuff("Passive500", 5.0f, 1, _spell, _owner, _owner, false);
                    }
                }
            }
        }
    }
}