﻿using System;
using System.Collections.Generic;
using System.Linq;
using Edsoft.Hypermedia.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Edsoft.Hypermedia.Tests.Serializers
{
    public class HaleSerializerTests : TestWithFixture
    {
        private HaleSerializer sut;
        private Func<IRepresentationBuilder> builderFactoryMethod;

        [SetUp]
        public void Init()
        {
            sut = new HaleSerializer();
            Fixture = GetFixture();
            builderFactoryMethod = () => MockRepository.GenerateMock<IRepresentationBuilder>();
        }

        private HypermediaRepresentation GetRepresentorWithTransitions(Func<HypermediaTransition> transitionFunc)
        {
            var result = Fixture.Create<HypermediaRepresentation>();
            result.Transitions.Clear();
            result.Transitions.AddMany(transitionFunc, new Random(DateTime.Now.Millisecond).Next(100));
            return result;
        }

        [Test]
        public void HasCorrectContentType()
        {
            Assert.AreEqual("application/vnd.hale+json", sut.ContentType);
        }

        [Test]
        public void Serialize_AddsSingleMethodAttributeForEachTransition()
        {
            var representor = GetRepresentorWithTransitions(() => new HypermediaTransition() { Rel = Fixture.Create<string>(), Methods = new []{ Fixture.Create<string>() } });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.Methods.Single(), result["_links"][transition.Rel]["method"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsMultipleMethodAttributesForEachTransition()
        {
            var representor = GetRepresentorWithTransitions(() => new HypermediaTransition() { Rel = Fixture.Create<string>(), Methods = Fixture.Create<string[]>() });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                var array = (JArray)result["_links"][transition.Rel]["method"];
                foreach (var method in transition.Methods)
                {
                    Assert.IsTrue(array.Any(a => a.Value<string>() == method));
                }
            }
        }

        [Test]
        public void Serialize_AddsSingleAcceptedMediaTypeAttributeForEachTransition()
        {
            var representor = GetRepresentorWithTransitions(() => new HypermediaTransition() { Rel = Fixture.Create<string>(), MediaTypesAccepted = new[] { Fixture.Create<string>() } });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.MediaTypesAccepted.Single(), result["_links"][transition.Rel]["enctype"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsMultipleAcceptedMediaTypeAttributesForEachTransition()
        {
            var representor = GetRepresentorWithTransitions(() => new HypermediaTransition() { Rel = Fixture.Create<string>(), MediaTypesAccepted = Fixture.Create<string[]>() });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                var array = (JArray)result["_links"][transition.Rel]["enctype"];
                foreach (var mediaType in transition.MediaTypesAccepted)
                {
                    Assert.IsTrue(array.Any(a => a.Value<string>() == mediaType));
                }
            }
        }

        [Test]
        public void Serialize_DoesNotAddAttributeForUndefinedRenderMethod()
        {
            var representor = GetRepresentorWithTransitions(() => new HypermediaTransition() { Rel = Fixture.Create<string>(), RenderMethod = TransitionRenderMethod.Undefined });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.IsNull(result["_links"][transition.Rel]["render"]);
            }
        }

        [Test]
        public void Serialize_AddsAttributeForEmbedRenderMethod()
        {
            var representor = GetRepresentorWithTransitions(() => new HypermediaTransition() { Rel = Fixture.Create<string>(), RenderMethod = TransitionRenderMethod.Embed });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual("embed", result["_links"][transition.Rel]["render"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsAttributeForResourceRenderMethod()
        {
            var representor = GetRepresentorWithTransitions(() => new HypermediaTransition() { Rel = Fixture.Create<string>(), RenderMethod = TransitionRenderMethod.Resource });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual("resource", result["_links"][transition.Rel]["render"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsTargetAttributeForEachTransition()
        {
            var representor = GetRepresentorWithTransitions(() => new HypermediaTransition() { Rel = Fixture.Create<string>(), Target = Fixture.Create<string>() });
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.Target, result["_links"][transition.Rel]["target"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsJsonTypeForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes) attribute.Value.DataType = null; // clear DataType
                return new HypermediaTransition() {Rel = Fixture.Create<string>(), Attributes = attributes};
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    Assert.AreEqual(attribute.Value.JsonType, result["_links"][transition.Rel]["data"][attribute.Key]["type"].Value<string>());
                }
            }
        }

        [Test]
        public void Serialize_AddsJsonTypeAndDataTypeForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    Assert.AreEqual(attribute.Value.JsonType + ":" + attribute.Value.DataType, result["_links"][transition.Rel]["data"][attribute.Key]["type"].Value<string>());
                }
            }
        }

        [Test]
        public void Serialize_AddsProfileUriTypeForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    Assert.AreEqual(attribute.Value.ProfileUri, result["_links"][transition.Rel]["data"][attribute.Key]["profile"].Value<string>());
                }
            }
        }

        [Test]
        public void Serialize_AddsAttributeWithHttpScopeForParameters()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var parameters = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Parameters = parameters };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var parameter in transition.Parameters)
                {
                    Assert.AreEqual(parameter.Value.ProfileUri, result["_links"][transition.Rel]["data"][parameter.Key]["profile"].Value<string>());
                    Assert.AreEqual("href", result["_links"][transition.Rel]["data"][parameter.Key]["scope"].Value<string>());
                }
            }
        }

        [Test]
        public void Serialize_AddsAttributesWithMatchingKeysWithEitherScopePreferingTransitionsWhenMatching()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                var parameters = new Dictionary<string, HypermediaTransitionAttribute>();
                foreach (var key in attributes.Keys)
                {
                    parameters[key] = new HypermediaTransitionAttribute();
                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Parameters = parameters, Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    Assert.AreEqual(attribute.Value.ProfileUri, result["_links"][transition.Rel]["data"][attribute.Key]["profile"].Value<string>());
                    Assert.AreEqual("either", result["_links"][transition.Rel]["data"][attribute.Key]["scope"].Value<string>());
                }
            }
        }

        private object GetRandomObject()
        {
            var fixtureFuncs = new Func<object>[]
            {
                () => Fixture.Create<string>(), 
                () => Fixture.Create<bool>(), 
                () => Fixture.Create<int>(), 
                () => Fixture.Create<Guid>()
            };

            var random = new Random(DateTime.Now.Millisecond);

            return fixtureFuncs[random.Next(fixtureFuncs.Length - 1)]();
        }

        [Test]
        public void Serialize_AddsValueForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes)
                {
                    attribute.Value.Value = GetRandomObject();
                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    var token = result["_links"][transition.Rel]["data"][attribute.Key]["value"];
                    Assert.AreEqual(attribute.Value.Value, token.ToObject(attribute.Value.Value.GetType()));
                }
            }
        }

        [Test]
        public void Serialize_AddsNestedAttributesAsData()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes)
                {
                    attribute.Value.Attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    foreach (var nestedAttribute in attribute.Value.Attributes)
                    {
                        var token = result["_links"][transition.Rel]["data"][attribute.Key]["data"][nestedAttribute.Key];
                        Assert.AreEqual(nestedAttribute.Value.ProfileUri, token["profile"].Value<string>());
                    }

                }
            }
        }

        [Test]
        public void Serialize_AddsNestedAttributesAsDataWithParameterHrefScope()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var parameters = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in parameters)
                {
                    attribute.Value.Parameters = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Parameters = parameters };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Parameters)
                {
                    foreach (var nestedAttribute in attribute.Value.Parameters)
                    {
                        var token = result["_links"][transition.Rel]["data"][attribute.Key]["data"][nestedAttribute.Key];
                        Assert.AreEqual(nestedAttribute.Value.ProfileUri, token["profile"].Value<string>());
                        Assert.AreEqual("href", token["scope"].Value<string>());
                    }
                }
            }
        }

        [Test]
        public void Serialize_AddsOptionsConstraintForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes)
                {
                    attribute.Value.Constraint = new HypermediaTransitionAttributeConstraint
                    {
                        Options = Fixture.Create<IList<string>>()
                    };

                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    var attr = result["_links"][transition.Rel]["data"][attribute.Key];
                    var options = attr["options"].Values<string>().ToList();

                    Assert.AreEqual(attribute.Value.Constraint.Options.Count, options.Count);
                    for (int i = 0; i < attribute.Value.Constraint.Options.Count; i++)
                    {
                        Assert.AreEqual(attribute.Value.Constraint.Options[i], options[i]);
                    }
                }
            }
        }

        [Test]
        public void Serialize_AddsInConstraintForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes)
                {
                    attribute.Value.Constraint = new HypermediaTransitionAttributeConstraint
                    {
                        IsIn = Fixture.Create<bool?>()
                    };

                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    var token = result["_links"][transition.Rel]["data"][attribute.Key]["in"];
                    Assert.AreEqual(attribute.Value.Constraint.IsIn, token.Value<bool?>());
                }
            }
        }

        [Test]
        public void Serialize_AddsMinConstraintForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes)
                {
                    attribute.Value.Constraint = new HypermediaTransitionAttributeConstraint
                    {
                        Min = Fixture.Create<int?>()
                    };

                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    var token = result["_links"][transition.Rel]["data"][attribute.Key]["min"];
                    Assert.AreEqual(attribute.Value.Constraint.Min, token.Value<int?>());
                }
            }
        }

        [Test]
        public void Serialize_AddsMinLengthConstraintForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes)
                {
                    attribute.Value.Constraint = new HypermediaTransitionAttributeConstraint
                    {
                        MinLength = Fixture.Create<int?>()
                    };

                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    var token = result["_links"][transition.Rel]["data"][attribute.Key]["minlength"];
                    Assert.AreEqual(attribute.Value.Constraint.MinLength, token.Value<int?>());
                }
            }
        }

        [Test]
        public void Serialize_AddsMaxConstraintForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes)
                {
                    attribute.Value.Constraint = new HypermediaTransitionAttributeConstraint
                    {
                        Max = Fixture.Create<int?>()
                    };

                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    var token = result["_links"][transition.Rel]["data"][attribute.Key]["max"];
                    Assert.AreEqual(attribute.Value.Constraint.Max, token.Value<int?>());
                }
            }
        }

        [Test]
        public void Serialize_AddsMaxLengthConstraintForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes)
                {
                    attribute.Value.Constraint = new HypermediaTransitionAttributeConstraint
                    {
                        MaxLength = Fixture.Create<int?>()
                    };

                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    var token = result["_links"][transition.Rel]["data"][attribute.Key]["maxlength"];
                    Assert.AreEqual(attribute.Value.Constraint.MaxLength, token.Value<int?>());
                }
            }
        }

        [Test]
        public void Serialize_AddsPatternConstraintForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes)
                {
                    attribute.Value.Constraint = new HypermediaTransitionAttributeConstraint
                    {
                        Pattern = Fixture.Create<string>()
                    };

                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    var token = result["_links"][transition.Rel]["data"][attribute.Key]["pattern"];
                    Assert.AreEqual(attribute.Value.Constraint.Pattern, token.Value<string>());
                }
            }
        }

        [Test]
        public void Serialize_AddMultiConstraintForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes)
                {
                    attribute.Value.Constraint = new HypermediaTransitionAttributeConstraint
                    {
                        IsMulti = Fixture.Create<bool?>()
                    };

                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    var token = result["_links"][transition.Rel]["data"][attribute.Key]["multi"];
                    Assert.AreEqual(attribute.Value.Constraint.IsMulti, token.Value<bool?>());
                }
            }
        }

        [Test]
        public void Serialize_AddRequiredConstraintForEachTransition()
        {
            Func<HypermediaTransition> transitionFunc = () =>
            {
                var attributes = Fixture.Create<IDictionary<string, HypermediaTransitionAttribute>>();
                foreach (var attribute in attributes)
                {
                    attribute.Value.Constraint = new HypermediaTransitionAttributeConstraint
                    {
                        IsRequired = Fixture.Create<bool?>()
                    };

                }
                return new HypermediaTransition() { Rel = Fixture.Create<string>(), Attributes = attributes };
            };

            var representor = GetRepresentorWithTransitions(transitionFunc);
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                foreach (var attribute in transition.Attributes)
                {
                    var token = result["_links"][transition.Rel]["data"][attribute.Key]["required"];
                    Assert.AreEqual(attribute.Value.Constraint.IsRequired, token.Value<bool?>());
                }
            }
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingSingleMethod()
        {
            var href = Fixture.Create<string>();
            var method = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""method"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, method);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Methods.Single() == method)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingMultipleMethods()
        {
            var href = Fixture.Create<string>();
            var methods = Fixture.Create<string[]>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""method"" : {2} }} 
                }}
            }}";

            json = String.Format(json, rel, href, JsonConvert.SerializeObject(methods));

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && !t.Methods.Except(methods).Any())));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingSingleMediaType()
        {
            var href = Fixture.Create<string>();
            var mediaType = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""enctype"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, mediaType);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.MediaTypesAccepted.Single() == mediaType)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingMultipleMediaTypes()
        {
            var href = Fixture.Create<string>();
            var mediaTypes = Fixture.Create<string[]>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""enctype"" : {2} }} 
                }}
            }}";

            json = String.Format(json, rel, href, JsonConvert.SerializeObject(mediaTypes));

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && !t.MediaTypesAccepted.Except(mediaTypes).Any())));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsRenderMethodToEmbed()
        {
            var href = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""render"" : ""embed"" }} 
                }}
            }}";

            json = String.Format(json, rel, href);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.RenderMethod == TransitionRenderMethod.Embed)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsRenderMethodToResource()
        {
            var href = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""render"" : ""resource"" }} 
                }}
            }}";

            json = String.Format(json, rel, href);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.RenderMethod == TransitionRenderMethod.Resource)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsToUndefinedForInvalidRenderMethod()
        {
            var href = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var render = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""render"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, render);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.RenderMethod == TransitionRenderMethod.Undefined)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingTarget()
        {
            var href = Fixture.Create<string>();
            var target = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""target"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, target);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Target == target)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingAttributeWithSingleType()
        {
            var href = Fixture.Create<string>();
            var attributeName = Fixture.Create<string>();
            var type = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""data"" : {{ ""{2}"" : {{ ""type"" : ""{3}"" }}}}}} 
                }}
            }}";

            json = String.Format(json, rel, href, attributeName, type);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Attributes[attributeName].JsonType == type)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingAttributeWitJsonTypeAndDataType()
        {
            var href = Fixture.Create<string>();
            var attributeName = Fixture.Create<string>();
            var type = Fixture.Create<string>();
            var dataType = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""data"" : {{ ""{2}"" : {{ ""type"" : ""{3}:{4}"" }}}}}} 
                }}
            }}";

            json = String.Format(json, rel, href, attributeName, type, dataType);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Attributes[attributeName].JsonType == type && t.Attributes[attributeName].DataType == dataType)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingProfileUri()
        {
            var href = Fixture.Create<string>();
            var attributeName = Fixture.Create<string>();
            var profileUri = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""data"" : {{ ""{2}"" : {{ ""profile"" : ""{3}"" }}}}}} 
                }}
            }}";

            json = String.Format(json, rel, href, attributeName, profileUri);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Attributes[attributeName].ProfileUri == profileUri)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionAttributesToParametersWhenScopeIsHref()
        {
            var href = Fixture.Create<string>();
            var attributeName = Fixture.Create<string>();
            var profileUri = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""data"" : {{ ""{2}"" : {{ ""profile"" : ""{3}"", ""scope"" : ""href"" }}}}}} 
                }}
            }}";

            json = String.Format(json, rel, href, attributeName, profileUri);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            // nothing should be in attributes
            builder.AssertWasNotCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Attributes.Any())));

            // should be added to parameters
            builder.AssertWasCalled(b => b.AddTransition(Arg<HypermediaTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Parameters[attributeName].ProfileUri == profileUri)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingValue()
        {
            var href = Fixture.Create<string>();
            var attributeName = Fixture.Create<string>();
            var value = Fixture.Create<int>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""{0}"": {{ ""href"" : ""{1}"", ""data"" : {{ ""{2}"" : {{ ""value"" : {3} }}}}}} 
                }}
            }}";

            json = String.Format(json, rel, href, attributeName, value);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());

            Assert.AreEqual(rel, transition.Rel, rel);
            Assert.AreEqual(href, transition.Uri);
            Assert.AreEqual(value, transition.Attributes[attributeName].Value);
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializesNestedDataObjects()
        {
            var href = Fixture.Create<string>();
            var attributeName = Fixture.Create<string>();
            var nestedAttributeName = Fixture.Create<string>();
            var profile = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""{0}"": {{ ""href"" : ""{1}"", ""data"" : {{ ""{2}"" : {{ 
                    ""data"" : {{
                                ""{3}"" :
                                    {{
                                        ""profile"" : ""{4}""
                                    }}
                                }}
                    }}}}}} 
                }}
            }}";

            json = String.Format(json, rel, href, attributeName, nestedAttributeName, profile);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());

            Assert.AreEqual(rel, transition.Rel, rel);
            Assert.AreEqual(href, transition.Uri);
            Assert.AreEqual(profile, transition.Attributes[attributeName].Attributes[nestedAttributeName].ProfileUri);
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializesNestedDataObjectsWithHrefScopeToParameters()
        {
            var href = Fixture.Create<string>();
            var attributeName = Fixture.Create<string>();
            var nestedAttributeName = Fixture.Create<string>();
            var profile = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""{0}"": {{ ""href"" : ""{1}"", ""data"" : {{ ""{2}"" : {{ 
                    ""data"" : {{
                                ""{3}"" :
                                    {{
                                        ""profile"" : ""{4}"",
                                        ""scope"" : ""href""
                                    }}
                                }}
                    }}}}}} 
                }}
            }}";

            json = String.Format(json, rel, href, attributeName, nestedAttributeName, profile);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());

            Assert.AreEqual(rel, transition.Rel, rel);
            Assert.AreEqual(href, transition.Uri);
            Assert.AreEqual(profile, transition.Attributes[attributeName].Parameters[nestedAttributeName].ProfileUri);
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializesNestedDataObjectsWithEitherScopeToParametersAndAttributes()
        {
            var href = Fixture.Create<string>();
            var attributeName = Fixture.Create<string>();
            var nestedAttributeName = Fixture.Create<string>();
            var profile = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""{0}"": {{ ""href"" : ""{1}"", ""data"" : {{ ""{2}"" : {{ 
                    ""data"" : {{
                                ""{3}"" :
                                    {{
                                        ""profile"" : ""{4}"",
                                        ""scope"" : ""either""
                                    }}
                                }}
                    }}}}}} 
                }}
            }}";

            json = String.Format(json, rel, href, attributeName, nestedAttributeName, profile);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());

            Assert.AreEqual(rel, transition.Rel, rel);
            Assert.AreEqual(href, transition.Uri);
            Assert.AreEqual(profile, transition.Attributes[attributeName].Parameters[nestedAttributeName].ProfileUri);
            Assert.AreEqual(profile, transition.Attributes[attributeName].Attributes[nestedAttributeName].ProfileUri);
      
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializeOptionsConstraint()
        {
            var option0 = Fixture.Create<string>();
            var option1 = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""edit"": {{ 
                        ""href"" : ""../{{?user_id}}"", 
                        ""data"" : {{
                            ""send_info"" : {{
                                ""options"" : [ ""{0}"", ""{1}"" ]
                            }}
                        }}
                    }} 
                }}
            }}";

            json = String.Format(json, option0, option1);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);
            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());
            var constraints = transition.Attributes.First().Value.Constraint;

            Assert.AreEqual(2, constraints.Options.Count);
            Assert.AreEqual(option0, constraints.Options[0]);
            Assert.AreEqual(option1, constraints.Options[1]);
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializeEmptyOptionsConstraint()
        {
            var json = @"
            {
                ""_links"": {
                    ""edit"": { 
                        ""href"" : ""../{?user_id}"", 
                        ""data"" : {
                            ""send_info"" : {
                                ""options"" : [],
                            }
                        }
                    }
                }
            }";

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());
            var constraints = transition.Attributes.First().Value.Constraint;

            Assert.AreEqual(0, constraints.Options.Count);
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializeInConstraint()
        {
            var isIn = Fixture.Create<bool>().ToString().ToLower();
            var json = @"
            {{
                ""_links"": {{
                    ""edit"": {{ 
                        ""href"" : ""../{{?user_id}}"", 
                        ""data"" : {{
                            ""send_info"" : {{
                                ""in"" : {0}
                            }}
                        }}
                    }} 
                }}
            }}";

            json = String.Format(json, isIn);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);
            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());
            var constraints = transition.Attributes.First().Value.Constraint;

            Assert.AreEqual(isIn, constraints.IsIn.ToString().ToLower());
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializeMinConstraint()
        {
            var min = Fixture.Create<int>();
            var json = @"
            {{
                ""_links"": {{
                    ""edit"": {{ 
                        ""href"" : ""../{{?user_id}}"", 
                        ""data"" : {{
                            ""send_info"" : {{
                                ""min"" : {0}
                            }}
                        }}
                    }} 
                }}
            }}";

            json = String.Format(json, min);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);
            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());
            var constraints = transition.Attributes.First().Value.Constraint;

            Assert.AreEqual(min, constraints.Min);
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializeMinLengthConstraint()
        {
            var minLength = Fixture.Create<int>();
            var json = @"
            {{
                ""_links"": {{
                    ""edit"": {{ 
                        ""href"" : ""../{{?user_id}}"", 
                        ""data"" : {{
                            ""send_info"" : {{
                                ""minlength"" : {0}
                            }}
                        }}
                    }} 
                }}
            }}";

            json = String.Format(json, minLength);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());
            var constraints = transition.Attributes.First().Value.Constraint;

            Assert.AreEqual(minLength, constraints.MinLength);
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializeMaxConstraint()
        {
            var max = Fixture.Create<int>();
            var json = @"
            {{
                ""_links"": {{
                    ""edit"": {{ 
                        ""href"" : ""../{{?user_id}}"", 
                        ""data"" : {{
                            ""send_info"" : {{
                                ""max"" : {0}
                            }}
                        }}
                    }} 
                }}
            }}";

            json = String.Format(json, max);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);
            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());
            var constraints = transition.Attributes.First().Value.Constraint;

            Assert.AreEqual(max, constraints.Max);
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializeMaxLengthConstraint()
        {
            var maxLength = Fixture.Create<int>();
            var json = @"
            {{
                ""_links"": {{
                    ""edit"": {{ 
                        ""href"" : ""../{{?user_id}}"", 
                        ""data"" : {{
                            ""send_info"" : {{
                                ""maxlength"" : {0}
                            }}
                        }}
                    }} 
                }}
            }}";

            json = String.Format(json, maxLength);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());
            var constraints = transition.Attributes.First().Value.Constraint;

            Assert.AreEqual(maxLength, constraints.MaxLength);
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializePatternConstraint()
        {
            var pattern = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""edit"": {{ 
                        ""href"" : ""../{{?user_id}}"", 
                        ""data"" : {{
                            ""send_info"" : {{
                                ""pattern"" : ""{0}""
                            }}
                        }}
                    }} 
                }}
            }}";

            json = String.Format(json, pattern);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);
            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());
            var constraints = transition.Attributes.First().Value.Constraint;

            Assert.AreEqual(pattern, constraints.Pattern);
        }

        public void DeserializeToNewBuilder_DeserializeMultiConstraint()
        {
            var isMulti = Fixture.Create<bool>().ToString().ToLower();
            var json = @"
            {{
                ""_links"": {{
                    ""edit"": {{ 
                        ""href"" : ""../{{?user_id}}"", 
                        ""data"" : {{
                            ""send_info"" : {{
                                ""multi"" : {0},
                            }}
                        }}
                    }} 
                }}
            }}";

            json = String.Format(json, isMulti);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);
            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());
            var constraints = transition.Attributes.First().Value.Constraint;

            Assert.AreEqual(isMulti, constraints.IsMulti.ToString().ToLower());
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializeRequiredConstraint()
        {
            var isRequired = Fixture.Create<bool>().ToString().ToLower();
            var json = @"
            {{
                ""_links"": {{
                    ""edit"": {{ 
                        ""href"" : ""../{{?user_id}}"", 
                        ""data"" : {{
                            ""send_info"" : {{
                                ""required"" : {0}
                            }}
                        }}
                    }} 
                }}
            }}";

            json = String.Format(json, isRequired);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            var transition = (HypermediaTransition)(builder.GetArgumentsForCallsMadeOn(b => b.AddTransition(null))[0].First());
            var constraints = transition.Attributes.First().Value.Constraint;

            Assert.AreEqual(isRequired, constraints.IsRequired.ToString().ToLower());
        }

    }
}
