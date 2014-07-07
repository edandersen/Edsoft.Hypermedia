using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edsoft.Hypermedia.Client.QuerySteps;

namespace Edsoft.Hypermedia.Client
{
    public class HypermediaQuery : IHypermediaQuery
    {
        public IList<IQueryStep> Steps { get; private set; }

        public HypermediaQuery() : this(new List<IQueryStep>())
        {
        }

        private HypermediaQuery(IEnumerable<IQueryStep> steps)
        {
            Steps = steps.ToList();
        }

        public void AddStep(IQueryStep step)
        {
            if (step == null) { throw new ArgumentNullException("step"); }

            Steps.Add(step);
        }

        public async Task<HypermediaRepresentation> ExecuteAsync(ITransitionRequestHandler requestHandler)
        {
            if (requestHandler == null) { throw new ArgumentNullException("requestHandler"); }

            HypermediaRepresentation representor = null;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var step in Steps)
            {
                representor = await step.ExecuteAsync(representor, requestHandler);
            }

            return representor;
        }

        public IHypermediaQuery Clone()
        {
            return new HypermediaQuery(this.Steps);
        }
    }
}
