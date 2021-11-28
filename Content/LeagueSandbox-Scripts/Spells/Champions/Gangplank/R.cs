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
using System;

namespace Spells
{
    public class CannonBarrage : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.2f;
            var damage = 75 + (45 * (spell.CastInfo.SpellLevel - 1)) + ap;
            target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
            AddBuff("GangplankR",1.25f, 1, spell, target, spell.CastInfo.Owner);
            var particle = AddParticlePos(spell.CastInfo.Owner, "pirate_cannonBarrage_tar.troy", target.Position, target.Position, lifetime: 1.0f);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {

            owner.StopMovement();

            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var mushroom = AddMinion(owner, "TeemoMushroom", "TeemoMushroom", spellPos);

            var Champs = GetChampionsInRange(owner.Position, 50000, true);
            foreach (IChampion player in Champs)
            {
                mushroom.SetInvisible((int)player.GetPlayerId(), mushroom, 0f, 0f);
                mushroom.SetHealthbarVisibility((int)player.GetPlayerId(), mushroom, false);
            }
            mushroom.SetCollisionRadius(0.0f);
            mushroom.SetStatus(StatusFlags.Targetable, false);
            mushroom.SetStatus(StatusFlags.Ghosted, true);

            var sec = spell.CreateSpellSector(new SectorParameters
            {
                BindObject = mushroom,
                Length = 600f,
                Tickrate = 1,
                CanHitSameTargetConsecutively = true,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                Type = SectorType.Area
            });
            var particle = AddParticlePos(owner, "pirate_cannonBarrage_aoe_indicator_green.troy", spellPos, spellPos, lifetime: 7.0f);
            var i = 1.0f;
            while(i <= 7.0f)
            {
                CreateTimer(i, () => 
                {
                    var randOffsetX = (float)new Random().Next(-400, 400);
                    var randOffsetY = (float)new Random().Next(-400, 400);

                    var randOffsetX2 = (float)new Random().Next(-400, 400);
                    var randOffsetY2 = (float)new Random().Next(-400, 400);

                    var randOffsetX3 = (float)new Random().Next(-400, 400);
                    var randOffsetY3 = (float)new Random().Next(-400, 400);

                    var randPoint = new Vector2(spellPos.X + randOffsetX, spellPos.Y + randOffsetY);
                    var randPoint2 = new Vector2(spellPos.X + randOffsetX2, spellPos.Y + randOffsetY2);
                    var randPoint3 = new Vector2(spellPos.X + randOffsetX3, spellPos.Y + randOffsetY3);

                    var particle1 = AddParticlePos(owner, "pirate_cannonBarrage_point.troy", randPoint, randPoint, lifetime: 1.0f);
                    var particle2 = AddParticlePos(owner, "pirate_cannonBarrage_point.troy", randPoint2, randPoint2, lifetime: 1.0f);
                    var particle3 = AddParticlePos(owner, "pirate_cannonBarrage_point.troy", randPoint3, randPoint3, lifetime: 1.0f);
                });
                LogDebug(i.ToString());
                i++;
            }
            CreateTimer(7.0f, () => { mushroom.TakeDamage(owner, 50000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false); sec.SetToRemove(); });
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
