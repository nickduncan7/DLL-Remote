using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CommonTypes
{
    [DataContract]
    public class Method
    {
        public Method()
        {
            Parameters = new List<Parameter>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<Parameter> Parameters { get; set; }

        [DataMember]
        public string ReturnType { get; set; }
    }
}