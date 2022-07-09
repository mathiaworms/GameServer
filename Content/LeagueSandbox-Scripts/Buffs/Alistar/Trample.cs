using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;


namespace Buffs
{
     class Trample : IBuffGameScript
    {
       public IBuffScriptMetaData BuffMetaData { get; set; } = new BuffScriptMetaData 
        {
            BuffType = BuffType.DAMAGE

        };    
        public BuffAddType BuffAddType => BuffAddType.RENEW_EXISTING;
        public int MaxStacks => 1;
        public bool IsHidden => false;

        public IStatsModifier StatsModifier { get; private set; } = new StatsModifier();

        static internal bool trample = false;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            trample = true;
            AddParticle(unit, unit, "alistar_trample_01.troy", unit.Position, lifetime: buff.Duration);
            AddParticle(unit, unit, "alistar_trample_head.troy", unit.Position, bone: "head", lifetime: buff.Duration);
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            trample = false;
        }

        public void OnUpdate(float diff)
        {
        }
    }
}