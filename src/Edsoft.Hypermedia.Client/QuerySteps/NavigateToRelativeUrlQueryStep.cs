using System;
using System.Threading.Tasks;

namespace Edsoft.Hypermedia.Client.QuerySteps
{
    public class NavigateToRelativeUrlQueryStep : IQueryStep
    {
        public string Url { get; private set; }

        public NavigateToRelativeUrlQueryStep(string url)
        {
            if (url == null) { throw new ArgumentNullException("url"); }

            this.Url = url;
        }

        public Task<HypermediaRepresentation> ExecuteAsync(HypermediaRepresentation currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            var transition = new HypermediaTransition() {Uri = Url};
            return transitionRequestHandler.RequestTransitionAsync(transition);
        }
    }
}
