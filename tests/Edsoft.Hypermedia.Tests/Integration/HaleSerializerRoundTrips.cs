using Edsoft.Hypermedia.Serializers;
using NUnit.Framework;

namespace Edsoft.Hypermedia.Tests.Integration
{
    public class HaleSerializerRoundTrips : RoundTripTests
    {
        private HaleSerializer serializer;

        [SetUp]
        public void Init()
        {
            serializer = new HaleSerializer();
            Fixture = GetFixture();
        }

        [Test]
        public void AllLinkObjectPropertiesExceptData_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hale\\AllLinkObjectPropertiesExceptData", serializer);
        }

        [Test]
        public void LinkObjectDataAttributes_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hale\\LinkObjectDataAttributes", serializer);
        }
    
    }
}
