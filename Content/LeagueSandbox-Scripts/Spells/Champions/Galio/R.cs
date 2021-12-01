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
    public class GalioIdolOfDurand : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
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
            LogDebug("yo");
            var x = GetChampionsInRange(spell.CastInfo.Owner.Position, 800, true);
            foreach (var units in x)
            {
                if (units.Team != spell.CastInfo.Owner.Team)
                {
                    var target = units;
                    var owner = spell.CastInfo.Owner;
                    //target.TakeDamage(owner, 50, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_RAW, true);
                    var ap = owner.Stats.AbilityPower.Total * 0.60;
                    float damage = (float)(ap + 200 + (owner.Spells[3].CastInfo.SpellLevel * 100));
                    target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
                    FaceDirection(owner.Position, target);
                    var xy = GetPointFromUnit(target, 300);
                    ForceMovement(target, "run", xy, 10, 0, 0, 0);
                    AddBuff("VeigarEventHorizon", 2.0f, 1, spell, target, owner);
                }
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

        public void OnUpdate(float diff)
        {
        }
    }
}
