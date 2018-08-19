﻿using System;
using GameServerCore.Logic.Domain;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits;

namespace LeagueSandbox.GameServer.Logic.GameObjects.Stats
{
    public abstract class Replication : IReplication
    {
        public class Replicate : IReplicate
        {
            public uint Value { get; set; }
            public bool IsFloat { get; set; }
            public bool Changed { get; set; }

            public void MarkAsUnchanged()
            {
                Changed = false;
            }
        }

        protected Replication(AttackableUnit owner)
        {
            Owner = owner;
            Update();
        }

        protected readonly AttackableUnit Owner;
        protected Stats Stats => Owner.Stats;

        public uint NetId => Owner.NetId;
        public Replicate[,] Values { get; private set; } = new Replicate[6, 32];
        public bool Changed { get; private set; }

        IReplicate[,] IReplication.Values => Values;

        private void DoUpdate(uint value, int primary, int secondary, bool isFloat)
        {
            if (Values[primary, secondary] == null)
            {
                Values[primary, secondary] = new Replicate
                {
                    Value = value,
                    IsFloat = isFloat,
                    Changed = true
                };
                Changed = true;
            }
            else if (Values[primary, secondary].Value != value)
            {
                Values[primary, secondary].IsFloat = isFloat;
                Values[primary, secondary].Value = value;
                Values[primary, secondary].Changed = true;
                Changed = true;
            }
        }

        protected void UpdateUint(uint value, int primary, int secondary)
        {
            DoUpdate(value, primary, secondary, false);
        }

        protected void UpdateInt(int value, int primary, int secondary)
        {
            DoUpdate((uint)value, primary, secondary, false);
        }

        protected void UpdateBool(bool value, int primary, int secondary)
        {
            DoUpdate(value ? 1u : 0u, primary, secondary, false);
        }

        protected void UpdateFloat(float value, int primary, int secondary)
        {
            DoUpdate(BitConverter.ToUInt32(BitConverter.GetBytes(value), 0), primary, secondary, true);
        }

        public void MarkAsUnchanged()
        {
            foreach (var x in Values)
            {
                x?.MarkAsUnchanged();
            }

            Changed = false;
        }

        public abstract void Update();
    }
}