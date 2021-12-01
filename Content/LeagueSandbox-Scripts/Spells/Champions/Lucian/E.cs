using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;

namespace Spells
{
    public class LucianE : ISpellScript
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
            var current = new Vector2(spell.CastInfo.Owner.Position.X, spell.CastInfo.Owner.Position.Y);
            var trueCoords = GetPointFromUnit(spell.CastInfo.Owner, spell.SpellData.CastRangeDisplayOverride);

            AddBuff("LucianPassiveBuff", 3.5f, 1, spell, spell.CastInfo.Owner, spell.CastInfo.Owner);

            FaceDirection(current, spell.CastInfo.Owner, true);
            CreateTimer(0.01f, () => { ForceMovement(spell.CastInfo.Owner, "Spell3", trueCoords, 1350, 0, 0, 0); });
            AddParticleTarget(spell.CastInfo.Owner, spell.CastInfo.Owner, "Lucian_E_cas_trail.troy", spell.CastInfo.Owner, 1f);

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