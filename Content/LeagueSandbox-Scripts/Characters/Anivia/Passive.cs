using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Scripting.CSharp;

namespace Passives
{
    public class Rebirth_Marker : ICharScript
    {
        
        IObjAiBase _owner;
        IParticle particle;
        bool haspassive = false;
        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            haspassive = true;
            //CreateTimer(0.1f, () => { particle = AddParticle(owner, owner, "RighteousFuryHalo_buf.troy", owner.Position, lifetime: float.MaxValue, bone: "head"); });
            //ApiEventManager.OnHitUnit.AddListener(this, owner, TargetExecute, false);
            ApiEventManager.OnTakeDamageByAnother.AddListener(this, owner, TargetExecute, false);
            _owner = owner; 
        }

        
        bool timer = false;
        bool oldpos = false;
        Vector2 oldposV;
        
        private void TargetExecute(IAttackableUnit unit1, IAttackableUnit unit2)
        {
           if (haspassive == true)
            {
                if (unit1.Stats.CurrentHealth < unit1.Stats.HealthPoints.Total * 0.05f)
                {
                    var unit1champ = unit1 as IChampion;
                    if (timer != true)
                    {
                        timer = true;
                        oldpos = true;
                        unit1champ.Respawn();
                        TeleportTo(unit1champ, oldposV.X, oldposV.Y);
                        unit1champ.ChangeModel("AniviaEgg");
                        unit1champ.SetStatus(StatusFlags.CanCast, false);
                        unit1champ.SetStatus(StatusFlags.CanMove, false);
                        unit1champ.SetStatus(StatusFlags.CanAttack, false);
                        //particle.SetToRemove();
                        CreateTimer(6.0f, () => 
                        {
                            unit1champ.ChangeModel("Anivia");
                            unit1champ.SetStatus(StatusFlags.CanCast, true);
                            unit1champ.SetStatus(StatusFlags.CanMove, true);
                            unit1champ.SetStatus(StatusFlags.CanAttack, true);
                        });

                        //AddBuff("ZhonyasHourglassBuff", 4.0f, 1, unit1champ.GetSpell(0), unit1, unit1champ);
                        //AddParticle(_owner, _owner, "GuardianAngel_tar.troy", _owner.Position);
                        //unit1champ.TakeDamage(_owner, hpval, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false);
                       // CreateTimer(160f, () => { timer = false; });
                    }
                }
            } 
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {

            if (oldpos == false)
            {
                oldposV = _owner.Position;
            }
            if (oldpos == true)
            {
                oldpos = false;
            } 
        }
    }
}