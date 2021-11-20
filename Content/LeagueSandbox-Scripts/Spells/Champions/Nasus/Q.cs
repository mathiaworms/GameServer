using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Scripting.CSharp;


namespace Spells
{
    public class NasusQ : ISpellScript
    {
        IObjAiBase Owner;
        IAttackableUnit Target;
        IAttackableUnit owner2;
        ISpell spelll;
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
           
         //   ApiEventManager.OnLevelUpSpell.AddListener(this, owner.GetSpell("NasusQ"), AddNasusQStacksBuff, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            Target = target;
            Owner = owner;
            spelll = spell;
        }


        public void AddNasusQStacksBuff(ISpell spell)
        {
          // AddBuff("NasusQStacks", 25000f, 1, spell, owner, owner, true);
        }

        public void OnSpellCast(ISpell spell)
        {
            AddBuff("NasusQAttack", 5f, 1, spell, spell.CastInfo.Owner, spell.CastInfo.Owner);
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