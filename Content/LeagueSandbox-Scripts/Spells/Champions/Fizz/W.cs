using System;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;

namespace Spells
{
    public class FizzSeastonePassive : ISpellScript
    {
        IAttackableUnit Target;
        ISpell daspell;
        IObjAiBase daowner;
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
            Target = target;
            daspell = spell;
            daowner = owner;
            ApiEventManager.OnLevelUpSpell.AddListener(this, owner.GetSpell("FizzSeastonePassive"), AddFizzPassive, false);
        }



        public void AddFizzPassive(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            AddBuff("FizzMalison", 99999f, 1, spell, daowner, daowner, true);
        }
        

        public void OnSpellCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
            AddBuff("FizzSeastoneActive", 5f, 1, spell, owner, owner);

        }

        public void OnSpellPostCast(ISpell spell)
        {
            var owner = spell.CastInfo.Owner;
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
            //ApiEventManager.OnHitUnit.AddListener(this, daowner, TargetTakePoison, false);
        }
    }
}