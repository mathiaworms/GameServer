using GameServerCore;
using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Enums;
using System.Numerics;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;

namespace Spells
{
    public class HallucinateFull : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            TriggersSpellCasts = true
            // TODO
        };

        public float petTimeAlive = 0.00f;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellCast.AddListener(this, spell, ExecuteSpell);
            LogDebug("haullucinate!");
        }
        public void ExecuteSpell(ISpell spell)
        {
        }
        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            spellx = spell;
            owner.StopMovement();
            CreateTimer(1.3f, () => { owner.SetStatus(StatusFlags.Targetable, true); });
            CreateTimer(0.3f, () => { owner.SetStatus(StatusFlags.Targetable, false); });
            var Champs = GetChampionsInRange(owner.Position, 50000, true);
            foreach (IChampion player in Champs)
            {
                CreateTimer(1.3f, () => { owner.SetHealthbarVisibility((int)player.GetPlayerId(), owner, true); });
                if (player.Team.Equals(owner.Team))
                {
                    CreateTimer(1.3f, () => { owner.SetInvisible((int)player.GetPlayerId(), owner, 1f, 0.1f); });
                    CreateTimer(0.3f, () => { owner.SetInvisible((int)player.GetPlayerId(), owner, 0.5f, 0.1f); });
                }
                if (!(player.Team.Equals(owner.Team)))
                {
                    if (player.IsAttacking)
                    {
                        player.CancelAutoAttack(false);
                    }
                    CreateTimer(1.3f, () => { owner.SetInvisible((int)player.GetPlayerId(), owner, 1f, 0.1f); });
                    CreateTimer(0.3f, () => 
                    {
                        owner.SetInvisible((int)player.GetPlayerId(), owner, 0f, 0.1f);
                        owner.SetHealthbarVisibility((int)player.GetPlayerId(), owner, false);
                    });
                }
            }
        }

        public void OnSpellCast(ISpell spell)
        {

        }
        ISpell spellx;
        static internal IMinion m;
        bool makeminion = false;
        public void OnSpellPostCast(ISpell spell)
        {
            //IMinion m = AddMinion((IChampion)spell.CastInfo.Owner, "Shaco", "Shaco", spell.CastInfo.Owner.Position);
            CreateTimer(1.0f, () =>
            {
                //    var owner = spell.CastInfo.Owner;
                //    var ownerPos = owner.Position;
                //    var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
                //    var to = Vector2.Normalize(ownerPos - spellPos);
                //             IMinion m = AddMinion((IChampion)owner, "Shaco", "Shaco", new Vector2(ownerPos.X - to.X * 175f, ownerPos.Y - to.Y * 175f));
                //    FaceDirection(ownerPos, m);
                makeminion = true;
            });

            CreateTimer(1.5f, () => { spell.CastInfo.Owner.SetSpell("Hallucinate", 3, true); });

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
            if (makeminion == true)
            {
                var owner = spellx.CastInfo.Owner;
                var ownerPos = owner.Position;
                var spellPos = new Vector2(spellx.CastInfo.TargetPosition.X, spellx.CastInfo.TargetPosition.Z);
                var to = Vector2.Normalize(ownerPos - spellPos);
                LogDebug("proc");
                m = AddMinion((IChampion)owner, "Shaco", "Shaco", new Vector2(ownerPos.X - to.X * 175f, ownerPos.Y - to.Y * 175f));
                FaceDirection(ownerPos, m);
                owner.StopMovement();
                makeminion = false;
                CreateTimer(18.0f, () => { m.TakeDamage(owner, 50000, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_INTERNALRAW, false); });
            }
        }
    }

    public class Hallucinate : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            LogDebug("haullucinate recast!");
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            LogDebug("LOL3!");
            if(HallucinateFull.m != null)
            {
                ForceMovement(HallucinateFull.m, "RUN", owner.Position, 500, 0, 0, 0);
            }
            CreateTimer(0.1f, () => { owner.Spells[3].LowerCooldown((float)100); });
            CreateTimer(19.0f, () => { spell.CastInfo.Owner.SetSpell("HallucinateFull", 3, true); });
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
