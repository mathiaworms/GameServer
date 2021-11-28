using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class KarthusWallOfPain : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            if(target.Team != owner.Team)
            {
                target.Stats.MoveSpeed.PercentBonus = -0.5f;
                CreateTimer(5.0f, () => { target.Stats.MoveSpeed.PercentBonus = 0f; });
            }
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            FaceDirection(spellPos, owner);
            var x = GetPointFromUnit(owner, 2500);

            var mushroom = AddMinion(owner, "TeemoMushroom", "TeemoMushroom", spellPos);

            var Champs = GetChampionsInRange(owner.Position, 50000, true);

            foreach (IChampion player in Champs)
            {
                mushroom.SetInvisible((int)player.GetPlayerId(), mushroom, 0f, 0.0f);
                mushroom.SetHealthbarVisibility((int)player.GetPlayerId(), mushroom, false);
            }

            mushroom.SetStatus(StatusFlags.Ghosted, true);
            mushroom.SetStatus(StatusFlags.Targetable, false);

            FaceDirection(x, mushroom);

            var x1 = GetPointFromUnit(mushroom, 400, 90);
            var x2 = GetPointFromUnit(mushroom, 400, -90);

            AddParticle(owner, null, "Karthus_Base_W_Post.troy", x1);
            AddParticle(owner, null, "Karthus_Base_W_Post.troy", x2);

            spell.CreateSpellSector(new SectorParameters
            {
                BindObject = mushroom,
                Length = -200f,
                Width = 400f,
                PolygonVertices = new Vector2[]
                    {
                    // Basic square, however the values will be scaled by Length/Width, which will make it a rectangle
                    new Vector2(-1, 0),
                    new Vector2(-1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                    },
                Tickrate = 500,
                Type = SectorType.Polygon
            });
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
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
