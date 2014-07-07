using System;
using System.Linq;
using System.Threading.Tasks;
using Edsoft.Hypermedia.Client;
using Edsoft.Hypermedia.Client.QuerySteps;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Edsoft.Hypermedia.Tests.QuerySteps
{
    public class NavigateToTransitionQueryStepTests : TestWithFixture
    {
        [SetUp]
        public void Init()
        {
            Fixture = GetFixture();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CTOR_SetsNullRel()
        {
            var sut = new NavigateToTransitionQueryStep((string)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CTOR_SetsNullFunc()
        {
            var sut = new NavigateToTransitionQueryStep((Func<HypermediaTransition, bool>)null);
        }

        [Test]
        public async Task ExecuteAsync_GetsCorrectTransitionAndRequestsFromTransitionRequestor()
        {
            var representor = Fixture.Create<HypermediaRepresentation>();
            var expected = Fixture.Create<HypermediaRepresentation>();
            var transition = representor.Transitions.First();
            var rel = transition.Rel;

            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();
            requestor.Stub(r => r.RequestTransitionAsync(transition)).Return(Task.FromResult(expected));

            var sut = new NavigateToTransitionQueryStep(rel);

            var result = await sut.ExecuteAsync(representor, requestor);

            Assert.AreEqual(expected, result);

        }

        [Test]
        public async Task ExecuteAsync_GetsCorrectTransitionAndRequestsFromTransitionRequestorUsingFunction()
        {
            var representor = Fixture.Create<HypermediaRepresentation>();
            var expected = Fixture.Create<HypermediaRepresentation>();
            var transition = representor.Transitions.First();
            var rel = Fixture.Create<string>();
            var name = Fixture.Create<string>();
            transition.Rel = rel;
            transition.Name = name;

            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();
            requestor.Stub(r => r.RequestTransitionAsync(transition)).Return(Task.FromResult(expected));

            var sut = new NavigateToTransitionQueryStep(t => t.Rel == rel && t.Name == name);

            var result = await sut.ExecuteAsync(representor, requestor);

            Assert.AreEqual(expected, result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_SetsNullRepresentor()
        {
            var representor = Fixture.Create<HypermediaRepresentation>();
            var transition = representor.Transitions.First();
            var rel = transition.Rel;

            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();

            var sut = new NavigateToTransitionQueryStep(rel);

            var result = await sut.ExecuteAsync(null, requestor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_SetsNullHandler()
        {
            var representor = Fixture.Create<HypermediaRepresentation>();
            var transition = representor.Transitions.First();
            var rel = Fixture.Create<string>();
            var name = Fixture.Create<string>();
            transition.Rel = rel;
            transition.Name = name;

            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();

            var sut = new NavigateToTransitionQueryStep(t => t.Rel == rel && t.Name == name);

            var result = await sut.ExecuteAsync(representor, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LocateTransition_SetsNull()
        {
            var representor = Fixture.Create<HypermediaRepresentation>();
            var transition = representor.Transitions.First();
            var rel = transition.Rel;

            var sut = new NavigateToTransitionQueryStep(rel);
            sut.LocateTransition(null);
        }
    }
}
