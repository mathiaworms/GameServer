using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Domain.GameObjects.Spell.Missile;

namespace Spells
{
    public class Incinerate : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true
            // TODO
        };
        public ISpellSector damagesector;
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

            var damagesector = spell.CreateSpellSector(new SectorParameters
            {
                Length = 625f,
                SingleTick = true,
                ConeAngle = 24.76f,
                Type = SectorType.Cone
            });

            AddParticle(owner, null, "IIncinerate_buf.troy", GetPointFromUnit(owner, 625f));
            AddParticleTarget(owner, owner, "Incinerate_cas.troy", owner);
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var stunduration = 1.25f + 0.25f * owner.GetSpell("InfernalGuardian").CastInfo.SpellLevel;
            var ap = owner.Stats.AbilityPower.Total * 0.8f;
            var damage = 70 + spell.CastInfo.SpellLevel * 45 + ap;

            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            if( spell.CastInfo.Owner.HasBuff("Pyromania"))
             
             {  
                    if (owner.GetBuffWithName("Pyromania").StackCount == 4)
                  {
                      AddBuff("Stun", stunduration, 1, spell, target, owner);
                    owner.RemoveBuffsWithName("Pyromania");
                   }
                   else {
                    AddBuff("Pyromania", 25000f, 1, spell, owner, owner);
                   }
            }
             else {
                 AddBuff("Pyromania", 25000f, 1, spell, spell.CastInfo.Owner, spell.CastInfo.Owner);
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
