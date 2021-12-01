using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;
using System.Linq;
using GameServerCore;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Domain.GameObjects.Spell.Sector;
namespace Spells
{
    public class AhriTumble : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
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

        public void OnSpellPostCast(ISpell spell)
        {
            var current = new Vector2(spell.CastInfo.Owner.Position.X, spell.CastInfo.Owner.Position.Y);
            var trueCoords = GetPointFromUnit(spell.CastInfo.Owner, spell.SpellData.CastRangeDisplayOverride);

            CreateTimer(0.5f, () =>
            {
                var units = GetUnitsInRange(spell.CastInfo.Owner.Position, 500, true).Where(x => x.Team == CustomConvert.GetEnemyTeam(spell.CastInfo.Owner.Team));
                var i = 0;
                foreach (var allyTarget in units)
                {
                    if (allyTarget is IAttackableUnit && spell.CastInfo.Owner != allyTarget)
                    {
                        if (i < 1)
                        {
                            SpellCast(spell.CastInfo.Owner, 5, SpellSlotType.ExtraSlots, true, allyTarget, Vector2.Zero);
                            i++;
                   
                        }

                    }
                }
            });


            FaceDirection(current, spell.CastInfo.Owner, true);
            CreateTimer(0.01f, () => { ForceMovement(spell.CastInfo.Owner, "Spell4", trueCoords, 1500, 0, 0, 0); });

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

    
     public class AhriTumbleMissile : ISpellScript
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

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
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

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var ap = owner.Stats.AbilityPower.Total * 0.35;
            var damage = ap + 30 + 30 * owner.Spells[0].CastInfo.SpellLevel;
            target.TakeDamage(owner, (float)damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            missile.SetToRemove();
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
