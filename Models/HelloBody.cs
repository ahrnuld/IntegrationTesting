using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HelloBody
    {
        public HelloBody(string name)
        {
            this.Name = name;
        }

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }


    }
}
