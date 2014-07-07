using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Edsoft.Hypermedia
{
    public class HypermediaRepresentation
    {
        public string SelfLink { get; set; }

        public JObject Attributes { get; set; }

        public IList<HypermediaTransition> Transitions { get; set; }

        public Dictionary<string, IList<HypermediaRepresentation>> EmbeddedResources { get; private set; }

        public ICollection<HypermediaRepresentation> Collection { get; private set; } 

        public HypermediaRepresentation()
        {
            Transitions = new List<HypermediaTransition>();
            EmbeddedResources = new Dictionary<string, IList<HypermediaRepresentation>>();
            Collection = new List<HypermediaRepresentation>();
        }

        public T ToObject<T>()
        {
            return Attributes.ToObject<T>();
        }

        public void SetAttributesFromObject(object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            Attributes = JObject.FromObject(data);
        }
    }
}
