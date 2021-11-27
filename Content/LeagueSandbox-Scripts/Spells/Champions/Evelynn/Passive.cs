using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Passives
{
    public class EvelynnPassive : ICharScript
    {
        IObjAiBase champ;
        ISpell thisSpell;
        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            thisSpell = spell;
            champ = owner as IChampion;
            ApiEventManager.OnPreAttack.AddListener(this, owner, OnPreAttack, false);
            ApiEventManager.OnSpellCast.AddListener(this, spell, OnSpellCast);
            ApiEventManager.OnBeingHit.AddListener(this, owner, OnBeingHit, false);
            AddBuff("EvelynnPassiveHandler", 6.0f, 1, thisSpell, spell.CastInfo.Owner, spell.CastInfo.Owner, false);
        }
        public void OnPreAttack(ISpell spell)
        {
            AddBuff("EvelynnPassiveHandler", 6.0f, 1, thisSpell, spell.CastInfo.Owner, spell.CastInfo.Owner, false);
        }

        public void OnSpellCast(ISpell spell)
        {
            AddBuff("EvelynnPassiveHandler", 6.0f, 1, thisSpell, spell.CastInfo.Owner, spell.CastInfo.Owner, false);
        }

        public void OnBeingHit(IAttackableUnit attackableUnit, IAttackableUnit source)
        {
            AddBuff("EvelynnPassiveHandler", 6.0f, 1, thisSpell, champ, champ, false);
        }
        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
