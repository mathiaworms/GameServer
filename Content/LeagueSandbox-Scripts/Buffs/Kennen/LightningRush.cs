using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;

namespace KennenLightningRush
{
    internal class KennenLightningRush : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 3;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IParticle p;
        IChampion Owner;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            var owner = ownerSpell.CastInfo.Owner;
            p = AddParticleTarget(owner, unit, "kennen_lr_buf.troy", unit, 1, buff.Duration); //Take a look at whi the particles disapear later
            StatsModifier.MoveSpeed.FlatBonus += 335f;
            StatsModifier.Armor.BaseBonus = ownerSpell.CastInfo.SpellLevel * 10;
            StatsModifier.MagicResist.BaseBonus = ownerSpell.CastInfo.SpellLevel * 10;
            unit.AddStatModifier(StatsModifier);

            SetStatus(unit, StatusFlags.CanAttack, false);
            SetStatus(unit, StatusFlags.Ghosted, true);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            // var champion = unit as IChampion;
            //var spell = champion.SetSpell("KennenLightningRush", 2, true);

            // StopAnimation(champion, "Spell3");
            // SetAnimStates(champion, new Dictionary<string, string> { { "RUN", "" } });
            //spell.SetCooldown(spell.GetCooldown() - (buff.Duration - buff.TimeElapsed));
            // RemoveParticle(p);
            var champion = unit as IChampion;
            RemoveParticle(p);

            SetStatus(unit, StatusFlags.CanAttack, true);
            SetStatus(unit, StatusFlags.Ghosted, false);
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
