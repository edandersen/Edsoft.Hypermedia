using System;
using System.Linq;
using System.Threading.Tasks;

namespace Edsoft.Hypermedia.Client.QuerySteps
{
    public class NavigateToTransitionQueryStep : IQueryStep
    {
        private readonly Func<HypermediaTransition, bool> selectionFunc;

        public NavigateToTransitionQueryStep(string rel)
        {
            if (rel == null) { throw new ArgumentNullException("rel"); }

            selectionFunc = transition => transition.Rel == rel;
        }

        public NavigateToTransitionQueryStep(Func<HypermediaTransition, bool> transitionSelectionFunc)
        {
            if (transitionSelectionFunc == null) { throw new ArgumentNullException("transitionSelectionFunc"); }

            selectionFunc = transitionSelectionFunc;
        }

        public Task<HypermediaRepresentation> ExecuteAsync(HypermediaRepresentation currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            var transition = LocateTransition(currentRepresentor);

            return transitionRequestHandler.RequestTransitionAsync(transition);
        }

        public HypermediaTransition LocateTransition(HypermediaRepresentation currentRepresentor)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }

            var transition = currentRepresentor.Transitions.Single(selectionFunc);
            return transition;
        }
    }
}
