using Buffs;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Linq;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    public class Pulverize : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };


        // NEED TO ADD W->Q COMBO STUFF
        // :/

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            _owner = owner;
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }
        IAttackableUnit _owner;
        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            AddBuff("Trample", 4.0f, 1, spell, owner, owner);
        }

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner as IChampion;
            PlayAnimation(owner, "SPELL1");
        }

        public void OnSpellPostCast(ISpell spell)
        {
            var ownerr = spell.CastInfo.Owner as IChampion;
            AddParticle(ownerr, null, "Pulverize_cas.troy", ownerr.Position, lifetime: 0.5f);

            var spellLevel = ownerr.GetSpell("Pulverize").CastInfo.SpellLevel;

            var ap = spell.CastInfo.Owner.Stats.AbilityPower.Total * 0.5f;
            var damage = 20 + spellLevel * 40 + ap;
            foreach (var enemy in GetUnitsInRange(ownerr.Position, 375, true)
                .Where(x => x.Team != ownerr.Team))
            {
                if (enemy is IObjAiBase)
                {
                    if(!(enemy is IBaseTurret))
                    {
                        enemy.TakeDamage(ownerr, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                        AddBuff("Pulverize", 1.0f, 1, spell, enemy, ownerr);
                    }
                }
            }
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
        bool cd = false;
        public void OnUpdate(float diff)
        {
            //ONCE AGAIN i am going to do spell logic in a separate spell
            // what a bummer
            var dmg = 6 +_owner.Stats.Level;
            var ap = _owner.Stats.AbilityPower.Total * 0.1f;
            //LogDebug(Buffs.Trample.trample.ToString());
            if(Trample.trample == true)
            {
                if(cd == false)
                {
                    var units = GetUnitsInRange(_owner.Position, 300, true);
                    foreach(var unit in units)
                    {
                        if(unit.Team != _owner.Team)
                        {
                            unit.TakeDamage(_owner, dmg + ap, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                            cd = true;
                            CreateTimer(0.9f, () => { cd = false; });
                        }
                    }
                }
            }

        }
    }
}