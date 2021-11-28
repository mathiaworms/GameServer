using System.Linq;
using GameServerCore;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Enums;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class BlindMonkEOne : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
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

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            //blindMonk_E_cas.troy
            AddParticleTarget(owner, owner, "blindMonk_E_cas.troy", owner, 2.0f, 1, "L_hand", "R_foot");
            CreateTimer(0.5f, () => { owner.SetSpell("BlindMonkETwo", 2, true); });
        }
        static internal bool procced = false;
        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
            CreateTimer(3.0f, () => {
                if (procced == false)
                {
                    spell.CastInfo.Owner.SetSpell("BlindMonkEOne", 2, true);
                    spell.CastInfo.Owner.Spells[2].SetCooldown(spell.SpellData.Cooldown[2]);
                }
            });
            var sector = spell.CreateSpellSector(new SectorParameters
            {
                Length = 300f,
                SingleTick = true,
                Type = SectorType.Area
            });
        }
        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var AD = spell.CastInfo.Owner.Stats.AttackDamage.Total;
            target.TakeDamage(owner, AD, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_DEFAULT, false);
            AddParticleTarget(owner, target, "blindMonk_E_thunderCrash_tar.troy", target, 1f);
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

    public class BlindMonkETwo : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
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

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {

            spell.CastInfo.Owner.SetSpell("BlindMonkEOne", 2, true);
            spell.CastInfo.Owner.Spells[2].SetCooldown(spell.SpellData.Cooldown[2]);
            BlindMonkEOne.procced = true;

            var sector = spell.CreateSpellSector(new SectorParameters
            {
                Length = 300f,
                SingleTick = true,
                Type = SectorType.Area
            });
        }
        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            AddBuff("LeeSinESlow", 4.0f, 1, spell, target, spell.CastInfo.Owner);
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
