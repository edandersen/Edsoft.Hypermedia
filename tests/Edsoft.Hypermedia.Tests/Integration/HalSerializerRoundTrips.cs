using Edsoft.Hypermedia.Serializers;
using NUnit.Framework;

namespace Edsoft.Hypermedia.Tests.Integration
{
    public class HalSerializerRoundTrips : RoundTripTests
    {
        private HalSerializer serializer;

        [SetUp]
        public void Init()
        {
            serializer = new HalSerializer();
            Fixture = GetFixture();
        }

        [Test]
        public void SelfLinkOnly_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\SelfLinkOnly", serializer);
        }

        [Test]
        public void MultipleLinksSameRelation_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\MultipleLinksSameRelation", serializer);
        }

        [Test]
        public void SimpleLinksAndAttributes_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\SimpleLinksAndAttributes", serializer);
        }

        // From "Resources" here: https://phlyrestfully.readthedocs.org/en/latest/halprimer.html
        [Test]
        public void ComplexEmbeddedResources_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\ComplexEmbeddedResources", serializer);
        }

        [Test]
        public void WormholeSample_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\WormholeSample", serializer);
        }

        [Test]
        public void HalAllLinkObjectProperties_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\AllLinkObjectProperties", serializer);
        }

    }
}
