using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore;
using System.Linq;
using System.Numerics;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;

namespace Spells
{
    public class VolibearE : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
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
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
            spell.CastInfo.Owner.PlayAnimation("Spell3", 0.5f);

            AddParticle(spell.CastInfo.Owner, spell.CastInfo.Owner, "Volibear_E_cas_blast.troy", spell.CastInfo.Owner.Position);

            float damage = new float[] { 60, 105, 150, 195, 240 }[spell.CastInfo.SpellLevel - 1];
            float slow = new float[] { 0.30f, 0.35f, 0.40f, 0.45f, 0.50f }[spell.CastInfo.SpellLevel - 1];

            var units = GetUnitsInRange(spell.CastInfo.Owner.Position, 300, true).Where(x => x.Team == CustomConvert.GetEnemyTeam(spell.CastInfo.Owner.Team));

            foreach (var target in units)
            {
                if (target is IAttackableUnit && spell.CastInfo.Owner != target)
                {
                    target.Stats.MoveSpeed.PercentBonus = -slow;
                    target.TakeDamage(spell.CastInfo.Owner, damage, GameServerCore.Enums.DamageType.DAMAGE_TYPE_MAGICAL, GameServerCore.Enums.DamageSource.DAMAGE_SOURCE_SPELL, false);
                    CreateTimer(3.0f, () => { target.Stats.MoveSpeed.PercentBonus = 0f; });
                }
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
