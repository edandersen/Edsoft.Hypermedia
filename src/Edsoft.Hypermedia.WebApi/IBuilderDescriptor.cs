using System;
using System.Web.Http.Controllers;

namespace Edsoft.Hypermedia.WebApi
{
    public interface IBuilderDescriptor
    {
        IRepresentationBuilder BuildForType(Type type, object value, HttpRequestContext context);
        bool SupportsType(Type type);
    }
}
