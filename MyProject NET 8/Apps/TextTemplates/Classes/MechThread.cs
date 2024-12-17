﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyProject_NET_8.TextTemplates
{
    internal class MechThread
    {
        public float MinValue { get; private set; }

        public float MaxValue { get; private set; }

        public string Designation { get; private set; }

        public int WrenchSize { get; private set; }

        [JsonConstructor]
        public MechThread(float minValue, float maxValue, string designation, int wrenchSize)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Designation = designation;
            WrenchSize = wrenchSize;
        }
    }
}