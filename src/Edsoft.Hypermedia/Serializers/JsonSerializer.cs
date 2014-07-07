using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Edsoft.Hypermedia.Serializers
{
    public class JsonSerializer : ISerializer
    {
        public virtual IEnumerable<string> IgnoredAttributes { get { return Enumerable.Empty<string>(); } }
        public virtual string ContentType { get { return "application/json"; } }
        
        public virtual string Serialize(HypermediaRepresentation representor)
        {
            if (representor == null)
            {
                throw new ArgumentNullException("representor");
            }

            return SerializeAttributesToJObject(representor).ToString();
        }

        protected JObject SerializeAttributesToJObject(HypermediaRepresentation representor)
        {
            var jObject = new JObject();

            if (representor.Attributes != null)
            {
                foreach (var property in representor.Attributes.Properties().Where(p => !IgnoredAttributes.Contains(p.Name)))
                {
                    jObject.Add(property.Name, property.Value);
                }
            }

            return jObject;
        }

        public virtual IRepresentationBuilder DeserializeToNewBuilder(string message, Func<IRepresentationBuilder> builderFactoryMethod)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (builderFactoryMethod == null)
            {
                throw new ArgumentNullException("builderFactoryMethod");
            }

            var document = JObject.Parse(message);

            var builder = builderFactoryMethod();

            SetAttributes(document, builder);

            return builder;
        }

        protected void SetAttributes(JObject document, IRepresentationBuilder builder)
        {
            builder.SetAttributes(document);
        }
    }
}