using System;

namespace Edsoft.Hypermedia.Serializers
{
    public interface ISerializer
    {
        string ContentType { get; }
        string Serialize(HypermediaRepresentation representor);
        IRepresentationBuilder DeserializeToNewBuilder(string message, Func<IRepresentationBuilder> builderFactoryMethod);
    }
}