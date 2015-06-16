using System;
using System.Runtime.Serialization;

namespace CommonTypes
{
    [DataContract]
    public class Parameter
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Namespace { get; set; }

        [DataMember]
        public int Ordinal { get; set; }
    }
}