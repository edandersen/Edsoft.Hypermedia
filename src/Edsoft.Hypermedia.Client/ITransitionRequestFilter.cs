using System.Net.Http;

namespace Edsoft.Hypermedia.Client
{
    public interface ITransitionRequestFilter
    {
        void Execute(HttpRequestMessage httpRequestMessage);
    }
}
