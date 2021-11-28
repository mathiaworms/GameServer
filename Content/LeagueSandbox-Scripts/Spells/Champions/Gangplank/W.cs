using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System;
using System.Numerics;
using GameServerCore.Enums;
namespace Spells
{
    public class RemoveScurvy : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
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
            owner.StopMovement();
            float ap = owner.Stats.AbilityPower.Total; //100% AP Ratio
            float newHealth = target.Stats.CurrentHealth + 80 + ap;
            target.Stats.CurrentHealth = Math.Min(newHealth, target.Stats.HealthPoints.Total);
            owner.SetStatus(StatusFlags.CanAttack, true);
            owner.SetStatus(StatusFlags.CanCast, true);
            owner.SetStatus(StatusFlags.CanMove, true);
            owner.SetStatus(StatusFlags.Stunned, false);
            owner.SetStatus(StatusFlags.Rooted, false);
            owner.SetStatus(StatusFlags.Disarmed, false);
            owner.SetStatus(StatusFlags.Rooted, false);
            owner.SetStatus(StatusFlags.Suppressed, false);

            owner.Stats.SetActionState(ActionState.CAN_ATTACK, true);
            owner.Stats.SetActionState(ActionState.CAN_CAST, true);
            owner.Stats.SetActionState(ActionState.CAN_MOVE, true);

            owner.SetStatus(StatusFlags.CanMove, true);
            owner.SetStatus(StatusFlags.Rooted, false);

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
