using System.Threading.Tasks;

namespace Edsoft.Hypermedia.Client
{
    public interface ITransitionRequestHandler
    {
        void AddRequestFilter(ITransitionRequestFilter filter);
        Task<HypermediaRepresentation> RequestTransitionAsync(HypermediaTransition transition, object toSerializeToJson = null);
    }
}
