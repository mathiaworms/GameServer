using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.API;

namespace Buffs
{
    class ZedRShadowBuff : IBuffGameScript
    {
        public IBuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_ENCHANCER,
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IBuff ThisBuff;
        IMinion Shadow;
        IParticle p;
		IParticle p2;
		IParticle p3;
		IParticle currentIndicator;
        int previousIndicatorState;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            ThisBuff = buff;
            Shadow = unit as IMinion;
            var ownerSkinID = Shadow.Owner.SkinID;
            string particles;			
            p = AddParticle(Shadow.Owner, null, "", Shadow.Position, buff.Duration);
            p2 = AddParticle(Shadow.Owner, null, ".troy", Shadow.Position, buff.Duration);
			AddParticleTarget(Shadow.Owner, Shadow, "zed_base_w_tar.troy", Shadow);
            ApiEventManager.OnSpellCast.AddListener(this, Shadow.Owner.GetSpell("ZedR2"), R2OnSpellCast);
			ApiEventManager.OnSpellCast.AddListener(this, Shadow.Owner.GetSpell("ZedShuriken"), QOnSpellCast);
            ApiEventManager.OnSpellPostCast.AddListener(this, Shadow.Owner.GetSpell("ZedShuriken"), QOnSpellPostCast);
            ApiEventManager.OnSpellCast.AddListener(this, Shadow.Owner.GetSpell("ZedPBAOEDummy"), EOnSpellCast);
			currentIndicator = AddParticleTarget(Shadow.Owner, Shadow.Owner, "zed_shadowindicatorfar.troy", Shadow, buff.Duration, flags: FXFlags.TargetDirection);
        }
		public void QOnSpellCast(ISpell spell)
        {
            if (Shadow != null && !Shadow.IsDead)
            {
                PlayAnimation(Shadow, "Spell1");
                var targetPos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);
                FaceDirection(targetPos, Shadow);
            }
        }

        public void QOnSpellPostCast(ISpell spell)
        {
            if (Shadow != null && !Shadow.IsDead)
            {
                var owner = spell.CastInfo.Owner;
                var targetPos = new Vector2(spell.CastInfo.TargetPositionEnd.X, spell.CastInfo.TargetPositionEnd.Z);

                SpellCast(Shadow.Owner, 0, SpellSlotType.ExtraSlots, targetPos, Vector2.Zero, true, Shadow.Position);
            }
        }
        public void EOnSpellCast(ISpell spell)
        {
			var owner = spell.CastInfo.Owner;
			var ownerSkinID = owner.SkinID;
            if (Shadow != null && !Shadow.IsDead)
            {
                SpellCast(Shadow.Owner, 2, SpellSlotType.ExtraSlots, true, Shadow, Vector2.Zero);
                PlayAnimation(Shadow, "Spell3", 0.5f);
				if (ownerSkinID == 1)
                {
                    AddParticleTarget(spell.CastInfo.Owner, null, "Zed_Skin01_E_cas.troy", owner);
                }
                else
				{
					AddParticleTarget(spell.CastInfo.Owner, null, "Zed_E_cas.troy", Shadow);
				}
            }
        }
		public void R2OnSpellCast(ISpell spell)
        {
            var ownerPos = Shadow.Owner.Position;			
            if (Shadow != null && !Shadow.IsDead)
            {
				TeleportTo(Shadow.Owner, Shadow.Position.X, Shadow.Position.Y);
				TeleportTo(Shadow, ownerPos.X, ownerPos.Y);
				AddParticleTarget(Shadow.Owner, Shadow.Owner, "zed_base_cloneswap.troy", Shadow.Owner);
				AddParticleTarget(Shadow.Owner, Shadow, "zed_base_cloneswap.troy", Shadow);
				AddParticle(Shadow.Owner, null, "", Shadow.Position);
                AddParticle(Shadow.Owner, null, "", Shadow.Position);
            }
            Shadow.Owner.RemoveBuffsWithName("ZedRHandler");			
        }
        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            if (Shadow != null && !Shadow.IsDead)
            {
				AddParticle(Shadow.Owner, null, "", Shadow.Position);
				if (currentIndicator != null)
                {
                    currentIndicator.SetToRemove();
                }
                if (p != null)
                {
                    p.SetToRemove();
					p2.SetToRemove();
                }
                SetStatus(Shadow, StatusFlags.NoRender, true);
                AddParticle(Shadow.Owner, null, "", Shadow.Position);
				AddParticle(Shadow.Owner, null, "zed_base_clonedeath.troy", Shadow.Position);
                Shadow.TakeDamage(Shadow, 10000f, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, DamageResultType.RESULT_NORMAL);
            }
        }
		public int GetIndicatorState()
        {
            var dist = Vector2.Distance(Shadow.Owner.Position, Shadow.Position);
            var state = 0;

            if (!Shadow.Owner.HasBuff("ZedR2"))
            {
                return state;
            }

            if (dist >= 1000.0f)
            {
                state = 0;
            }
            else if (dist >= 800.0f)
            {
                state = 1;
            }
            else if (dist >= 0f)
            {
                state = 2;
            }

            return state;
        }

        public string GetIndicatorName(int state)
        {
            switch (state)
            {
                case 0:
                    {
                        return "zed_shadowindicatorfar.troy";
                    }
                case 1:
                    {
                        return "zed_shadowindicatormed.troy";
                    }
                case 2:
                    {
                        return "zed_shadowindicatornearbloop.troy";
                    }
                default:
                    {
                        return "zed_shadowindicatorfar.troy";
                    }
            }
        }
        public void OnUpdate(float diff)
        {
            if (Shadow != null && !Shadow.IsDead)
            {
                int state = GetIndicatorState();
                if (state != previousIndicatorState)
                {
                    previousIndicatorState = state;
                    if (currentIndicator != null)
                    {
                        currentIndicator.SetToRemove();
                    }

                    currentIndicator = AddParticleTarget(Shadow.Owner, Shadow.Owner, GetIndicatorName(state), Shadow, ThisBuff.Duration - ThisBuff.TimeElapsed, flags: FXFlags.TargetDirection);
                }
            }			
        }
    }
}