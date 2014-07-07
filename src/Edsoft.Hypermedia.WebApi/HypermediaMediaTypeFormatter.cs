using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using Edsoft.Hypermedia.Serializers;

namespace Edsoft.Hypermedia.WebApi
{
    public class HypermediaMediaTypeFormatter : MediaTypeFormatter
    {
        private static readonly IReadOnlyDictionary<string, ISerializer> Serializers = new ReadOnlyDictionary
            <string, ISerializer>(
            new Dictionary<string, ISerializer>()
            {
                {"application/json", new JsonSerializer()},
                {"application/hal+json", new HalSerializer()},
                {"application/vnd.hale+json", new HaleSerializer()}
            });

        private readonly List<IBuilderDescriptor> descriptors = new List<IBuilderDescriptor>();

        private readonly HttpRequestMessage requestMessage;

        public HypermediaMediaTypeFormatter(params IBuilderDescriptor[] descriptors) : this(null, descriptors)
        {
        }

        public HypermediaMediaTypeFormatter(HttpRequestMessage requestMessage, params IBuilderDescriptor[] descriptors)
        {
            this.requestMessage = requestMessage;

            this.descriptors.AddRange(descriptors);

            foreach (var serializer in Serializers)
            {
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(serializer.Key));
            }
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof (IRepresentationBuilder) || descriptors.Any(d => d.SupportsType(type));
        }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return new HypermediaMediaTypeFormatter(request, descriptors.ToArray());
        }

        public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var serializer = Serializers[content.Headers.ContentType.MediaType];

            // Support serializing of returned IRepresentationBuilders
            if (type == typeof(IRepresentationBuilder))
            {
                await WriteToStreamFromBuilder((IRepresentationBuilder)value, writeStream, serializer);
            }
            else if (requestMessage != null)
            {
                // Support a matching Descriptor
                var matchingDescriptor = descriptors.Single(d => d.SupportsType(type));

                var requestContext = (HttpRequestContext)requestMessage.Properties[HttpPropertyKeys.RequestContextKey];
                var builder = matchingDescriptor.BuildForType(type, value, requestContext);
                await WriteToStreamFromBuilder(builder, writeStream, serializer);

            }
        }

        private static async Task WriteToStreamFromBuilder(IRepresentationBuilder builder, Stream writeStream, ISerializer serializer)
        {
            using (var writer = new StreamWriter(writeStream))
            {
                await writer.WriteAsync(serializer.Serialize(builder.ToRepresentation()));
            }
        }
    }
}
