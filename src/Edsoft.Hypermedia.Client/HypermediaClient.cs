using System;
using System.Net.Http;
using System.Threading.Tasks;
using Edsoft.Hypermedia.Client.QuerySteps;
using Edsoft.Hypermedia.Serializers;

namespace Edsoft.Hypermedia.Client
{
    public class HypermediaClient
    {
        public ITransitionRequestHandler TransitionRequestHandler { get; private set; }

        public HypermediaClient(ITransitionRequestHandler transitionRequestHandler)
        {
            
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            TransitionRequestHandler = transitionRequestHandler;
        }

        public HypermediaClient(Uri baseAddress, ISerializer serializer) : this(new HttpClient{BaseAddress = baseAddress}, serializer)
        {
        }

        public HypermediaClient(HttpClient client, ISerializer serializer)
        {
            if (client == null) { throw new ArgumentNullException("client"); }
            if (client.BaseAddress == null) { throw new ArgumentException("HttpClient.BaseAddress must not be null."); }
            if (serializer == null) { throw new ArgumentNullException("serializer"); }

            TransitionRequestHandler = new HttpClientTransitionRequestHandler(client, serializer);
        }

        public IHypermediaQuery CreateQuery(HypermediaRepresentation representor = null)
        {
            var query = new HypermediaQuery();
            if (representor == null)
            {
                return query;
            }

            query.AddStep(new NavigateToRepresentorQueryStep(representor));
            return query;
        }

        public Task<HypermediaRepresentation> ExecuteQueryAsync(IHypermediaQuery query)
        {
            if (query == null) { throw new ArgumentNullException("query"); }

            return query.ExecuteAsync(TransitionRequestHandler);
        }
    }
}
