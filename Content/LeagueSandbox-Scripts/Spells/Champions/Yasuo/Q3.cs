using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;

namespace Spells
{
    public class YasuoQ3W : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }
        ISpellMissile v;
        IMinion mushroom;
        bool destroy = false;
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

            if (owner.HasBuff("YasuoEFIX"))
            {
                CreateTimer(0.45F, () =>
                {
                    owner.PlayAnimation("Spell3A", 0.5f, 0, 1);
                    AddParticleTarget(owner, owner, "Yasuo_Base_EQ_cas.troy", owner);
                    var sector = spell.CreateSpellSector(new SectorParameters
                    {
                        BindObject = spell.CastInfo.Owner,
                        Length = 215f,
                        SingleTick = true,
                        CanHitSameTargetConsecutively = true,
                        OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectNeutral | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes,
                        Type = SectorType.Area
                    });
                    owner.SetSpell("YasuoQW", 0, true);
                    owner.RemoveBuffsWithName("YasuoQ02");
                });
            }
                else
                {
                v = spell.CreateSpellMissile(s.MissileParameters);
                mushroom = AddMinion(owner, "TeemoMushroom", "TeemoMushroom", owner.Position);
                var Champs = GetChampionsInRange(owner.Position, 50000, true);
                var spellPos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);
                FaceDirection(spellPos, spell.CastInfo.Owner, true);
                owner.StopMovement();
                owner.PlayAnimation("Spell1C", 0.5f, 0, 1);
                owner.SetSpell("YasuoQW", 0, true);
                owner.RemoveBuffsWithName("YasuoQ02");
                var x = AddParticle(owner, mushroom, "Yasuo_Base_Q_wind_mis.troy", owner.Position);
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
                    mushroom.SetInvisible((int)player.GetPlayerId(), mushroom, 0f, 0f);
                    mushroom.SetHealthbarVisibility((int)player.GetPlayerId(), mushroom, false);
                }
                mushroom.SetCollisionRadius(0.0f);
                mushroom.SetStatus(StatusFlags.Targetable, false);
            }
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            AddParticleTarget(spell.CastInfo.Owner, target, "Yasuo_Base_Q_hit_tar.troy", target);
            var owner = spell.CastInfo.Owner;
            var APratio = owner.Stats.AttackDamage.Total;
            var spelllvl = (spell.CastInfo.SpellLevel * 20);
            target.TakeDamage(owner, APratio  + spelllvl + 2, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            ForceMovement(target, "RUN", new Vector2(target.Position.X + 10f, target.Position.Y + 10f), 13f, 0, 16.5f, 0);
            if(target is Champion)
            {
                AddBuff("Knockup", 1f, 1, spell, target, owner, false);
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

        public void OnUpdate(float diff)
        {
        }
    }
}

