using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Passives
{
    public class GalioRunicSkin : ICharScript
    {
        private ISpell originspell;
        private IObjAiBase ownermain;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            originspell = spell;
            ownermain = owner;
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        float baseap = 0;
        float bonusap = 0;

        public void OnUpdate(float diff)
        {
            if (!ownermain.HasBuff("GalioPassiveBuff"))
            {
                AddBuff("GalioPassiveBuff", 0.5f, 1, originspell, ownermain, ownermain);
            }
        }
    }
}