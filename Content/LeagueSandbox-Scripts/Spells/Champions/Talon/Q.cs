using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class TalonNoxianDiplomacy : ISpellScript
    {

        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            NotSingleTargetSpell = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            //var owner = spell.CastInfo.Owner as IChampion;
            AddBuff("TalonNoxianDiplomacyBuff", 6.0f, 1, spell, owner, owner);
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

    public class TalonNoxianDiplomacyAttack : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = false,
            NotSingleTargetSpell = true
            // TODO
        };

        ISpell originspell;
        IObjAiBase ownermain;
        int Applied = 1;
        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            originspell = spell;
            ownermain = owner;
            ApiEventManager.OnHitUnit.AddListener(this, owner, TargetExecute, false);
        }

        public void TargetExecute(IAttackableUnit unit, bool arg2)
        {
            var owner = ownermain;
            owner.SetStatus(StatusFlags.Targetable, true);
            var Champs = GetChampionsInRange(owner.Position, 50000, true);
            foreach (IChampion player in Champs)
            {
                owner.SetInvisible((int)player.GetPlayerId(), owner, 1f, 0.1f);
                owner.SetHealthbarVisibility((int)player.GetPlayerId(), owner, true);
            }

            var ADratio = owner.Stats.AttackDamage.PercentBonus * 0.3f;
            var damage = 40f + (30f * (originspell.CastInfo.SpellLevel - 1)) + ADratio;
            if (Applied != 1)
            {
                unit.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                AddBuff("TalonBleedDebuff", 6f, 1, originspell, unit, owner);
                Applied = 1;
                //CreateTimer((float)6, () => { Applied = 1; });
            }
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            Applied = 0;
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

