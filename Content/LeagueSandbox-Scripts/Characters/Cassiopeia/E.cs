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
    public class CassiopeiaTwinFang : ISpellScript
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
            var APratio = owner.Stats.AbilityPower.Total;
            var damage = 30 + spell.CastInfo.SpellLevel * 25 + APratio;

            if(target is IChampion)
            {
                AddBuff("CassiopeiaDeadlyCadence", float.MaxValue, 1, owner.GetSpell(0), owner, owner, true);
            }

            AddParticleTarget(owner, target, "Cassiopeia_Base_E_TwinFang_tar.troy", target, 1f);
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            if (target.HasBuff("CassiopeiaPoisonTicker"))
            {
                owner.Spells[2].SetCooldown(0.5f, true);
                //cassio ticker 3 buff
            }
            if (target.HasBuff("CassiopeiaPoisonTicker2"))
            {
                owner.Spells[2].SetCooldown(0.5f, true);
                //cassio ticker 4 buff
            }

            CreateTimer(0.3f, () => {
                if (target.IsDead)
                {
                    owner.Stats.CurrentMana += 45;
                }
            });
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