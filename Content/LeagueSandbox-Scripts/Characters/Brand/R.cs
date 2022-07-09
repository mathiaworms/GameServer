using GameServerCore.Domain.GameObjects;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;
using GameServerCore.Domain.GameObjects.Spell.Sector;
using GameServerCore.Enums;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.Scripting.CSharp;
using System;
using System.Numerics;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;

namespace Spells
{
    public class BrandWildfire : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            CastingBreaksStealth = true,
            DoesntBreakShields = true,
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = false,
            SpellDamageRatio = 1.0f,
            SpellFXOverrideSkins = new string[] { "FrostFireBrand" },
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Target
            }
        };

        private bool doOnce = false;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, false);
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
        }

        public void OnSpellCast(ISpell spell)
        {
        }

        public void OnSpellPostCast(ISpell spell)
        {
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            var owner = spell.CastInfo.Owner;
            var ap = owner.Stats.AbilityPower.Total * 0.5f;
            var damage = 50f + spell.CastInfo.SpellLevel * 100f + ap;

            //RemoveBuff(owner, "BrandWildfire");

            if (target is IObjAiBase aiTarget)
            {
                //AddBuff("BrandWildfire", 4.0f, 1, spell, owner, aiTarget);

                doOnce = false;

                var units = GetUnitsInRange(target.Position, 600.0f, true).FindAll(unit => unit != target && unit.Team != owner.Team && !(unit is IObjBuilding || unit is IBaseTurret));

                // Randomize units list.
                Random rng = new Random();
                int n = units.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    IAttackableUnit value = units[k];
                    units[k] = units[n];
                    units[n] = value;
                }

                foreach (var unit in units)
                {
                    if (unit == target || unit.Team == owner.Team || unit is IObjBuilding || unit is IBaseTurret)
                    {
                        continue;
                    }

                    if (!doOnce)
                    {
                        if (!unit.Status.HasFlag(StatusFlags.Stealthed))
                        {
                            if (HasBuff(target, "BrandWildfire"))
                            {
                                // Extra spell 5 no longer exists?
                                //SpellCast(owner, 4, SpellSlotType.ExtraSlots, true, unit, target.Position);
                            }
                            else
                            {
                                SpellCast(owner, 1, SpellSlotType.ExtraSlots, true, unit, target.Position);
                            }

                            doOnce = true;
                        }
                        else
                        {
                            if (unit.IsVisibleByTeam(owner.Team))
                            {
                                if (HasBuff(target, "BrandWildfire"))
                                {
                                    // Extra spell 5 no longer exists?
                                    //SpellCast(owner, 4, SpellSlotType.ExtraSlots, true, unit, target.Position);
                                }
                                else
                                {
                                    SpellCast(owner, 1, SpellSlotType.ExtraSlots, true, unit, target.Position);
                                }

                                doOnce = true;
                            }
                        }
                    }
                }

                if (HasBuff(target, "BrandWildfire"))
                {
                    if (owner.SkinID == 3)
                    {
                        AddParticleTarget(owner, target, "BrandConflagration_tar_frost.troy", target, flags: 0);
                    }
                    else
                    {
                        AddParticleTarget(owner, target, "BrandConflagration_tar.troy", target, flags: 0);
                    }

                    // BreakSpellShields(target);

                    AddBuff("BrandWildfire", 4.0f, 1, spell, target, owner);

                    target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
                }
                else
                {
                    // BreakSpellShields(target);

                    AddBuff("BrandWildfire", 4.0f, 1, spell, target, owner);

                    target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);

                    if (owner.SkinID == 3)
                    {
                        AddParticleTarget(owner, target, "BrandConflagration_tar_frost.troy", target, flags: 0);
                    }
                    else
                    {
                        AddParticleTarget(owner, target, "BrandConflagration_tar.troy", target, flags: 0);
                    }
                }
            }
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
        }
    }

    public class BrandWildfireMissile : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
        {
            DoesntBreakShields = true,
            TriggersSpellCasts = true,
            IsDamagingSpell = true,
            NotSingleTargetSpell = false,
            PersistsThroughDeath = true,
            SpellDamageRatio = 1.0f,
            SpellFXOverrideSkins = new string[] { "FrostFireBrand" },
            MissileParameters = new MissileParameters
            {
                Type = MissileType.Target,
            }
        };

        private bool doOnce = false;

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnSpellPreCast(IObjAiBase owner, ISpell spell, IAttackableUnit target, Vector2 start, Vector2 end)
        {
            LogInfo("BrandWildfireMissile OnSpellPreCast called.");
            ApiEventManager.OnSpellHit.AddListener(this, spell, TargetExecute, true);
        }

        public void TargetExecute(ISpell spell, IAttackableUnit target, ISpellMissile missile, ISpellSector sector)
        {
            LogInfo("BrandWildfireMissile TargetExecute called.");
            var owner = spell.CastInfo.Owner;
            var ap = owner.Stats.AbilityPower.Total * 0.5f;
            var damage = 50f + spell.CastInfo.SpellLevel * 100f + ap;

            AddBuff("CrashFixStacks", 4.0f, 1, spell, owner, owner);

            doOnce = false;

            if (owner.GetBuffWithName("CrashFixStacks").StackCount <= 5)
            {
                var units = GetUnitsInRange(target.Position, 600.0f, true).FindAll(unit => unit != target && unit.Team != owner.Team && !(unit is IObjBuilding || unit is IBaseTurret));

                // Randomize units list.
                Random rng = new Random();
                int n = units.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    IAttackableUnit value = units[k];
                    units[k] = units[n];
                    units[n] = value;
                }

                foreach (var unit in units)
                {
                    if (unit == target || unit.Team == owner.Team || unit is IObjBuilding || unit is IBaseTurret)
                    {
                        continue;
                    }

                    if (!doOnce)
                    {
                        if (!unit.Status.HasFlag(StatusFlags.Stealthed))
                        {
                            if (HasBuff(target, "BrandWildfire"))
                            {
                                // Extra spell 5 no longer exists?
                                //SpellCast(owner, 4, SpellSlotType.ExtraSlots, true, unit, target.Position);
                            }
                            else
                            {
                                SpellCast(owner, 1, SpellSlotType.ExtraSlots, true, unit, target.Position);
                            }

                            doOnce = true;
                        }
                        else
                        {
                            if (unit.IsVisibleByTeam(owner.Team))
                            {
                                if (HasBuff(target, "BrandWildfire"))
                                {
                                    // Extra spell 5 no longer exists?
                                    //SpellCast(owner, 4, SpellSlotType.ExtraSlots, true, unit, target.Position);
                                }
                                else
                                {
                                    SpellCast(owner, 1, SpellSlotType.ExtraSlots, true, unit, target.Position);
                                }

                                doOnce = true;
                            }
                        }
                    }
                }
            }

            if (HasBuff(target, "BrandWildfire"))
            {
                if (owner.SkinID == 3)
                {
                    AddParticleTarget(owner, target, "BrandConflagration_tar_frost.troy", target, flags: 0);
                }
                else
                {
                    AddParticleTarget(owner, target, "BrandConflagration_tar.troy", target, flags: 0);
                }

                // BreakSpellShields(target);

                AddBuff("BrandWildfire", 4.0f, 1, spell, target, owner);

                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);
            }
            else
            {
                // BreakSpellShields(target);

                AddBuff("BrandWildfire", 4.0f, 1, spell, target, owner);

                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELLAOE, false);

                if (owner.SkinID == 3)
                {
                    AddParticleTarget(owner, target, "BrandConflagration_tar_frost.troy", target, flags: 0);
                }
                else
                {
                    AddParticleTarget(owner, target, "BrandConflagration_tar.troy", target, flags: 0);
                }
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
        }
    }
}