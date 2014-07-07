using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using Edsoft.Hypermedia;
using Edsoft.Hypermedia.WebApi;
using Edsoft.Hypermedia.WebApi.Extensions;
using WebApiSample.Models;

namespace WebApiSample.BuilderDescriptors
{
    public class FriendModelEnumerableBuilderDescriptor : IBuilderDescriptor
    {
        public IRepresentationBuilder BuildForType(Type type, object value, HttpRequestContext context)
        {
            var builder = new RepresentationBuilder();

            var models = (IEnumerable<FriendModel>)value;

            builder.SetCollection(models, d => context.Url.Route("DefaultApi", new { controller = "friends", id = d.Id }));
            builder.SetSelfLinkToCurrentUrl(context);

            return builder;
        }

        public bool SupportsType(Type type)
        {
            return type == typeof (IEnumerable<FriendModel>);
        }
    }
}