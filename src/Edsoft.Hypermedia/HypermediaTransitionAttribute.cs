using System.Collections.Generic;

namespace Edsoft.Hypermedia
{
    public class HypermediaTransitionAttribute : IAttributesContainer
    {
        public object Value { get; set; }
        public string ProfileUri { get; set; }
        public string JsonType { get; set; }
        public string DataType { get; set; }
        public IDictionary<string, HypermediaTransitionAttribute> Attributes { get; set; }
        public IDictionary<string, HypermediaTransitionAttribute> Parameters { get; set; }
        public HypermediaTransitionAttributeConstraint Constraint { get; set; }

        public HypermediaTransitionAttribute()
        {
            Attributes = new Dictionary<string, HypermediaTransitionAttribute>();
            Parameters = new Dictionary<string, HypermediaTransitionAttribute>();
        }
    }
}