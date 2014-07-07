using System.Web.Http.Controllers;

namespace Edsoft.Hypermedia.WebApi.Extensions
{
    public static class RepresentorBuilderWebApiExtensions
    {
        public static void SetSelfLinkToCurrentUrl(this IRepresentationBuilder builder, HttpRequestContext requestContext)
        {
            builder.SetSelfLink(requestContext.Url.Request.RequestUri.PathAndQuery);
        }

        public static void AddTranstionToRoute(this IRepresentationBuilder builder, HttpRequestContext requestContext, string rel, string routeName, object routeValues)
        {
            builder.AddTransition(rel, requestContext.Url.Route(routeName, routeValues));
        }
    }
}
