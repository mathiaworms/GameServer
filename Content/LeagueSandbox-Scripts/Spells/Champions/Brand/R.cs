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
    public class BrandWildfire : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Chained,
                MaximumHits = 5,
                CanHitSameTarget = true
                //CanHitSameTargetConsecutively = true
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
            var ap = owner.Stats.AbilityPower.Total * 0.5;
            var damage = 45 + spell.CastInfo.SpellLevel * 35 + ap;
            var currtarget = target;
            var nexttarget = GetClosestUnitInRange(currtarget.Position, 300, true);
            SpellCast(spell.CastInfo.Owner, 1, SpellSlotType.ExtraSlots, currtarget.Position, nexttarget.Position, true, currtarget.Position);
            //var nexttarget = GetClosestUnitInRange(currtarget.Position, 300, true);
            //AddParticlePos(owner, "BrandWildfire_mis.troy", currtarget.Position, nexttarget.Position);
            //AddParticlePos(owner, "BrandWildfire_cas.troy", currtarget.Position, nexttarget.Position);
            /*for (int i = 4; i <= 4; i++)
            {
                var nexttarget = GetClosestUnitInRange(currtarget.Position, 300, true);
                SpellCast(spell.CastInfo.Owner, 1, SpellSlotType.ExtraSlots, currtarget.Position, nexttarget.Position, true, currtarget.Position);
                AddParticlePos(owner, "BrandWildfire_mis.troy", currtarget.Position, nexttarget.Position);
                missile.PlayAnimation("Spell4", 1, 0, 0);
                currtarget = nexttarget;

            }*/
            

            var ad = owner.Stats.AbilityPower.Total * 0.5 + spell.CastInfo.Owner.GetSpell(3).CastInfo.SpellLevel * 100 + 50;
            AddBuff("BrandPassive", 4f, 1, spell, target, owner);
            

            target.TakeDamage(owner, (float)ad, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            //firstTarget = target;
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
