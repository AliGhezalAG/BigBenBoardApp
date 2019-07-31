using System;
using System.Runtime.Serialization;
using WiimoteLib;

namespace RestWCFServiceLibrary.WiiMote
{
    [@DataContract]
    public class WiiBoardWeightData
    {
        [@DataMember]
        public float weightKg { get; set; }
        [@DataMember]
        public string horodate { get; set; }
    }
}
