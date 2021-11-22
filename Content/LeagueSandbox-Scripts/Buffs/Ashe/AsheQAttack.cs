using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain;


namespace Buffs
{
    internal class AsheQAttack : IBuffGameScript
    {
        public BuffType BuffType => BuffType.COMBAT_ENCHANCER;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        IBuff thisBuff;
        IObjAiBase Unit;
        IParticle p;
        IParticle p2;
        IObjAiBase Owner;
        ISpell spelll;
        float[] manaCost = { 8.0f , 8.0f , 8.0f , 8.0f , 8.0f };
        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            spelll = ownerSpell;
            Owner = ownerSpell.CastInfo.Owner;
            thisBuff = buff;
            if (unit is IObjAiBase ai)
            {
                Unit = ai;

                ApiEventManager.OnHitUnit.AddListener(this, ai, TargetExecute, true);

                ai.SkipNextAutoAttack();
            }

            unit.AddStatModifier(StatsModifier);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            //ApiEventManager.OnHitUnit.RemoveListener(this);
        }
        public void TargetExecute(IAttackableUnit target, bool Iscrit)
        {
            
            AddBuff("FrostShot", 2f, 1, Owner.GetSpell("FrostShot"), target, Owner);
            Owner.Stats.CurrentMana -= manaCost[spelll.CastInfo.SpellLevel - 1];
            AddBuff("AsheQAttack", 2f, 1, Owner.GetSpell("FrostShot"), Owner, Owner);
        }


        public void OnUpdate(float diff)
        {

        }
    }
}

