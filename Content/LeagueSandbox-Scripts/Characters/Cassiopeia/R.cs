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
    public class CassiopeiaPetrifyingGaze : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
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
            var owner = spell.CastInfo.Owner;

            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            FaceDirection(spellPos, owner, false);

            spell.CreateSpellSector(new SectorParameters
            {
                Length = 750f,
                SingleTick = true,
                ConeAngle = 40f,
                Type = SectorType.Cone
            });
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var hitFacing = false;
            var point = GetPointFromUnit(target, 825);
            var champs = GetChampionsInRange(point, 825, true);

            var owner = spell.CastInfo.Owner;
            var ap = owner.Stats.AbilityPower.Total * 0.5f;
            var damage = 50 + spell.CastInfo.SpellLevel * 100 + ap;

            foreach (var champion in champs)
            {
                if (champion.NetId == spell.CastInfo.Owner.NetId)
                { 
                    AddParticleTarget(owner, target, "Cassiopeia_Base_R_tar.troy", target);
                    AddParticleTarget(owner, target, "CassDeadlyCadence_buf.troy", target, lifetime: 2f, bone: "C_BUFFBONE_GLB_HEAD_LOC");
                    AddParticleTarget(owner, target, "CassDeathDust.troy", target, lifetime: 2f, bone: "root");
                    AddParticleTarget(owner, target, "Cassiopeia_Base_R_PetrifyMiss_tar.troy", target, lifetime: 2f, bone: "C_BUFFBONE_GLB_HEAD_LOC");
                    target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    AddBuff("Stun", 2f, 1, spell, target, owner);
                    hitFacing = true;
                }
            }

            if (hitFacing == false)
            {
                AddParticleTarget(owner, target, "Cassiopeia_Base_R_tar.troy", target);
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                AddBuff("CassiopeiaSlow", 2f, 1, spell, target, owner);
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
}
