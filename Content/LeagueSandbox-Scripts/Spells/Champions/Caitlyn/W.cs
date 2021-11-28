using GameServerCore;
using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Enums;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;

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
        IMinion m1;
        ISpellSector sec;
        public void OnSpellPostCast(ISpell spell)
        {

            var spellLVL = spell.CastInfo.SpellLevel;
            var owner = spell.CastInfo.Owner;
            unit = owner;
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            if(m1 == null)
            {
                m1 = AddMinion((IChampion)owner, "CaitlynTrap", "CaitlynTrap", spellPos);
                m1.SetStatus(StatusFlags.Ghosted, true);
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

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }
        bool procced;
        public void OnUpdate(float diff)
        {
           //if (procced = true)
           //{
           //    m.TakeDamage(unit, 5000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false);
           //}
        }
    }
}
