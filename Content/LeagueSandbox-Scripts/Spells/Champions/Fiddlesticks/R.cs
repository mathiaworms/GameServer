using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System.Numerics;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using System.Collections.Generic;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell.Missile;


namespace Spells
{
    public class Crowstorm : ISpellScript
    {
        IBuff thisBuff;
        public ISpellSector DamageSector;

         public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true,
            CastingBreaksStealth = true,
            DoesntBreakShields = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = true,
            SpellToggleSlot = 4
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {

        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
         
             var current = new Vector2(owner.Position.X, owner.Position.Y);
            var to = start - current;
            Vector2 trueCoords;

            if (to.Length() > 800)
            {
                to = Vector2.Normalize(to);
                var range = to * 800;
                trueCoords = current + range;
            }
            else
            {
                trueCoords = start;
            }

            owner.FaceDirection(new Vector3(to.X, 0.0f, to.Y));
            owner.StopChanneling(ChannelingStopCondition.Cancel, ChannelingStopSource.Move);


            TeleportTo(owner, trueCoords.X, trueCoords.Y);


            AddBuff("Crowstorm", 5, 1, spell, owner, owner, true);
            
        }

        public void OnSpellCast(ISpell spell)
        {
           
        }
       
      public void OnSpellPostCast(ISpell spell)
        {
          
        }
        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
         


        }
         private void ApplyAuraDamage(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            
         
        }
        public void OnSpellChannel(ISpell spell)
        {
        }

        public void OnSpellChannelCancel(ISpell spell)
        {
        }

        public void OnSpellPostChannel(ISpell spell)
        {
        }

        public void OnUpdate(float diff)
        {
            


          
        }
    }
}

 