using System;
using System.Runtime.Serialization;
using WiimoteLib;

namespace RestWCFServiceLibrary.WiiMote
{
    [@DataContract]
    public class WiiBoardData
    {
        [@DataMember]
        public float topLeftKg { get; set; }

        [@DataMember]
        public float topRightKg { get; set; }

        [@DataMember]
        public float bottomLeftKg { get; set; }

        [@DataMember]
        public float bottomRightKg { get; set; }

        [@DataMember]
        public float weightKg { get; set; }

        [@DataMember]
        public PointGravity gravity { get; set; }

        [@DataMember]
        public string Horodate { get; set; }

        [@DataMember]
        public double TIMESTAMP { get; set; }
    }

    [@DataContract]
    public class PointGravity
    {
        [@DataMember]
        public float X { get; set; }

        [@DataMember]
        public float Y { get; set; }
    }
}
