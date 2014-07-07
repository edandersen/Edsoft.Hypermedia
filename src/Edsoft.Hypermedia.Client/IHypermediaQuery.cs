using System.Collections.Generic;
using System.Threading.Tasks;
using Edsoft.Hypermedia.Client.QuerySteps;

namespace Edsoft.Hypermedia.Client
{
    public interface IHypermediaQuery
    {
        IList<IQueryStep> Steps { get; }
        void AddStep(IQueryStep step);
        Task<HypermediaRepresentation> ExecuteAsync(ITransitionRequestHandler requestHandler);
        IHypermediaQuery Clone();
    }
}