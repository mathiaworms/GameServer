﻿using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Collections.Generic;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Buffs
{
    internal class RenektonSliceAndDiceDelay : IBuffGameScript
    {
        public IBuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData
        {
            BuffType = BuffType.COMBAT_ENCHANCER,
            BuffAddType = BuffAddType.REPLACE_EXISTING
        };

        public IStatsModifier StatsModifier { get; private set; }

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            ((IObjAiBase)unit).SetSpell("RenektonDice", 2, true);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            if (((IObjAiBase)unit).Spells[2].SpellName == "RenektonDice")
            {
                ((IObjAiBase)unit).SetSpell("RenektonSliceAndDice", 2, true);
            }
        }

        public void OnUpdate(float diff)
        {
            //nothing!
        }
    }
}
