using System;
using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using GameServerCore.Maps;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.Buildings.AnimatedBuildings;
using LeagueSandbox.GameServer.GameObjects.Other;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects;
using GameServerCore.Domain.GameObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using GameServerCore.Maps;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.Buildings.AnimatedBuildings;
using LeagueSandbox.GameServer.GameObjects.Other;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace MapScripts
{
    public class Map4 : IMapScript
    {
        public bool EnableBuildingProtection { get; set; } = true;

        //General Map variable
        private IMapScriptHandler _map;

        //Stuff about minions
        public bool SpawnEnabled { get; set; }
        public long FirstSpawnTime { get; set; } = 90 * 1000;
        public long NextSpawnTime { get; set; } = 90 * 1000;
        public long SpawnInterval { get; set; } = 30 * 1000;
        public bool MinionPathingOverride { get; set; } = false;
        public List<IMonsterCamp> JungleCamps { get; set; }

        //General things that will affect players globaly, such as default gold per-second, Starting gold....
        public float GoldPerSecond { get; set; } = 1.9f;
        public float StartingGold { get; set; } = 825.0f;
        public bool HasFirstBloodHappened { get; set; } = false;
        public bool IsKillGoldRewardReductionActive { get; set; } = true;
        public int BluePillId { get; set; } = 2001;
        public long FirstGoldTime { get; set; } = 90 * 1000;

        //Tower type enumeration might vary slightly from map to map, so we set that up here
        public TurretType GetTurretType(int trueIndex, LaneID lane, TeamId teamId)
        {
            TurretType returnType = TurretType.NEXUS_TURRET;
            switch (trueIndex)
            {
                case 1:
                case 6:
                case 7:
                    returnType = TurretType.INHIBITOR_TURRET;
                    break;
                case 2:
                    returnType = TurretType.INNER_TURRET;
                    break;
            }

            if (trueIndex == 1 && lane == LaneID.MIDDLE)
            {
                returnType = TurretType.NEXUS_TURRET;
            }
            return returnType;
        }

        //Nexus models
        public Dictionary<TeamId, string> NexusModels { get; set; } = new Dictionary<TeamId, string>
        {
            {TeamId.TEAM_BLUE, "OrderNexus" },
            {TeamId.TEAM_PURPLE, "ChaosNexus" }
        };
        //Inhib models
        public Dictionary<TeamId, string> InhibitorModels { get; set; } = new Dictionary<TeamId, string>
        {
            {TeamId.TEAM_BLUE, "OrderInhibitor" },
            {TeamId.TEAM_PURPLE, "ChaosInhibitor" }
        };
        //Tower Models
        public Dictionary<TeamId, Dictionary<TurretType, string>> TowerModels { get; set; } = new Dictionary<TeamId, Dictionary<TurretType, string>>
        {
            {TeamId.TEAM_BLUE, new Dictionary<TurretType, string>
            {
              {TurretType.FOUNTAIN_TURRET, "OrderTurretShrine" },
                {TurretType.NEXUS_TURRET, "OrderTurretAngel" },
                {TurretType.INHIBITOR_TURRET, "OrderTurretDragon" },
                {TurretType.INNER_TURRET, "OrderTurretNormal2" },
                {TurretType.OUTER_TURRET, "OrderTurretNormal" },
            } },
            {TeamId.TEAM_PURPLE, new Dictionary<TurretType, string>
            {
                {TurretType.FOUNTAIN_TURRET, "ChaosTurretShrine" },
                {TurretType.NEXUS_TURRET, "ChaosTurretNormal" },
                {TurretType.INHIBITOR_TURRET, "ChaosTurretGiant" },
                {TurretType.INNER_TURRET, "ChaosTurretWorm2" },
                {TurretType.OUTER_TURRET, "ChaosTurretWorm" },
            } }
        };

        public Dictionary<MonsterSpawnType, string> MonsterModels { get; set; } = new Dictionary<MonsterSpawnType, string>
        {
            {MonsterSpawnType.WORM,"Worm"},
            {MonsterSpawnType.DRAGON, "Dragon"},
            {MonsterSpawnType.ELDER_LIZARD, "LizardElder"}, {MonsterSpawnType.YOUNG_LIZARD_ELDER, "YoungLizard"},
            {MonsterSpawnType.ANCIENT_GOLEM, "AncientGolem" }, {MonsterSpawnType.YOUNG_LIZARD_ANCIENT, "YoungLizard"},
            {MonsterSpawnType.GREAT_WRAITH, "GreatWraith" },
            {MonsterSpawnType.GIANT_WOLF, "GiantWolf" }, {MonsterSpawnType.WOLF, "Wolf"},
            {MonsterSpawnType.GOLEM, "Golem" }, {MonsterSpawnType.LESSER_GOLEM, "SmallGolem"},
            {MonsterSpawnType.WRAITH, "Wraith" }, {MonsterSpawnType.LESSER_WRAITH, "LesserWraith"}
        };

        //Turret Items
        public Dictionary<TurretType, int[]> TurretItems { get; set; } = new Dictionary<TurretType, int[]>
        {
            { TurretType.OUTER_TURRET, new[] { 1500, 1501, 1502, 1503 } },
            { TurretType.INNER_TURRET, new[] { 1500, 1501, 1502, 1503, 1504 } },
            { TurretType.INHIBITOR_TURRET, new[] { 1501, 1502, 1503, 1505 } },
            { TurretType.NEXUS_TURRET, new[] { 1501, 1502, 1503, 1505 } }
        };

        //List of every path minions will take, separated by team and lane
        public Dictionary<LaneID, List<Vector2>> MinionPaths { get; set; } = new Dictionary<LaneID, List<Vector2>>
        {
                //Pathing coordinates for Top lane
                {LaneID.TOP, new List<Vector2> {
            new Vector2(1960f, 6684f),
            new Vector2(1410.101f, 8112.747f),
            new Vector2(2153f, 9228.292f),
            new Vector2(3254.654f, 8801.554f),
            new Vector2(3967.261f, 7689.167f),
            new Vector2(5187.636f, 7075.806f),
            new Vector2(6742.38f, 6797.492f),
            new Vector2(8587.43f, 7340.201f),
            new Vector2(9798.584f, 8079.067f),
            new Vector2(10926.82f, 9423.166f),
            new Vector2(12042.19f, 8413.8f),
            new Vector2(11758.64f, 6586.07f)}
                },



                //Pathing coordinates for Bot lane
                {LaneID.BOTTOM, new List<Vector2> {
            new Vector2(1960f, 5977f),
            new Vector2(1340.854f, 4566.956f),
            new Vector2(2218.08f, 3771.371f),
            new Vector2(3822.184f, 3735.409f),
            new Vector2(5268.443f, 3410.666f),
            new Vector2(6766.503f, 3435.633f),
            new Vector2(8776.482f, 3423.031f),
            new Vector2(10120.18f, 3735.568f),
            new Vector2(11622.86f, 3707.629f),
            new Vector2(12086.33f, 4369.503f),
            new Vector2(11797.77f, 6077.822f) }
                }
       };
        //List of every wave type
        public Dictionary<string, List<MinionSpawnType>> MinionWaveTypes = new Dictionary<string, List<MinionSpawnType>>
        { {"RegularMinionWave", new List<MinionSpawnType>
        {
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER }
        },
        {"CannonMinionWave", new List<MinionSpawnType>{
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CANNON,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER }
        },
        {"SuperMinionWave", new List<MinionSpawnType>{
            MinionSpawnType.MINION_TYPE_SUPER,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER }
        },
        {"DoubleSuperMinionWave", new List<MinionSpawnType>{
            MinionSpawnType.MINION_TYPE_SUPER,
            MinionSpawnType.MINION_TYPE_SUPER,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER }
        }
        };

        //Here you setup the conditions of which wave will be spawned
        public Tuple<int, List<MinionSpawnType>> MinionWaveToSpawn(float gameTime, int cannonMinionCount, bool isInhibitorDead, bool areAllInhibitorsDead)
        {
            var cannonMinionTimestamps = new List<Tuple<long, int>>
            {
                new Tuple<long, int>(0, 2),
                new Tuple<long, int>(20 * 60 * 1000, 1),
                new Tuple<long, int>(35 * 60 * 1000, 0)
            };
            var cannonMinionCap = 2;

            foreach (var timestamp in cannonMinionTimestamps)
            {
                if (gameTime >= timestamp.Item1)
                {
                    cannonMinionCap = timestamp.Item2;
                }
            }
            var list = "RegularMinionWave";
            if (cannonMinionCount >= cannonMinionCap)
            {
                list = "CannonMinionWave";
            }

            if (isInhibitorDead)
            {
                list = "SuperMinionWave";
            }

            if (areAllInhibitorsDead)
            {
                list = "DoubleSuperMinionWave";
            }
            return new Tuple<int, List<MinionSpawnType>>(cannonMinionCap, MinionWaveTypes[list]);
        }

        //Minion models for this map
        public Dictionary<TeamId, Dictionary<MinionSpawnType, string>> MinionModels { get; set; } = new Dictionary<TeamId, Dictionary<MinionSpawnType, string>>
        {
            {TeamId.TEAM_BLUE, new Dictionary<MinionSpawnType, string>{
                {MinionSpawnType.MINION_TYPE_MELEE, "Blue_Minion_Basic"},
                {MinionSpawnType.MINION_TYPE_CASTER, "Blue_Minion_Wizard"},
                {MinionSpawnType.MINION_TYPE_CANNON, "Blue_Minion_MechCannon"},
                {MinionSpawnType.MINION_TYPE_SUPER, "Blue_Minion_MechMelee"}
            }},
            {TeamId.TEAM_PURPLE, new Dictionary<MinionSpawnType, string>{
                {MinionSpawnType.MINION_TYPE_MELEE, "Red_Minion_Basic"},
                {MinionSpawnType.MINION_TYPE_CASTER, "Red_Minion_Wizard"},
                {MinionSpawnType.MINION_TYPE_CANNON, "Red_Minion_MechCannon"},
                {MinionSpawnType.MINION_TYPE_SUPER, "Red_Minion_MechMelee"}
            }}
        };

        //This function is executed in-between Loading the map structures and applying the structure protections. Is the first thing on this script to be executed
        public void Init(IMapScriptHandler map)
        {
            _map = map;

            SpawnEnabled = map.IsMinionSpawnEnabled();
            map.AddSurrender(1200000.0f, 300000.0f, 30.0f);

            //Due to riot's questionable map-naming scheme some towers are missplaced into other lanes during outomated setup, so we have to manually fix them.
            map.ChangeTowerOnMapList("Turret_T1_C_07_A", TeamId.TEAM_BLUE, LaneID.MIDDLE, LaneID.BOTTOM);
            map.ChangeTowerOnMapList("Turret_T1_C_06_A", TeamId.TEAM_BLUE, LaneID.MIDDLE, LaneID.TOP);

            // Announcer events
            map.AddAnnouncement(FirstSpawnTime - 90 * 1000, Announces.THIRY_SECONDS_TO_MINIONS_SPAWN, true); // 30 seconds until minions spawn
            map.AddAnnouncement(FirstSpawnTime, Announces.MINIONS_HAVE_SPAWNED, false); // Minions have spawned (90 * 1000)
            map.AddAnnouncement(FirstSpawnTime, Announces.MINIONS_HAVE_SPAWNED2, false); // Minions have spawned [2] (90 * 1000)
        
            //Map props
       
        }

        public void OnMatchStart()
        {
            JungleCamps = new List<IMonsterCamp>() {
                //Neutral,
                _map.CreateMonsterCamp(MonsterCampType.DRAGON, new Vector2(6748.594f, 7878f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(6748.594f, 7878f), MonsterSpawnType.DRAGON} } ,
                600.0f, new Vector2(6748.594f, 7878f)),

                _map.CreateMonsterCamp(MonsterCampType.BLUE_RED_BUFF, new Vector2(6748.594f, 5000.0f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(6748.594f, 5000.0f), MonsterSpawnType.ELDER_LIZARD}  } ,
                180.0f, new Vector2(6748.594f, 5000.0f)),

                //BLUE TEAM
                _map.CreateMonsterCamp(MonsterCampType.BLUE_GOLEMS, new Vector2(4750.874f, 4800.0f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(4650.874f, 4800.0f), MonsterSpawnType.GOLEM},
                { new Vector2(4750.874f, 4800.0f), MonsterSpawnType.LESSER_GOLEM}},
                100.0f, new Vector2 (4750.874f, 4800.0f)),

                _map.CreateMonsterCamp(MonsterCampType.BLUE_WRAITHS, new Vector2(5514.874f, 9423.166f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(5514.874f, 9323.166f), MonsterSpawnType.WRAITH},
                { new Vector2(5514.874f, 9423.166f), MonsterSpawnType.LESSER_WRAITH},
                { new Vector2(5414.874f, 9423.166f), MonsterSpawnType.LESSER_WRAITH}},
                100.0f, new Vector2 (5514.874f, 9423.166f)),

                _map.CreateMonsterCamp(MonsterCampType.BLUE_WOLVES, new Vector2(4350.874f, 9423.166f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(4350.874f, 9423.166f), MonsterSpawnType.GIANT_WOLF},
                { new Vector2(4250.874f, 9423.166f), MonsterSpawnType.WOLF}
                },
                100.0f, new Vector2(4350.874f, 9423.166f)),
                
                 // RED TEAM
                _map.CreateMonsterCamp(MonsterCampType.RED_GOLEMS, new Vector2(8750.874f, 4800.0f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(8650.874f, 4800.0f), MonsterSpawnType.GOLEM},
                { new Vector2(8750.874f, 4800.0f), MonsterSpawnType.LESSER_GOLEM}},
                100.0f, new Vector2(8750.874f, 4800.0f)),

                _map.CreateMonsterCamp(MonsterCampType.RED_WRAITHS, new Vector2(7940.874f, 9423.166f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(7940.874f, 9323.166f), MonsterSpawnType.WRAITH},
                { new Vector2(7940.874f, 9423.166f), MonsterSpawnType.LESSER_WRAITH},
                { new Vector2(7840.874f, 9423.166f), MonsterSpawnType.LESSER_WRAITH}},
                100.0f, new Vector2(7940.874f, 9423.166f)),

                _map.CreateMonsterCamp(MonsterCampType.RED_WOLVES, new Vector2(9200.874f, 9423.166f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(9200.874f, 9423.166f), MonsterSpawnType.GIANT_WOLF},
                { new Vector2(9250.874f, 9423.166f), MonsterSpawnType.WOLF}},

                100.0f, new Vector2(9200.874f, 9423.166f))
            };
        }


        //This function gets executed every server tick
        public void Update(float diff)
        {
            foreach (var camp in JungleCamps)
            {
                if (!camp.IsAlive())
                {
                    camp.RespawnCooldown -= diff;

                    if (camp.RespawnCooldown <= 0)
                    {
                        camp.Spawn();
                        camp.RespawnCooldown = GetMonsterSpawnInterval(camp.CampType);
                    }
                }
            }
        }
        public int GetMonsterSpawnInterval(MonsterCampType monsterType)
        {
            switch (monsterType)
            {
                case MonsterCampType.BLUE_RED_BUFF:
                    return 90;
                case MonsterCampType.DRAGON:
                    return 300;
                default:
                    return 50;
            }
        }

        public float GetGoldFor(IAttackableUnit u)
    {
        if (!(u is ILaneMinion m))
        {
            if (!(u is IMonster s))
            {
                if (!(u is IChampion c))
                {
                    return 0.0f;
                }

                var gold = 300.0f; //normal gold for a kill
                if (c.KillDeathCounter < 5 && c.KillDeathCounter >= 0)
                {
                    if (c.KillDeathCounter == 0)
                    {
                        return gold;
                    }

                    for (var i = c.KillDeathCounter; i > 1; --i)
                    {
                        gold += gold * 0.165f;
                    }

                    return gold;
                }

                if (c.KillDeathCounter >= 5)
                {
                    return 500.0f;
                }

                if (c.KillDeathCounter >= 0)
                    return 0.0f;

                var firstDeathGold = gold - gold * 0.085f;

                if (c.KillDeathCounter == -1)
                {
                    return firstDeathGold;
                }

                for (var i = c.KillDeathCounter; i < -1; ++i)
                {
                    firstDeathGold -= firstDeathGold * 0.2f;
                }

                if (firstDeathGold < 50)
                {
                    firstDeathGold = 50;
                }

                return firstDeathGold;
            }
        }
            if (u is ILaneMinion mi)
            {
                 var dic = new Dictionary<MinionSpawnType, float>
                     {
                         { MinionSpawnType.MINION_TYPE_MELEE, 19.8f + 0.2f * (int)(_map.GameTime() / (90 * 1000)) },
                         { MinionSpawnType.MINION_TYPE_CASTER, 16.8f + 0.2f * (int)(_map.GameTime() / (90 * 1000)) },
                         { MinionSpawnType.MINION_TYPE_CANNON, 40.0f + 0.5f * (int)(_map.GameTime() / (90 * 1000)) },
                         { MinionSpawnType.MINION_TYPE_SUPER, 40.0f + 1.0f * (int)(_map.GameTime() / (180 * 1000)) }
                      };

                if (!dic.ContainsKey(mi.MinionSpawnType))
                    {
                        return 0.0f;
                    }

                   return dic[mi.MinionSpawnType];
            }
            else if (u is IMonster mo)
            {
                var dic = new Dictionary<MonsterSpawnType, float>
                 {
                { MonsterSpawnType.WRAITH, 35.0f },
                { MonsterSpawnType.LESSER_WRAITH, 4.0f },
                { MonsterSpawnType.GIANT_WOLF, 40.0f },
                { MonsterSpawnType.WOLF, 8.0f },
                { MonsterSpawnType.GOLEM, 55.0f },
                { MonsterSpawnType.LESSER_GOLEM, 15.0f },
                { MonsterSpawnType.ANCIENT_GOLEM, 60.0f },
                { MonsterSpawnType.ELDER_LIZARD, 60.0f },
                { MonsterSpawnType.YOUNG_LIZARD_ANCIENT, 7.0f },
                { MonsterSpawnType.YOUNG_LIZARD_ELDER, 7.0f },
                { MonsterSpawnType.DRAGON, 150.0f },
                  };

                if (!dic.ContainsKey(mo.MinionSpawnType))
                {
                    return 0.0f;
                }

                return dic[mo.MinionSpawnType];
            }

            return 0.0f;
        }

            
    

        public float GetExperienceFor(IAttackableUnit u)
        {
           if ((u is ILaneMinion m))
            {
                var dic = new Dictionary<MinionSpawnType, float>
            {
                { MinionSpawnType.MINION_TYPE_MELEE, 64.0f },
                { MinionSpawnType.MINION_TYPE_CASTER, 32.0f },
                { MinionSpawnType.MINION_TYPE_CANNON, 92.0f },
                { MinionSpawnType.MINION_TYPE_SUPER, 97.0f }
            };

                if (!dic.ContainsKey(m.MinionSpawnType))
                {
                    return 0.0f;
                }

                return dic[m.MinionSpawnType];
            }

            else if ((u is IMonster mo))
            {
                var dic = new Dictionary<MonsterSpawnType, float>
            {
                { MonsterSpawnType.WRAITH, 90.0f },
                { MonsterSpawnType.LESSER_WRAITH, 20.0f },
                { MonsterSpawnType.GIANT_WOLF, 110.0f },
                { MonsterSpawnType.WOLF, 25.0f },
                { MonsterSpawnType.GOLEM, 140.0f },
                { MonsterSpawnType.LESSER_GOLEM, 40.0f },
                { MonsterSpawnType.ANCIENT_GOLEM, 260.0f },
                { MonsterSpawnType.ELDER_LIZARD, 260.0f },
                { MonsterSpawnType.YOUNG_LIZARD_ELDER, 20.0f },
                { MonsterSpawnType.YOUNG_LIZARD_ANCIENT, 20.0f },
                { MonsterSpawnType.DRAGON, 150.0f },
            };

                if (!dic.ContainsKey(mo.MinionSpawnType))
                {
                    return 0.0f;
                }

                return dic[mo.MinionSpawnType];
            }

            return 0.0f;
        }

        public void SetMinionStats(ILaneMinion m)
        {
            // Same for all minions
            m.Stats.MoveSpeed.BaseValue = 325.0f;

            switch (m.MinionSpawnType)
            {
                case MinionSpawnType.MINION_TYPE_MELEE:
                    m.Stats.CurrentHealth = 475.0f + 20.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.HealthPoints.BaseValue = 475.0f + 20.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.AttackDamage.BaseValue = 12.0f + 1.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.Range.BaseValue = 180.0f;
                    m.Stats.AttackSpeedFlat = 1.250f;
                    m.IsMelee = true;
                    break;
                case MinionSpawnType.MINION_TYPE_CASTER:
                    m.Stats.CurrentHealth = 279.0f + 7.5f * (int)(_map.GameTime() / (90 * 1000));
                    m.Stats.HealthPoints.BaseValue = 279.0f + 7.5f * (int)(_map.GameTime() / (90 * 1000));
                    m.Stats.AttackDamage.BaseValue = 23.0f + 1.0f * (int)(_map.GameTime() / (90 * 1000));
                    m.Stats.Range.BaseValue = 600.0f;
                    m.Stats.AttackSpeedFlat = 0.670f;
                    break;
                case MinionSpawnType.MINION_TYPE_CANNON:
                    m.Stats.CurrentHealth = 700.0f + 27.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.HealthPoints.BaseValue = 700.0f + 27.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.AttackDamage.BaseValue = 40.0f + 3.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.Range.BaseValue = 450.0f;
                    m.Stats.AttackSpeedFlat = 1.0f;
                    break;
                case MinionSpawnType.MINION_TYPE_SUPER:
                    m.Stats.CurrentHealth = 1500.0f + 200.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.HealthPoints.BaseValue = 1500.0f + 200.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.AttackDamage.BaseValue = 190.0f + 10.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.Range.BaseValue = 170.0f;
                    m.Stats.AttackSpeedFlat = 0.694f;
                    m.Stats.Armor.BaseValue = 30.0f;
                    m.Stats.MagicResist.BaseValue = -30.0f;
                    m.IsMelee = true;
                    break;
            }
        }
    }
}
