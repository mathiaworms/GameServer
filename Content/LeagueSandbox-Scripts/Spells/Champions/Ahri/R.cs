using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;
using System.Linq;
using GameServerCore;

namespace Spells
{
    public class AhriTumble : ISpellScript
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

            CreateTimer(0.5f, () =>
            {
                var units = GetUnitsInRange(spell.CastInfo.Owner.Position, 500, true).Where(x => x.Team == CustomConvert.GetEnemyTeam(spell.CastInfo.Owner.Team));
                var i = 0;
                foreach (var allyTarget in units)
                {
                    if (allyTarget is IAttackableUnit && spell.CastInfo.Owner != allyTarget)
                    {
                        if (i < 1)
                        {
                            SpellCast(spell.CastInfo.Owner, 3, SpellSlotType.ExtraSlots, true, allyTarget, Vector2.Zero);
                            i++;
                        }

                    }
                }
            });


            FaceDirection(current, spell.CastInfo.Owner, true);
            CreateTimer(0.01f, () => { ForceMovement(spell.CastInfo.Owner, "Spell4", trueCoords, 1500, 0, 0, 0); });

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