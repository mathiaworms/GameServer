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
    public class CaitlynYordleTrap : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, ExecuteSpell, false);
        }

        public void ExecuteSpell(ISpell spell, IAttackableUnit unit, ISpellMissile mis, ISpellSector sector)
        {
            LogDebug("HIT");
            LogDebug(unit.Model.ToString());
            LogDebug(spell.SpellName.ToString());
            unit.Stats.SetActionState(ActionState.CAN_MOVE, false);
            unit.StopMovement();
            CreateTimer(0.5f, () => { unit.StopMovement(); });
            CreateTimer(0.25f, () => { unit.StopMovement(); });
            CreateTimer(1.5f, () => { unit.Stats.SetActionState(ActionState.CAN_MOVE, true); });
            sector.SetToRemove();
            m1.TakeDamage(unit, 5000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false);
            m1 = null;
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

        public IAttackableUnit unit;
        private IMinion m1;
        private ISpellSector sec;

        public void OnSpellPostCast(ISpell spell)
        {
            var spellLVL = spell.CastInfo.SpellLevel;
            var owner = spell.CastInfo.Owner;
            unit = owner;
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            if (m1 == null)
            {
                m1 = AddMinion((IChampion)owner, "CaitlynTrap", "CaitlynTrap", spellPos);
                m1.SetStatus(StatusFlags.Ghosted, true);
                m1.SetStatus(StatusFlags.Targetable, false);
                m1.IsVisibleByTeam(owner.Team);
                sec = spell.CreateSpellSector(new SectorParameters
                {
                    Length = 15f,
                    Tickrate = 100,
                    CanHitSameTargetConsecutively = true,
                    OverrideFlags = SpellDataFlags.AffectHeroes | SpellDataFlags.AffectEnemies | SpellDataFlags.IgnoreEnemyMinion,
                    //OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                    Type = SectorType.Area
                });
            }
            else
            {
                if (m1 != null)
                {
                    m1.TakeDamage(unit, 5000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false);
                    m1 = null;
                    m1 = AddMinion((IChampion)owner, "CaitlynTrap", "CaitlynTrap", spellPos);
                    m1.SetStatus(StatusFlags.Ghosted, true);
                    m1.SetStatus(StatusFlags.Targetable, false);
                    m1.IsVisibleByTeam(owner.Team);
                    sec.SetToRemove();
                    sec = spell.CreateSpellSector(new SectorParameters
                    {
                        Length = 15f,
                        Tickrate = 100,
                        CanHitSameTargetConsecutively = true,
                        OverrideFlags = SpellDataFlags.AffectHeroes | SpellDataFlags.AffectEnemies | SpellDataFlags.IgnoreEnemyMinion,
                        //OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                        Type = SectorType.Area
                    });
                }
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
            //if (procced = true)
            //{
            //    m.TakeDamage(unit, 5000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false);
            //}
        }
    }
}