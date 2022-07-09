using GameServerCore;
using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    public class DravenSpinning : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnKill.AddListener(this, owner, AddStackOnKill, false);
        }

        public void AddStackOnKill(IDeathData dat)
        {
            if(dat.Unit is IChampion)
            {
                LogDebug(" KILLED CHAMPION");

                if (_owner.GetBuffWithName("DravenPassiveStacks") != null)
                {
                    var DoubleGold = _owner.GetBuffWithName("DravenPassiveStacks").StackCount * 2;
                    var BaseGold = 50;
                    _owner.Stats.Gold += DoubleGold + BaseGold;
                    _owner.GetBuffWithName("DravenPassiveStacks").DeactivateBuff();
                }

            }
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        IObjAiBase _owner;
        ISpell _spell;

        bool fixCrash = false;

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {

            //FOR SOME REASON
            // IT CRASHES IF I DON'T DO THIS.
            // UNLUCKY!

            if(fixCrash == false)
            {
                ApiEventManager.OnLaunchAttack.AddListener(this, owner, OnLaunchAttack, false);
                fixCrash = true;
            }

            _owner = owner;
            _spell = spell;
            AddBuff("DravenSpinning", 5.8f, 1, spell, owner, owner);
        }

        public void RemoveStacks(int var)
        {
            int x = 0;
            foreach (var swag in _owner.GetBuffsWithName("DravenSpinning"))
            {
                if (x < var)
                {
                    x++;
                    swag.DeactivateBuff();
                }
            }
        }

        bool add1buff = false;
        bool add1buffPassive = false;
        //Draven_Base_Q_reticle.troy
        //Draven_Base_Q_reticle_self.troy
        public void ReturnAttack(IAttackableUnit unit, bool crit)
        {
            var pos = GetPointFromUnit(_owner, _owner.Stats.MoveSpeed.Total / 1.5f);
            AddParticle(_owner, null, "Draven_Base_Q_catch_indicator.troy", pos, lifetime: 1.0f);
            AddParticle(_owner, null, "Draven_Base_Q_reticle_self.troy", pos, lifetime: 1.0f);
            AddParticle(_owner, null, "Draven_Base_Q_reticle.troy", pos, lifetime: 1.0f);
            CreateTimer(0.5f, () => { SpellCast(_owner, 2, SpellSlotType.ExtraSlots, pos, pos, false, unit.Position); });
            CreateTimer(1.0f, () => 
            { 
                if (Extensions.IsVectorWithinRange(_owner.Position, pos, 200))
                {
                    add1buff = true;
                    add1buffPassive = true;
                }
            });
        }

        public void OnLaunchAttack(ISpell spell)
        {
            float damage = spell.CastInfo.Owner.Stats.AttackDamage.Total;
            if (_owner.HasBuff("DravenSpinning"))
            {
                if(_owner.TargetUnit is IBaseTurret)
                {
                    LogDebug("yo");
                    SpellCast(_owner, 2, SpellSlotType.ExtraSlots, spell.CastInfo.Targets[0].Unit.Position, spell.CastInfo.Targets[0].Unit.Position, false, Vector2.Zero);
                    RemoveStacks(1);
                    add1buff = true;
                }
                else
                {
                    // CAST AUTO
                    SpellCast(_owner, 2, SpellSlotType.ExtraSlots, spell.CastInfo.Targets[0].Unit.Position, spell.CastInfo.Targets[0].Unit.Position, false, Vector2.Zero);
                    // REMOVE STACKS
                    int i = 0;
                    while (i < _owner.GetBuffWithName("DravenSpinning").StackCount)
                    {
                        i++;
                        if (i == _owner.GetBuffWithName("DravenSpinning").StackCount)
                        {
                            RemoveStacks(1);
                        }
                    }
                    ApiEventManager.OnHitUnitByAnother.AddListener(this, _owner, ReturnAttack, true);
                }
            }
            else
            {
                SpellCast(spell.CastInfo.Owner, 2, SpellSlotType.ExtraSlots, spell.CastInfo.Targets[0].Unit.Position, spell.CastInfo.Targets[0].Unit.Position, false, Vector2.Zero);
            }
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell, ChannelingStopSource source)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
            if(add1buff == true)
            {
                AddBuff("DravenSpinning", 5.8f, 1, _spell, _owner, _owner);
                add1buff = false;
            }
            if (add1buffPassive == true)
            {
                AddBuff("DravenPassiveStacks", float.MaxValue, 1, _spell, _owner, _owner, true);
                add1buffPassive = false;
            }
        }
    }

    //public class DravenAttackP_L : ISpellScript
    //{
    //    public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
    //    {
    //        TriggersSpellCasts = true
    //    };
    //
    //    public void OnActivate(IObjAiBase owner, ISpell spell)
    //    {
    //        ApiEventManager.OnSpellHit.AddListener(this, spell, OnLaunchAttack, false);
    //    }
    //
    //    public void OnDeactivate(IObjAiBase owner, ISpell spell)
    //    {
    //    }
    //
    //    public void OnLaunchAttack(ISpell spell, IAttackableUnit unit, ISpellMissile mis, ISpellSector sec)
    //    {
    //    }
    //
    //    public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
    //    {
    //        //LogDebug("yo");
    //    }
    //
    //    public void OnSpellCast(ISpell spell)
    //    {
    //    }
    //
    //    public void OnSpellPostCast(ISpell spell)
    //    {
    //    }
    //
    //    public void OnSpellChannel(ISpell spell)
    //    {
    //    }
    //
    //    public void OnSpellChannelCancel(ISpell spell, ChannelingStopSource source)
    //    {
    //    }
    //
    //    public void OnSpellPostChannel(ISpell spell)
    //    {
    //    }
    //
    //    public void OnUpdate(float diff)
    //    {
    //    }
    //}

}