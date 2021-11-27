using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class CarpetBomb : ISpellScript
    {
	public ISpellSector DamageSector;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            CastingBreaksStealth = true,
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
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
            var owner = spell.CastInfo.Owner;
            var trueCoords = GetPointFromUnit(owner, 800f);
            var semiCoords = GetPointFromUnit(owner, 400f);
            ForceMovement(owner, "Spell2", trueCoords, 600, 0, 0, 0);
            var spellpos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);

	    AddParticle(owner, null, "corki_valkrie_speed.troy", owner.Position, lifetime: 5.0f , reqVision: false);
	    AddParticle(owner, null, "corki_valkrie_impact_cas.troy", owner.Position, lifetime: 5.0f , reqVision: false);
       AddParticle(owner, null, "corki_valkrie_speed.troy", semiCoords, lifetime: 5.0f , reqVision: false);
	    AddParticle(owner, null, "corki_valkrie_impact_cas.troy", semiCoords, lifetime: 5.0f , reqVision: false);
       AddParticle(owner, null, "corki_valkrie_speed.troy", trueCoords, lifetime: 5.0f , reqVision: false);
	    AddParticle(owner, null, "corki_valkrie_impact_cas.troy", trueCoords, lifetime: 5.0f , reqVision: false);
       
            DamageSector = spell.CreateSpellSector(new SectorParameters
            {
                
                Length = 800f,
                Width = 100f,
                PolygonVertices = new Vector2[]
                {
                    // Basic square, however the values will be scaled by Length/Width, which will make it a rectangle
                    new Vector2(-1, 0),
                    new Vector2(-1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                },
		Tickrate = 2,
                CanHitSameTargetConsecutively = true,
		OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                Type = SectorType.Polygon,
		Lifetime = 5.0f
            });
        }

        public void OnSpellChannel(ISpell spell)
        {
        }
	  public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {   
             var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.2f;
            var damage = 15 + (15 * spell.CastInfo.SpellLevel ) + ap;

            target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
           

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
