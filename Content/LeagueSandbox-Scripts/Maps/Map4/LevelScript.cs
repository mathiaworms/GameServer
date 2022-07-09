using System;
using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.Content;
using GameServerCore.Scripting.CSharp;
using LeagueSandbox.GameServer.Scripting.CSharp;
using LeagueSandbox.GameServer.GameObjects.Stats;
using static LeagueSandbox.GameServer.API.ApiMapFunctionManager;

namespace MapScripts.Map4
{
    public class CLASSIC : IMapScript
    {
        public IMapScriptMetadata MapScriptMetadata { get; set; } = new MapScriptMetadata
        {
            BaseGoldPerGoldTick = 0.95f,
            AIVars = new AIVars
            {
                StartingGold = 825.0f
            }
        };

        public virtual IGlobalData GlobalData { get; set; } = new GlobalData();
        public bool HasFirstBloodHappened { get; set; } = false;
        public long NextSpawnTime { get; set; } = 75 * 1000;
        public string LaneMinionAI { get; set; } = "LaneMinionAI";
        public Dictionary<TeamId, Dictionary<int, Dictionary<int, Vector2>>> PlayerSpawnPoints { get; }

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
            new Vector2(1350.854f, 4266.956f),
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
        }};

        //These minion modifiers will remain unused for the moment, untill i pull the spawning systems to MapScripts
        Dictionary<MinionSpawnType, IStatsModifier> MinionModifiers = new Dictionary<MinionSpawnType, IStatsModifier>
        {
            { MinionSpawnType.MINION_TYPE_MELEE, new StatsModifier() },
            { MinionSpawnType.MINION_TYPE_CASTER, new StatsModifier() },
            { MinionSpawnType.MINION_TYPE_CANNON, new StatsModifier() },
            { MinionSpawnType.MINION_TYPE_SUPER, new StatsModifier() },
        };

        //This function is executed in-between Loading the map structures and applying the structure protections. Is the first thing on this script to be executed
        public virtual void Init(Dictionary<GameObjectTypes, List<MapObject>> mapObjects)
        {
            MapScriptMetadata.MinionSpawnEnabled = IsMinionSpawnEnabled();
            AddSurrender(1200000.0f, 300000.0f, 30.0f);

            MinionModifiers[MinionSpawnType.MINION_TYPE_MELEE].GoldGivenOnDeath.FlatBonus = 0.5f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_MELEE].HealthPoints.FlatBonus = 20.0f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_MELEE].AttackDamage.FlatBonus = 1.0f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_MELEE].Armor.FlatBonus = 3.0f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_MELEE].MagicResist.FlatBonus = 1.25f;

            MinionModifiers[MinionSpawnType.MINION_TYPE_CASTER].GoldGivenOnDeath.FlatBonus = 0.2f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_CASTER].HealthPoints.FlatBonus = 7.5f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_CASTER].AttackDamage.FlatBonus = 1.0f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_CASTER].Armor.FlatBonus = 0.625f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_CASTER].MagicResist.FlatBonus = 1.0f;

            MinionModifiers[MinionSpawnType.MINION_TYPE_CANNON].GoldGivenOnDeath.FlatBonus = 1f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_CANNON].HealthPoints.FlatBonus = 27.0f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_CANNON].AttackDamage.FlatBonus = 3.0f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_CANNON].Armor.FlatBonus = 3.0f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_CANNON].MagicResist.FlatBonus = 3.0f;

            MinionModifiers[MinionSpawnType.MINION_TYPE_SUPER].GoldGivenOnDeath.FlatBonus = 1.0f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_SUPER].HealthPoints.FlatBonus = 200.0f;
            MinionModifiers[MinionSpawnType.MINION_TYPE_SUPER].AttackDamage.FlatBonus = 10.0f;

            CreateLevelProps.CreateProps();
            LevelScriptObjects.LoadObjects(mapObjects);
        }

        public void OnMatchStart()
        {
            LevelScriptObjects.OnMatchStart();
            NeutralMinionSpawn.InitializeCamps();
        }

        //This function gets executed every server tick
        public void Update(float diff)
        {
            LevelScriptObjects.OnUpdate(diff);
            NeutralMinionSpawn.OnUpdate(diff);

            float gameTime = GameTime();

            if (MapScriptMetadata.MinionSpawnEnabled)
            {
                if (_minionNumber > 0)
                {
                    // Spawn new Minion every 0.8s
                    if (gameTime >= NextSpawnTime + _minionNumber * 8 * 100)
                    {
                        if (SetUpLaneMinion())
                        {
                            _minionNumber = 0;
                            NextSpawnTime = (long)gameTime + MapScriptMetadata.SpawnInterval;
                        }
                        else
                        {
                            _minionNumber++;
                        }
                    }
                }
                else if (gameTime >= NextSpawnTime)
                {
                    SetUpLaneMinion();
                    _minionNumber++;
                }
            }

            if (!AllAnnouncementsAnnounced)
            {
                CheckInitialMapAnnouncements(gameTime);
            }
        }

        public void SpawnAllCamps()
        {
            NeutralMinionSpawn.ForceCampSpawn();
        }

        public Vector2 GetFountainPosition(TeamId team)
        {
            return LevelScriptObjects.FountainList[team].Position;
        }

        bool AllAnnouncementsAnnounced = false;
        List<EventID> AnnouncedEvents = new List<EventID>();
        public void CheckInitialMapAnnouncements(float time)
        {
            if (time >= 180.0f * 1000)
            {
                //The Altars have unlocked!
              //  NotifyWorldEvent(EventID.OnStartGameMessage4, 10);
               // AllAnnouncementsAnnounced = true;
            }
            else if (time >= 150.0f * 1000 && !AnnouncedEvents.Contains(EventID.OnStartGameMessage2))
            {
                // The Altars will unlock in 30 seconds
              //  NotifyWorldEvent(EventID.OnStartGameMessage2, 10);
               // AnnouncedEvents.Add(EventID.OnStartGameMessage2);

            }
            else if (time >= 75.0f * 1000 && !AnnouncedEvents.Contains(EventID.OnStartGameMessage3))
            {
                // Minions have Spawned
                NotifyWorldEvent(EventID.OnStartGameMessage3, 10);
                NotifyWorldEvent(EventID.OnNexusCrystalStart, 0);
                AnnouncedEvents.Add(EventID.OnStartGameMessage3);
            }
            else if (time >= 30.0f * 1000 && !AnnouncedEvents.Contains(EventID.OnStartGameMessage1))
            {
                // Welcome to the Twisted Tree Line!
                NotifyWorldEvent(EventID.OnStartGameMessage1, 10);
                AnnouncedEvents.Add(EventID.OnStartGameMessage1);
            }
        }

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

        public int _minionNumber;
        public int _cannonMinionCount;
        public bool SetUpLaneMinion()
        {
            int cannonMinionCap = 2;
            foreach (var barrack in LevelScriptObjects.SpawnBarracks)
            {
                TeamId opposed_team = barrack.Value.GetOpposingTeamID();
                TeamId barrackTeam = barrack.Value.GetTeamID();
                LaneID lane = barrack.Value.GetSpawnBarrackLaneID();
                IInhibitor inhibitor = LevelScriptObjects.InhibitorList[opposed_team][lane];
                Vector2 position = new Vector2(barrack.Value.CentralPoint.X, barrack.Value.CentralPoint.Z);
                bool isInhibitorDead = inhibitor.InhibitorState == InhibitorState.DEAD;
                Tuple<int, List<MinionSpawnType>> spawnWave = MinionWaveToSpawn(GameTime(), _cannonMinionCount, isInhibitorDead, LevelScriptObjects.AllInhibitorsAreDead[opposed_team]);
                cannonMinionCap = spawnWave.Item1;

                List<Vector2> waypoint = new List<Vector2>(MinionPaths[lane]);

                if (barrackTeam == TeamId.TEAM_PURPLE)
                {
                    waypoint.Reverse();
                }

                CreateLaneMinion(spawnWave.Item2, position, barrackTeam, _minionNumber, barrack.Value.Name, waypoint);
            }

            if (_minionNumber < 8)
            {
                return false;
            }

            if (_cannonMinionCount >= cannonMinionCap)
            {
                _cannonMinionCount = 0;
            }
            else
            {
                _cannonMinionCount++;
            }
            return true;
        }
    }
}
