using System;
using System.Threading.Tasks;
using HelloWorldInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans;
using Orleans.TestingHost;

namespace Tests
{
    /// <summary>
    /// -----------------------------------------------------------------
    /// How this works:
    /// 
    /// 1. The constructore of the <c>TestingSiloHost</c> base class 
    ///    will create an in-process test silo environment for this tets class, 
    ///    which (by default) will be shared by all test cases in this class.
    /// 2. The default test environment will contain a mini cluster of 2 silos, 
    ///    each running in seperate AppDomains in the current process.
    ///    The silos will be named Primary and Secondary.
    /// 3. The Orleans client environment will be initialized in the main AppDomain
    ///    where this test class is running, so each of the test cases can assume 
    ///    that everything is initialized at the start of their execution.
    /// 4. The configuration used for the test silos and test client are based on 
    ///    the <c>TestSiloOptions</c> and <c>TestClientOptions</c> config object 
    ///    which can optionally be passed in to the base class constructor.
    /// 5. There are also various utility methods in the <c>TestingSiloHost</c> class
    ///    that allow silos to be stopped or restarted to allow tests to programmatically 
    ///    simulat some simple failure mode conditions.
    /// 6. TestingSiloHost is agnostic to the test framework being used. 
    ///    The test cases here are written as normal MsTest code, although any similar 
    ///    testing framework such as NUnit or xUnit could have been used instead.
    /// ----------------------------------------------------------------- */
    /// </summary>
    [TestClass]
    public class HelloWorldSiloTests : TestingSiloHost
    {
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Optional. 
            // By default, the next test class which uses TestignSiloHost will
            // cause a fresh Orleans silo environment to be created.
            StopAllSilos();
        }

        [TestMethod]
        public async Task SayHelloTest()
        {
            // The Orleans silo / client test environment is already set up at this point.

            const long id = 0;
            const string greeting = "Bonjour";

            IHello grain = GrainFactory.GetGrain<IHello>(id);
            
            // This will create and call a Hello grain with specified 'id' in one of the test silos.
            string reply = await grain.SayHello(greeting);
            
            Assert.IsNotNull(reply, "Grain replied with some message");
            string expected = string.Format("You said: '{0}', I say: Hello!", greeting);
            Assert.AreEqual(expected, reply, "Grain replied with expected message");
        }
    }
}
