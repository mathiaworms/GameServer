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
    public class MissFortuneBulletTime : ISpellScript
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
        public ISpellSector DamageSector;
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

            var DamageSector = spell.CreateSpellSector(new SectorParameters
            {
		        BindObject = owner,
                Length = 1450f,
		        Tickrate = 4,
                ConeAngle = 30.0f,
	        	CanHitSameTargetConsecutively = true,
                Type = SectorType.Cone
            });


                AddParticleTarget(owner, owner, "MissFortune_Base_R_cas.troy", owner);
		        AddParticleTarget(owner, owner, "MissFortune_Base_R_cas_left.troy", owner);
		        AddParticle(owner, null, "MissFortune_Base_R_MuzzleFlash_Cas.troy", spellPos, lifetime: 5.0f , reqVision: false);
                AddParticle(owner, null, "MissFortune_Base_R_mis.troy", spellPos, lifetime: 5.0f , reqVision: false);

                AddParticle(owner, null, "MissFortune_Base_W_buf.troy", GetPointFromUnit(owner, 1450f), lifetime: 5.0f);
       }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
             var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.125f;
		var ad = spell.CastInfo.Owner.Stats.AttackDamage.Total * 0.125f;
		if (spell.CastInfo.SpellLevel == 1 )
		{
                var damage = 50 + ap + ad;
                target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            }
		else 
		{
                var damage = 75 + (50 * (spell.CastInfo.SpellLevel - 2)) + ap + ad;
                target.TakeDamage(spell.CastInfo.Owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            }
           
           // AddBuff("MissFortuneR", 0.5f, 1, spell, target, owner);

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
