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
    public class Headshot_Marker : ICharScript
    {

        private IObjAiBase _owner;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            _owner = owner;
            ApiEventManager.OnHitUnitByAnother.AddListener(this, owner, TargetExecute, false);
        }
        int i = 0;
         public void TargetExecute(IAttackableUnit target, bool isCrit)
         {
            i++;
            if (i == 6)
            {
                i = 0;
                if(target is IMinion)
                {
                    target.TakeDamage(_owner, _owner.Stats.AttackDamage.Total, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, isCrit);
                }
                target.TakeDamage(_owner, _owner.Stats.AttackDamage.Total * 0.5f, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_ATTACK, isCrit);
                _owner.RemoveBuffsWithName("CaitlynHeadshotReady");
            }
            LogDebug(i.ToString());
         }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}