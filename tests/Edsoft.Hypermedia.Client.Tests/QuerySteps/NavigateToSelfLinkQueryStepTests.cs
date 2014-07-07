using System;
using System.Threading.Tasks;
using Edsoft.Hypermedia.Client;
using Edsoft.Hypermedia.Client.QuerySteps;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Edsoft.Hypermedia.Tests.QuerySteps
{
    public class NavigateToSelfLinkQueryStepTests : TestWithFixture
    {
        [SetUp]
        public void Init()
        {
            Fixture = GetFixture();
        }

        [Test]
        public async Task ExecuteAsync_RequestsTransitionWithSelfLinkAsUrl()
        {
            var representor = Fixture.Create<HypermediaRepresentation>();
            var expected = Fixture.Create<HypermediaRepresentation>();

            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();
            requestor.Stub(r => r.RequestTransitionAsync(Arg<HypermediaTransition>.Matches(t => t.Uri == representor.SelfLink), Arg<object>.Is.Null)).Return(Task.FromResult(expected));

            var sut = new NavigateToSelfLinkQueryStep();

            var result = await sut.ExecuteAsync(representor, requestor);

            Assert.AreEqual(expected, result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_SetsNullRepresentor()
        {
            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();

            var sut = new NavigateToSelfLinkQueryStep();

            var result = await sut.ExecuteAsync(null, requestor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_SetsNullHandler()
        {
            var representor = Fixture.Create<HypermediaRepresentation>();

            var sut = new NavigateToSelfLinkQueryStep();

            var result = await sut.ExecuteAsync(representor, null);
        }
    }
}
