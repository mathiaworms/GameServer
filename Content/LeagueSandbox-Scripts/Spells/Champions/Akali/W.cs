using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects.Spell.Sector;

namespace Spells
{
    public class AkaliSmokeBomb : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
           // ApiEventManager.OnSpellSectorHit.AddListener(this, new KeyValuePair<ISpell, IObjAiBase>(spell, owner), TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
           // ApiEventManager.OnSpellSectorHit.RemoveListener(this);
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public ISpellSector Shroud;
        //public IObjAiBase Owner;
        public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            //var initialCastPos = owner.Position;
            //var initialCastPos = new Vector2(owner.Position.X, owner.Position.Y);
            var initialCastPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);

            //AddBuff("AkaliTwilightShroud", 6f, 1, spell, owner, owner);

            AddParticle(owner, null, "akali_smoke_bomb_tar.troy", initialCastPos, 8f);
            AddParticle(owner, null, "akali_smoke_bomb_tar_team_green.troy", initialCastPos, 8f);

            //var point = GetPointFromUnit(owner, 250f);
            //TeleportTo(owner, point.X, point.Y);


            /*
              TODO: Display green border (akali_smoke_bomb_tar_team_green.troy) for the own team,
              display red border (akali_smoke_bomb_tar_team_red.troy) for the enemy team
              Currently only displaying the green border for everyone.
            */

            if (Shroud != null)
            {
                Shroud.SetToRemove();
                Shroud = null;
            }

            Shroud = spell.CreateSpellSector(new SectorParameters
            {
                //BindObject = ownerSpell.CastInfo.Owner,
                Length = 320f,
                Tickrate = 2,
                CanHitSameTargetConsecutively = true,
                OverrideFlags = SpellDataFlags.AffectEnemies | SpellDataFlags.AffectMinions | SpellDataFlags.AffectHeroes | SpellDataFlags.AffectFriends,
                Type = SectorType.Area
            });

            _timeSinceCast = 0f;

            //Owner = owner;

        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;

            if (!HasBuff(owner, "AkaliTwilightShroudCD") && target.NetId == owner.NetId)
            {
                AddBuff("AkaliTwilightShroud", 0.75f, 1, spell, owner, owner);
            }

            if (target.Team != owner.Team)
            {
                AddBuff("AkaliTwilightShroudDebuff", 0.75f, 1, spell, target, owner);
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

        private float _timeSinceCast = 0f;
        private float _duration = 8000f;
        public void OnUpdate(float diff)
        {
            _timeSinceCast += diff;
            if (Shroud != null && _timeSinceCast >= _duration)
            {
                Shroud.SetToRemove();
                Shroud = null;
            }
        }
    }
}


