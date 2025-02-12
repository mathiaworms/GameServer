using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    public class GalioRighteousGust : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        private ISpellMissile v;
        private IMinion mushroom;

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            ISpellScriptMetadata s = new SpellScriptMetadata()
            {
                TriggersSpellCasts = true,
                MissileParameters = new MissileParameters
                {
                    Type = MissileType.Circle,
                }
            };
            v = spell.CreateSpellMissile(s.MissileParameters);
            mushroom = AddMinion(owner, "TeemoMushroom", "TeemoMushroom", owner.Position);
            mushroom.SetStatus(StatusFlags.Ghosted, true);
            var Champs = GetAllChampionsInRange(owner.Position, 50000);
            var spellPos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);
            var x = AddParticle(owner, mushroom, "galio_windTunnel_mis.troy", owner.Position);
            var y = AddParticle(owner, mushroom, "galio_windTunnel_mis_02.troy", owner.Position);
            var z = AddParticle(owner, mushroom, "galio_windTunnel_mis.troy", owner.Position);
            float inte = 0.0f;
            float intinc = 0.01f;
            while (inte < 1.0f)
            {
                CreateTimer(inte, () => { mushroom.TeleportTo(v.Position.X, v.Position.Y); });
                inte += intinc;
            }
            CreateTimer(1.5f, () => { mushroom.TakeDamage(owner, 20000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false); });
            foreach (IChampion player in Champs)
            {
              //  mushroom.SetInvisible((int)player.GetPlayerId(), mushroom, 0f, 0f);
               // mushroom.SetHealthbarVisibility((int)player.GetPlayerId(), mushroom, false);
            }
            mushroom.SetCollisionRadius(0.0f);
            mushroom.SetStatus(StatusFlags.Targetable, false);
        }

        public void OnSpellCast(ISpell spell)
        {
            //AddParticleTarget(spell.CastInfo.Owner, spell.CastInfo.Owner, "galio_windTunnel_mis.troy", spell.CastInfo.Owner, 1f, bone: "L_HAND");
            //AddParticleTarget(spell.CastInfo.Owner, spell.CastInfo.Owner, "galio_windTunnel_mis_02.troy", spell.CastInfo.Owner, 1f, bone: "L_HAND");
            //AddParticleTarget(spell.CastInfo.Owner, spell.CastInfo.Owner, "galio_windTunnel_mis.troy", spell.CastInfo.Owner, 1f, bone: "L_HAND");
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            if (target.Team != spell.CastInfo.Owner.Team)
            {
                var owner = spell.CastInfo.Owner;
                var APratio = owner.Stats.AbilityPower.Total;
                var spelllvl = (spell.CastInfo.SpellLevel * 45);
                target.TakeDamage(owner, APratio + spelllvl + 25, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                var xy = target as IObjAiBase;
                xy.SetTargetUnit(null);
                ForceMovement(target, "RUN", new Vector2(target.Position.X + 5f, target.Position.Y + 5f), 13f, 0, 16.5f, 0);
                if (target is Champion)
                {
                    AddBuff("Pulverize", 1.0f, 1, spell, target, spell.CastInfo.Owner);
                }
            }
        }

        public void OnSpellPostCast(ISpell spell)
        {
            //var owner = spell.CastInfo.Owner as IChampion;
            //var trueCoords = GetPointFromUnit(owner, 1180f);
            //
            //SpellCast(owner, 1, SpellSlotType.ExtraSlots, trueCoords, trueCoords, false, Vector2.Zero);
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, ISpellMissile missile)
        {
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

    public class GalioRighteousGustMissile : ISpellScript
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

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var champion = target as IChampion;
            var owner = spell.CastInfo.Owner as IChampion;
            var spellLevel = owner.GetSpell("GalioRighteousGust").CastInfo.SpellLevel;
            if (champion == null)
            {
                return;
            }

            if (champion.Team == owner.Team && champion != owner)
            {
                AddBuff("GalioRighteousGustHaste", 5f, 1, spell, champion, owner);
            }
            else if (champion == owner) //TODO: Fix getting self proc at cast (you are supposed to have to E/Flash into it in order to get the buff i think)
            {
                AddBuff("GalioRighteousGustHaste", 5f, 1, spell, champion, owner);
            }
            else
            {
                var APratio = owner.Stats.AbilityPower.Total * 0.5f;
                var damage = 15 + (45 * spellLevel) + APratio;

                champion.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            }
            AddParticleTarget(owner, champion, "galio_windTunnel_unit_tar.troy", champion, lifetime: 1f);
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
}