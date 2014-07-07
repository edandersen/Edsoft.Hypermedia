using System;
using System.Threading.Tasks;

namespace Edsoft.Hypermedia.Client.QuerySteps
{
    public class PostToTransitionQueryStep : NavigateToTransitionQueryStep
    {
        public object Data { get; private set; }

        public PostToTransitionQueryStep(string rel, object data) : base(rel)
        {
            this.Data = data;
        }

        public PostToTransitionQueryStep(Func<HypermediaTransition, bool> transitionSelectorFunc, object data)
            : base(transitionSelectorFunc)
        {
            this.Data = data;
        }

        public Task<HypermediaRepresentation> ExecuteAsync(HypermediaRepresentation currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            var transition = LocateTransition(currentRepresentor);

            return transitionRequestHandler.RequestTransitionAsync(transition, Data);
        }
    }
}
