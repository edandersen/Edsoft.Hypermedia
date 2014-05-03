﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors
{
    public class RepresentorBuilder : IRepresentorBuilder
    {
        private readonly CrichtonRepresentor representor;

        public RepresentorBuilder()
        {
            representor = new CrichtonRepresentor();
        }

        public CrichtonRepresentor ToRepresentor()
        {
            return representor;
        }

        public void SetSelfLink(string self)
        {
            representor.SelfLink = self;
        }

        public void SetAttributes(JObject attributes)
        {
            representor.Attributes = attributes;
        }

        public void SetAttributesFromObject(object data)
        {
            representor.Attributes = JObject.FromObject(data);
        }

        public void AddTransition(CrichtonTransition transition)
        {
            representor.Transitions.Add(transition);
        }

        public void AddTransition(string rel, string uri = null, string title = null, string type = null, bool uriIsTemplated = false, 
            string depreciationUri = null, string name = null, string profileUri = null, string languageTag = null)
        {
            representor.Transitions.Add(new CrichtonTransition { Rel = rel, Uri = uri, Title = title, Type = type, 
                UriIsTemplated = uriIsTemplated, DepreciationUri = depreciationUri, Name = name, ProfileUri = profileUri, LanguageTag = languageTag});
        }

        public void AddEmbeddedResource(string key, CrichtonRepresentor resource)
        {
            if (!representor.EmbeddedResources.ContainsKey(key))
                representor.EmbeddedResources[key] = new List<CrichtonRepresentor>();

            representor.EmbeddedResources[key].Add(resource);
        }

        public void SetCollection(IEnumerable<CrichtonRepresentor> representors)
        {
            foreach(var representorInCollection in representors)
                representor.Collection.Add(representorInCollection);
        }

        public void SetCollection<T>(IEnumerable<T> collection, Func<T, string> selfLinkFunc)
        {
            foreach (var item in collection)
            {
                var newRepresentor = new CrichtonRepresentor {SelfLink = selfLinkFunc(item)};
                newRepresentor.SetAttributesFromObject(item);

                representor.Collection.Add(newRepresentor);
            }
        }
    }
}
