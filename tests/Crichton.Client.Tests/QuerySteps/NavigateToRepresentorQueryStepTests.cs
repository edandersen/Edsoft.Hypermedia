﻿using System;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Client.Tests.QuerySteps
{
    public class NavigateToRepresentorQueryStepTests : TestWithFixture
    {
        private NavigateToRepresentorQueryStep sut;
        private CrichtonRepresentor representor;
        private ITransitionRequestHandler requestor;

        [SetUp]
        public void Init()
        {
            Fixture = GetFixture();
            representor = Fixture.Create<CrichtonRepresentor>();
            sut = new NavigateToRepresentorQueryStep(representor);
            requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CTOR_SetsNullUrl()
        {
            var step = new NavigateToRepresentorQueryStep(null);
        }

        [Test]
        public async Task ExecuteAsync_ReturnsConstructorSetRepresentor()
        {
            var result = await sut.ExecuteAsync(representor, requestor);

            Assert.AreEqual(result, representor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_SetsNullRepresentor()
        {
            var result = await sut.ExecuteAsync(null, requestor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_SetsNullHandler()
        {
            var result = await sut.ExecuteAsync(representor, null);
        }
    }
}
