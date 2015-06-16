using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CommonTypes
{
    [DataContract]
    public class Library
    {
        public Library()
        {
            Methods = new List<Method>();
        }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public List<Method> Methods { get; set; }

        public bool ContainsMethod(string name)
        {
            bool ret = false;
            Methods.ForEach(method =>
            {
                if (!String.Equals(method.Name, name, StringComparison.InvariantCultureIgnoreCase)) return;
                ret = true;
            });

            return ret;
        }

        public Method GetMethod(string name)
        {
            bool found = false;
            Method ret = null;
           
            Methods.ForEach(method =>
            {
                if (String.Equals(method.Name, name, StringComparison.InvariantCultureIgnoreCase) && !found)
                {
                    ret = method;
                    found = true;
                }
            });
            return ret;
        }
    }
}