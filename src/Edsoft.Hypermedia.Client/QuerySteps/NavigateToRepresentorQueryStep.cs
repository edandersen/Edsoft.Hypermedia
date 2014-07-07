using System;
using System.Threading.Tasks;

namespace Edsoft.Hypermedia.Client.QuerySteps
{
    public class NavigateToRepresentorQueryStep : IQueryStep
    {
        public HypermediaRepresentation Representor { get; private set; }

        public NavigateToRepresentorQueryStep(HypermediaRepresentation representor)
        {
            if (representor == null) { throw new ArgumentNullException("representor"); }

            this.Representor = representor;
        }

        public async Task<HypermediaRepresentation> ExecuteAsync(HypermediaRepresentation currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            return Representor;
        }
    }
}
