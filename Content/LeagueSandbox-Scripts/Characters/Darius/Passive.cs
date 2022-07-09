using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Passives
{
    public class DariusHemoMarker : ICharScript

    {
        private ISpell Spell;
        private int counter;

        public void OnActivate(IObjAiBase owner, ISpell spell)

        {
            Spell = spell;

            {
                ApiEventManager.OnHitUnitByAnother.AddListener(this, owner, OnHitUnit, false);
            }
        }

        public void OnHitUnit(IAttackableUnit target, bool IsCrit)

        {
            var owner = Spell.CastInfo.Owner;
            AddBuff("DariusHemoMarker", 5f, 1, Spell, target, owner);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnHitUnit.RemoveListener(this);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}