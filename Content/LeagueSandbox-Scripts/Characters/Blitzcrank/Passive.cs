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
    public class ManaBarrierIcon : ICharScript
    {
        private ISpell originspell;
        private IObjAiBase _owner;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            originspell = spell;
            _owner = owner;
            ApiEventManager.OnTakeDamageByAnother.AddListener(this, owner, TargetExecute, false);
        }
        bool haspassive = false;
        bool timer = false;
        bool oldpos = false;
        Vector2 oldposV;
        private void TargetExecute(IAttackableUnit unit1, IAttackableUnit unit2)
        {
                if (unit1.Stats.CurrentHealth < unit1.Stats.HealthPoints.Total * 0.2f)
                {
                    var unit1champ = unit1 as IChampion;
                    if (timer != true)
                    {
                        timer = true;
                        var shieldamt = unit1.Stats.ManaPoints.Total * 0.5f;
                        unit1champ.ApplyShield(unit1, shieldamt, true, true, false);
                        CreateTimer(10.0f, () => { unit1champ.ApplyShield(unit1, -shieldamt, true, true, false); });
                        CreateTimer(90f, () => { timer = false; });
                    }
                }
            }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}