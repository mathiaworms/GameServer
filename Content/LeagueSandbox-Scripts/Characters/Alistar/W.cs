using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Enums;
namespace Spells
{
    public class Headbutt : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            CastingBreaksStealth = true,
            DoesntBreakShields = true,
            TriggersSpellCasts = true,
            IsDamagingSpell = false,
            NotSingleTargetSpell = true
            // TODO
        };

        public static IAttackableUnit _target = null;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
           // ApiEventManager.OnFinishDash.AddListener(this, owner, AlistarPush, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        private IObjAiBase _owner;

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            AddBuff("Trample", 4.0f, 1, spell, owner, owner);
            var to = Vector2.Normalize(target.Position - owner.Position);
            owner.SetTargetUnit(null);
            ForceMovement(owner, "Spell2", new Vector2(target.Position.X - to.X * 100f, target.Position.Y - to.Y * 100f), 1500, 0, 0, 0);
            _target = target;
            _owner = owner;
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void AlistarPush(IAttackableUnit unit)
        {
            LogDebug("yo");
            FaceDirection(_owner.Position, _target);
            var x = GetPointFromUnit(_target, -600);

            var xy = _target as IObjAiBase;
            xy.SetTargetUnit(null);

            ForceMovement(_target, "run", x, 1500, 0, 0, 0);
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, ISpellMissile missile)
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