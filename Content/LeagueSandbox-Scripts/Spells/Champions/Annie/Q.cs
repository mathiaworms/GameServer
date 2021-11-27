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

namespace Spells
{
    public class Disintegrate : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Target
            }
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            var ownerSkinID = owner.SkinID;
            var ap = owner.Stats.AbilityPower.Total * spell.SpellData.MagicDamageCoefficient;
            var damage = 45 + spell.CastInfo.SpellLevel * 35 + ap;
            var stunduration = 1.25f + 0.25f * owner.GetSpell("InfernalGuardian").CastInfo.SpellLevel;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);

            if (ownerSkinID == 5)
            {
                AddParticleTarget(owner, target, "DisintegrateHit_tar_frost.troy", target);
            }
            else
            {
                AddParticleTarget(owner, target, "DisintegrateHit_tar.troy", target);
            }

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
