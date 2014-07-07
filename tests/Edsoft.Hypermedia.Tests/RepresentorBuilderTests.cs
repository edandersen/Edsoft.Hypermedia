using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Edsoft.Hypermedia.Tests
{
    public class RepresentorBuilderTests : TestWithFixture
    {
        private RepresentationBuilder sut;

        [SetUp]
        public void Init()
        {
            sut = new RepresentationBuilder();
            Fixture = GetFixture();
        }

        [Test]
        public void ToRepresentor_ReturnsARepresentor()
        {
            var result = sut.ToRepresention();

            Assert.IsInstanceOf<HypermediaRepresentation>(result);
        }

        [Test]
        public void SetSelfLink_SetsSelfLink()
        {
            var self = Fixture.Create<string>();

            sut.SetSelfLink(self);
            var result = sut.ToRepresention();

            Assert.AreEqual(self, result.SelfLink);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetSelfLink_SetsSelfLinkWithNull()
        {
            sut.SetSelfLink(null);
        }

        [Test]
        public void SetAttributes_SetsRepresentorAttributes()
        {
            var attributes = new JObject();
            sut.SetAttributes(attributes);
            var result = sut.ToRepresention();

            Assert.AreEqual(attributes, result.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetAttributes_SetsRepresentorAttributesWithNull()
        {
            sut.SetAttributes(null);
        }

        [Test]
        public void SetAttributesFromObject_SetsAttributes()
        {
            var example = Fixture.Create<ExampleDataObject>();
            var expectedJObject = JObject.FromObject(example);

            sut.SetAttributesFromObject(example);
            var result = sut.ToRepresention();

            foreach (var property in expectedJObject.Properties())
            {
                Assert.AreEqual(expectedJObject[property.Name], result.Attributes[property.Name]);
            }

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetAttributesFromObject_SetsDataWithNull()
        {
            sut.SetAttributesFromObject(null);
        }

        [Test]
        public void AddTransition_AddsTransitionObjectOnce()
        {
            var transition = Fixture.Create<HypermediaTransition>();
            
            sut.AddTransition(transition);
            var result = sut.ToRepresention();

            Assert.IsNotNull(result.Transitions.SingleOrDefault(t => t == transition));

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddTransition_AddsTransitionWithNull()
        {
            sut.AddTransition(null);
        }

        [Test]
        public void AddTransition_CorrectlyAddsSimpleTransition()
        {
            var rel = Fixture.Create<string>();
            var uri = Fixture.Create<string>();

            sut.AddTransition(rel, uri);
            var result = sut.ToRepresention();

            result.Transitions.Should().ContainSingle(t => t.Rel == rel && t.Uri == uri);

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddTransition_AddsRelWithNull()
        {
            var uri = Fixture.Create<string>();

            sut.AddTransition(null, uri);
        }

        [Test]
        public void AddTransition_CorrectlyAddsSimpleTransitionWithTitle()
        {
            var rel = Fixture.Create<string>();
            var uri = Fixture.Create<string>();
            var title = Fixture.Create<string>();

            sut.AddTransition(rel, uri, title);
            var result = sut.ToRepresention();

            result.Transitions.Should().ContainSingle(t => t.Rel == rel && t.Uri == uri && t.Title == title);

        }

        [Test]
        public void AddTransition_CorrectlyAddsSimpleTransitionWithTitleAndType()
        {
            var rel = Fixture.Create<string>();
            var uri = Fixture.Create<string>();
            var title = Fixture.Create<string>();
            var type = Fixture.Create<string>();

            sut.AddTransition(rel, uri, title, type);
            var result = sut.ToRepresention();

            result.Transitions.Should().ContainSingle(t => t.Rel == rel && t.Uri == uri && t.Title == title && t.Type == type);

        }

        [Test]
        public void AddTransition_CorrectlyAddsSimpleTransitionWithIsTemplatedTrue()
        {
            var rel = Fixture.Create<string>();
            var isTemplated = Fixture.Create<bool>();

            sut.AddTransition(rel, uriIsTemplated: isTemplated);
            var result = sut.ToRepresention();

            result.Transitions.Should().ContainSingle(t => t.Rel == rel && t.UriIsTemplated == isTemplated);
        }

        [Test]
        public void AddTransition_CorrectlyAddsSimpleTransitionWithDepreciationLink()
        {
            var rel = Fixture.Create<string>();
            var depreciationUri = Fixture.Create<string>();

            sut.AddTransition(rel, depreciationUri: depreciationUri);
            var result = sut.ToRepresention();

            result.Transitions.Should().ContainSingle(t => t.Rel == rel && t.DepreciationUri == depreciationUri);
        }

        [Test]
        public void AddTransition_CorrectlyAddsSimpleTransitionWithName()
        {
            var rel = Fixture.Create<string>();
            var name = Fixture.Create<string>();

            sut.AddTransition(rel, name: name);
            var result = sut.ToRepresention();

            result.Transitions.Should().ContainSingle(t => t.Rel == rel && t.Name == name);
        }

        [Test]
        public void AddTransition_CorrectlyAddsSimpleTransitionWithProfileUri()
        {
            var rel = Fixture.Create<string>();
            var profileUri = Fixture.Create<string>();

            sut.AddTransition(rel, profileUri: profileUri);
            var result = sut.ToRepresention();

            result.Transitions.Should().ContainSingle(t => t.Rel == rel && t.ProfileUri == profileUri);
        }

        [Test]
        public void AddTransition_CorrectlyAddsSimpleTransitionWithLanguageTag()
        {
            var rel = Fixture.Create<string>();
            var languageTag = Fixture.Create<string>();

            sut.AddTransition(rel, languageTag: languageTag);
            var result = sut.ToRepresention();

            result.Transitions.Should().ContainSingle(t => t.Rel == rel && t.LanguageTag == languageTag);
        }

        [Test]
        public void AddEmbeddedResource_AddsResourceWithCorrectKey()
        {
            var key = Fixture.Create<string>();
            var resource = Fixture.Create<HypermediaRepresentation>();

            sut.AddEmbeddedResource(key, resource);
            var result = sut.ToRepresention();

            result.EmbeddedResources[key].Should().ContainSingle(t => t == resource);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEmbeddedResource_AddsKeyWithNull()
        {
            var resource = Fixture.Create<HypermediaRepresentation>();

            sut.AddEmbeddedResource(null, resource);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEmbeddedResource_AddsEmbeddedWithNull()
        {
            var key = Fixture.Create<string>();

            sut.AddEmbeddedResource(key, null);
        }

        [Test]
        public void SetCollection_SetsCollectionDataWithSelfLinks()
        {
            var examples = Fixture.Create<IList<ExampleDataObject>>();
            Func<ExampleDataObject, string> selfLinkFunc = e => "self-link-" + e.Id;

            sut.SetCollection(examples, selfLinkFunc);

            var result = sut.ToRepresention();

            foreach (var example in examples)
            {
                var exampleDataObject = example; // prevent different version of compiler warning
                result.Collection.Should().ContainSingle(c => c.SelfLink == selfLinkFunc(exampleDataObject));
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCollection_SetsCollectionWithNull()
        {
            Func<ExampleDataObject, string> selfLinkFunc = e => "self-link-" + e.Id;

            sut.SetCollection(null, selfLinkFunc);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCollection_SetsSelfLinkFuncWithNull()
        {
            var examples = Fixture.Create<IList<ExampleDataObject>>();

            sut.SetCollection(examples, null);
        }

        [Test]
        public void SetCollection_SetsRepresentors()
        {
            var representors = Fixture.CreateMany<HypermediaRepresentation>().ToList();

            sut.SetCollection(representors);

            var result = sut.ToRepresention();

            CollectionAssert.AreEquivalent(representors, result.Collection);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCollection_SetsRepresentorsWithNull()
        {
            sut.SetCollection(null);
        }
    }
}
