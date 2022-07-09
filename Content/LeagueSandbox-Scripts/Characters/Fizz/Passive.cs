using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Passives
{
    public class FizzPassive : ICharScript
    {
        private ISpell originspell;
        private IObjAiBase ownermain;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ownermain = owner;
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
            if(!ownermain.Status.HasFlag(StatusFlags.Ghosted)){
                ownermain.SetStatus(GameServerCore.Enums.StatusFlags.Ghosted, true); // IDK IF THERE IS ANYTHING THAT REMOVES GHOSTING
                LogDebug("ghosted");
            }
        }
    }
}