using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
namespace Passives
{
    public class Nevershade : ICharScript
    {

        private IObjAiBase _owner;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            AddBuff("MundoPassiveCooldown", 0.9f, 1, spell, owner, owner);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}