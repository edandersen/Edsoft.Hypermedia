using System.Collections.Generic;

namespace Edsoft.Hypermedia
{
    public interface IAttributesContainer
    {
        IDictionary<string, HypermediaTransitionAttribute> Attributes { get; set; }
        IDictionary<string, HypermediaTransitionAttribute> Parameters { get; set; }
    }
}