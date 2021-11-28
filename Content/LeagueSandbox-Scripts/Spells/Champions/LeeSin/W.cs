using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Enums;

namespace Spells
{
    public class BlindMonkWOne : ISpellScript
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
            var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.8;
            var spellvl = spell.CastInfo.SpellLevel * 40;
            ////owner.TeleportTo(target.Position.X + 75, target.Position.Y + 75);
            TeleportTo(owner, target.Position.X + 50, target.Position.Y);
            //owner.SetTargetUnit(null);
            var x = target as IObjAiBase;
            //AddParticleTarget(x, x, "blindMonk_W_shield_block.troy", x, lifetime: 2.0f);
            x.ApplyShield(target, (float)(ap + spellvl), true, true, false);
            CreateTimer(2.0f, () => { x.ApplyShield(target, -5000, true, true, false); });
            CreateTimer(0.5f, () => { owner.SetSpell("BlindMonkWTwo", 1, true); });
            //owner.DashToTarget(target, 1000f, "RUN", 0, false, 0, 0, 5000) ;
        }

        public void OnSpellCast(ISpell spell)
        {
        }
        static internal bool procced = false;
        public void OnSpellPostCast(ISpell spell)
        {
            CreateTimer(3.0f, () => {
                if (procced == false)
                {
                    spell.CastInfo.Owner.SetSpell("BlindMonkWOne", 1, true);
                    spell.CastInfo.Owner.Spells[1].SetCooldown(spell.SpellData.Cooldown[1]);
                }
            });
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

    public class BlindMonkWTwo : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Circle
            }
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
            AddBuff("LeeSinWTwoBuff", 4.0f, 1, spell, spell.CastInfo.Owner, spell.CastInfo.Owner);
            spell.CastInfo.Owner.SetSpell("BlindMonkWOne", 1, true);
            spell.CastInfo.Owner.Spells[1].SetCooldown(spell.SpellData.Cooldown[1]);
            BlindMonkWOne.procced = true;
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
