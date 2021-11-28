using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.NetInfo;
using System;
using LeagueSandbox.GameServer.Content;

namespace Spells
{
    public class YasuoWMovingWall : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
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
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var spellLvl = spell.CastInfo.SpellLevel;
            owner.StopMovement();
            FaceDirection(spellPos, owner);
            var x = GetPointFromUnit(owner, 250);
            var x1 = GetPointFromUnit(owner, 260);
            var x2 = GetPointFromUnit(owner, 1);
            var mushroom2 = AddMinion(owner, "TeemoMushroom", "TeemoMushroom", x);             //
            mushroom2.SetStatus(StatusFlags.Ghosted, true);                                                               //FOR Particle
            var mushroom = AddChampion(owner.Team, (int)owner.NetId, "TeemoMushroom", x);  //MIDDLE MUSHROOM

            mushroom.SetStatus(StatusFlags.Ghosted, true);
            Vector2 LPos1 = GetPointFromUnit(owner, 3050, 30); 
            Vector2 RPos1 = GetPointFromUnit(owner, 3050, 30);

            Vector2 LPos1L = GetPointFromUnit(owner, 3050, 30);
            Vector2 RPos1L = GetPointFromUnit(owner, 3050, 30);

            //var LPos1 = GetPointFromUnit(owner, 300, 30);                       //LEVEL 1
            //var RPos1 = GetPointFromUnit(owner, 300, -30);                     //LEVEL 1
            if (spellLvl == 1)
            {
                LPos1 = GetPointFromUnit(owner, 300, 30);
                RPos1 = GetPointFromUnit(owner, 300, -30);

                LPos1L = GetPointFromUnit(owner, 300, 15);
                RPos1L = GetPointFromUnit(owner, 300, -15);
            }
            if (spellLvl == 2)
            {
                LPos1 = GetPointFromUnit(owner, 350, 37);
                RPos1 = GetPointFromUnit(owner, 350, -37);

                LPos1L = GetPointFromUnit(owner, 320, 20);
                RPos1L = GetPointFromUnit(owner, 320, -20);
            }
            if (spellLvl == 3)
            {
                LPos1 = GetPointFromUnit(owner, 360, 40);
                RPos1 = GetPointFromUnit(owner, 360, -40);

                LPos1L = GetPointFromUnit(owner, 320, 20);
                RPos1L = GetPointFromUnit(owner, 320, -20);
            }
            if (spellLvl == 4)
            {
                LPos1 = GetPointFromUnit(owner, 400, 47);
                RPos1 = GetPointFromUnit(owner, 400, -47);

                LPos1L = GetPointFromUnit(owner, 320, 25);
                RPos1L = GetPointFromUnit(owner, 320, -25);
            }
            if (spellLvl == 5)
            {
                LPos1 = GetPointFromUnit(owner, 440, 50);
                RPos1 = GetPointFromUnit(owner, 440, -50);

                LPos1L = GetPointFromUnit(owner, 300, 37);
                RPos1L = GetPointFromUnit(owner, 300, -37);

            }
            var Lmushroom = AddChampion(owner.Team, (int)owner.NetId, "TeemoMushroom", LPos1);
            var Rmushroom = AddChampion(owner.Team, (int)owner.NetId, "TeemoMushroom", RPos1);

            var LLmushroom = AddChampion(owner.Team, (int)owner.NetId, "TeemoMushroom", LPos1L);
            var LRmushroom = AddChampion(owner.Team, (int)owner.NetId, "TeemoMushroom", RPos1L);

            mushroom.Stats.HealthPoints.PercentBonus += 200f;
            Rmushroom.Stats.HealthPoints.PercentBonus += 200f;
            Lmushroom.Stats.HealthPoints.PercentBonus += 200f;
            LRmushroom.Stats.HealthPoints.PercentBonus += 200f;
            LLmushroom.Stats.HealthPoints.PercentBonus += 200f;

            mushroom.SetStatus(StatusFlags.Ghosted, true);
            Rmushroom.SetStatus(StatusFlags.Ghosted, true);
            Lmushroom.SetStatus(StatusFlags.Ghosted, true);
            LRmushroom.SetStatus(StatusFlags.Ghosted, true);
            LLmushroom.SetStatus(StatusFlags.Ghosted, true);

            mushroom2.SetStatus(StatusFlags.Targetable, false);
            mushroom.SetStatus(StatusFlags.Targetable, false) ;
            Rmushroom.SetStatus(StatusFlags.Targetable, false);
            Lmushroom.SetStatus(StatusFlags.Targetable, false);
            LRmushroom.SetStatus(StatusFlags.Targetable, false);
            LLmushroom.SetStatus(StatusFlags.Targetable, false);

            var Champs = GetChampionsInRange(owner.Position, 50000, true);

            foreach (IChampion player in Champs)
            {
                mushroom.SetInvisible((int)player.GetPlayerId(), mushroom, 0f, 0.0f);
                mushroom.SetHealthbarVisibility((int)player.GetPlayerId(), mushroom, false);
                Lmushroom.SetInvisible((int)player.GetPlayerId(), Lmushroom, 0f, 0.0f);
                Lmushroom.SetHealthbarVisibility((int)player.GetPlayerId(), Lmushroom, false);
                Rmushroom.SetInvisible((int)player.GetPlayerId(), Rmushroom, 0f, 0.0f);
                Rmushroom.SetHealthbarVisibility((int)player.GetPlayerId(), Rmushroom, false);
                LLmushroom.SetInvisible((int)player.GetPlayerId(), LLmushroom, 0f, 0.0f);
                LLmushroom.SetHealthbarVisibility((int)player.GetPlayerId(), LLmushroom, false);
                LRmushroom.SetInvisible((int)player.GetPlayerId(), LRmushroom, 0f, 0.0f);
                LRmushroom.SetHealthbarVisibility((int)player.GetPlayerId(), LRmushroom, false);
                mushroom2.SetInvisible((int)player.GetPlayerId(), mushroom2, 0f, 0.0f);
                mushroom2.SetHealthbarVisibility((int)player.GetPlayerId(), mushroom2, false);

            }
                CreateTimer(4.0f, () => 
            {
                TeleportTo(mushroom, 0,0);
                TeleportTo(Lmushroom, 0, 0);
                TeleportTo(Rmushroom, 0, 0);
                TeleportTo(LLmushroom, 0, 0);
                TeleportTo(LRmushroom, 0, 0);
                //mushroom.TakeDamage(mushroom, 500000000000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false);
                //Lmushroom.TakeDamage(mushroom, 50000000000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false);
                //Rmushroom.TakeDamage(mushroom, 5000000000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false);
                //LLmushroom.TakeDamage(mushroom, 50000000000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false);
                //LRmushroom.TakeDamage(mushroom, 5000000000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false);
            });
            CreateTimer(4.0f, () => { mushroom2.TakeDamage(mushroom, 5000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false); });
            FaceDirection(x1, mushroom2);
            var y = AddParticle(owner, mushroom2,  "Yasuo_Base_W_windwall" + spellLvl + ".troy", x, lifetime: 4.0f, flags: FXFlags.BindDirection, followGroundTilt: true);
        }

        public void WallHit(IAttackableUnit unit, IAttackableUnit unit1)
        {
            LogDebug("hit");
            LogDebug(unit.Team.ToString());
            LogDebug(unit1.Team.ToString());
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
