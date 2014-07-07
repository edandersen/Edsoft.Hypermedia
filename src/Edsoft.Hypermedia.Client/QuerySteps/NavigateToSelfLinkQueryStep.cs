using System;
using System.Threading.Tasks;

namespace Edsoft.Hypermedia.Client.QuerySteps
{
    public class NavigateToSelfLinkQueryStep : IQueryStep
    {
        public Task<HypermediaRepresentation> ExecuteAsync(HypermediaRepresentation currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            var selfTransition = new HypermediaTransition() {Uri = currentRepresentor.SelfLink};

            return transitionRequestHandler.RequestTransitionAsync(selfTransition);
        }
    }
}
