using System.Threading.Tasks;

namespace Edsoft.Hypermedia.Client.QuerySteps
{
    public interface IQueryStep
    {
        Task<HypermediaRepresentation> ExecuteAsync(HypermediaRepresentation currentRepresentor, ITransitionRequestHandler transitionRequestHandler);
    }
}
