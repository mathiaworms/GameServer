using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain.GameObjects.Spell.Missile;

namespace Spells
{
    public class ZedUlt : ISpellScript
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
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X - 75, spell.CastInfo.TargetPosition.Z - 75);
            var originPos = new Vector2(spell.CastInfo.Owner.Position.X + 1, spell.CastInfo.Owner.Position.Y + 1);
            SpellCast(owner, 4, SpellSlotType.ExtraSlots, originPos, originPos, true, owner.Position);
            owner.DashToLocation(spellPos, 1600, "RUN");
            owner.SetStatus(GameServerCore.Enums.StatusFlags.Targetable, false);
            AddParticle(spell.CastInfo.Owner, spell.CastInfo.Targets[0].Unit, "Zed_Ult_TargetMarker_tar.troy", Vector2.Zero);
            var Champs = GetChampionsInRange(owner.Position, 50000, true);
            CreateTimer(4.0f, () => { target.TakeDamage(owner, 200, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false); });
            foreach (IChampion player in Champs)
            {
                CreateTimer(0.5f, () => { owner.SetInvisible((int)player.GetPlayerId(), owner, 1f, 0.1f); owner.SetStatus(GameServerCore.Enums.StatusFlags.Targetable, true); });
                owner.SetInvisible((int)player.GetPlayerId(), owner, 0f, 0.05f);
            }
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
