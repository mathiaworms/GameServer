using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Scripting.CSharp;

namespace Passives
{
    public class Scurvy : ICharScript
    {
        ISpell originspell;
        IObjAiBase ownermain;
        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            originspell = spell;
            ownermain = owner;
            ApiEventManager.OnHitUnit.AddListener(this, owner, TargetExecute, false);

        }
        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }
        public void OnUpdate(float diff)
        {
        }

        public void TargetExecute(IAttackableUnit unit, bool arg2)
        {
            var owner = ownermain;
            AddBuff("GangplankBleed", 3f, 1, originspell, unit, owner);
            LogDebug(unit.GetBuffWithName("GangplankBleed").StackCount.ToString());
            }
        }
    }

