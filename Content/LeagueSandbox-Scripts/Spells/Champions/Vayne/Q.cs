using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class VayneTumble : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {

            if (spell.CastInfo.Owner.HasBuff("VayneUlt"))
            {
                LogDebug("has r");

                CreateTimer(1.0f, () => { owner.SetStatus(StatusFlags.Targetable, true); });
                owner.SetStatus(StatusFlags.Targetable, false);
                var Champs = GetChampionsInRange(owner.Position, 50000, true);
                foreach (IChampion player in Champs)
                {
                    CreateTimer(1.0f, () => { owner.SetHealthbarVisibility((int)player.GetPlayerId(), owner, true); });
                    if (player.Team.Equals(owner.Team))
                    {
                        CreateTimer(1.0f, () => { owner.SetInvisible((int)player.GetPlayerId(), owner, 1f, 0.1f); });
                        owner.SetInvisible((int)player.GetPlayerId(), owner, 0.5f, 0.1f);
                    }
                    if (!(player.Team.Equals(owner.Team)))
                    {
                        if (player.IsAttacking)
                        {
                            player.CancelAutoAttack(false);
                        }
                        CreateTimer(1.0f, () => { owner.SetInvisible((int)player.GetPlayerId(), owner, 1f, 0.1f); });
                        owner.SetInvisible((int)player.GetPlayerId(), owner, 0f, 0.1f);
                        owner.SetHealthbarVisibility((int)player.GetPlayerId(), owner, false);
                    }
                }
            }

            var current = new Vector2(spell.CastInfo.Owner.Position.X, spell.CastInfo.Owner.Position.Y);
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var to = Vector2.Normalize(spellPos - current);
            var range = to * spell.SpellData.CastRangeDisplayOverride;
            var trueCoords = current + range;
            ForceMovement(spell.CastInfo.Owner, "Spell1", trueCoords, 700, 0, 0, 0, GameServerCore.Enums.ForceMovementType.FIRST_WALL_HIT);
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
