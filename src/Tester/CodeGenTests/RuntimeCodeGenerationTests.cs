namespace Tester.CodeGenTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    using Orleans.Serialization;

    using UnitTests.Tester;

    /// <summary>
    /// Tests runtime code generation.
    /// </summary>
    [DeploymentItem("OrleansCodeGenerator.dll")]
    [TestClass]
    public class RuntimeCodeGenerationTests : UnitTestSiloHost
    {
        [TestInitialize]
        public void InitializeForTesting()
        {
            SerializationManager.InitializeForTesting();
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            StopAllSilos();
        }

        [TestMethod, TestCategory("BVT"), TestCategory("Functional"), TestCategory("CodeGen")]
        public async Task RuntimeCodeGenTest()
        {
            var grain = GrainFactory.GetGrain<IRuntimeCodeGenGrain<@event>>(Guid.NewGuid());
            var expected = new @event
            {
                Id = Guid.NewGuid(),
                @if = new List<@event> { new @event { Id = Guid.NewGuid() } },
                PrivateId = Guid.NewGuid(),
                @public = new @event { Id = Guid.NewGuid() },
                Enum = @event.@enum.@int
            };

            var actual = await grain.SetState(expected);
            Assert.IsNotNull(actual, "Result of SetState should be a non-null value.");
            Assert.IsTrue(expected.Equals(actual));

            var newActual = await grain.@static();
            Assert.IsTrue(expected.Equals(newActual), "Result of @static() should be equal to expected value.");
        }

        [TestMethod, TestCategory("BVT"), TestCategory("Functional"), TestCategory("CodeGen")]
        public async Task RuntimeCodeGenNestedGenericTest()
        {
            const int Expected = 123985;
            var grain = GrainFactory.GetGrain<INestedGenericGrain>(Guid.NewGuid());
            
            var nestedGeneric = new NestedGeneric<int> { Payload = new NestedGeneric<int>.Nested { Value = Expected } };
            var actual = await grain.Do(nestedGeneric);
            Assert.AreEqual(Expected, actual, "NestedGeneric<int>.Nested value should round-trip correctly.");
            
            var nestedConstructedGeneric = new NestedConstructedGeneric
            {
                Payload = new NestedConstructedGeneric.Nested<int> { Value = Expected }
            };
            actual = await grain.Do(nestedConstructedGeneric);
            Assert.AreEqual(Expected, actual, "NestedConstructedGeneric.Nested<int> value should round-trip correctly.");
        }
    }
}
