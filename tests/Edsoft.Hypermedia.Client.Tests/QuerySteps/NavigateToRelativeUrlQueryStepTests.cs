using System;
using System.Threading.Tasks;
using Edsoft.Hypermedia.Client;
using Edsoft.Hypermedia.Client.QuerySteps;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Edsoft.Hypermedia.Tests.QuerySteps
{
    public class NavigateToRelativeUrlQueryStepTests : TestWithFixture
    {
        [SetUp]
        public void Init()
        {
            Fixture = GetFixture();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CTOR_SetsNullUrl()
        {
            var sut = new NavigateToRelativeUrlQueryStep(null);
        }

        [Test]
        public async Task ExecuteAsync_RequestsTransitionWithRelativeUrlAsUrl()
        {
            var representor = Fixture.Create<HypermediaRepresentation>();
            var expected = Fixture.Create<HypermediaRepresentation>();
            var url = Fixture.Create<string>();

            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();
            requestor.Stub(r => r.RequestTransitionAsync(Arg<HypermediaTransition>.Matches(t => t.Uri == url), Arg<object>.Is.Null)).Return(Task.FromResult(expected));

            var sut = new NavigateToRelativeUrlQueryStep(url);

            var result = await sut.ExecuteAsync(representor, requestor);

            Assert.AreEqual(expected, result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_SetsNullRepresentor()
        {
            var url = Fixture.Create<string>();
            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();

            var sut = new NavigateToRelativeUrlQueryStep(url);

            var result = await sut.ExecuteAsync(null, requestor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_SetsNullHandler()
        {
            var url = Fixture.Create<string>();
            var representor = Fixture.Create<HypermediaRepresentation>();

            var sut = new NavigateToRelativeUrlQueryStep(url);

            var result = await sut.ExecuteAsync(representor, null);
        }
    }
}
