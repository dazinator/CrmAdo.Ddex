using System;
using System.Linq;
using NUnit.Framework;

namespace CrmAdo.DdexProvider.IntegrationTests
{
    [TestFixture()]
    [Category("Experimentation")]
    public class Experiments : BaseTest
    {        
        
        [Test]
        [TestCase(TestName = "Experiment for loading icon resource")]
        public void Experiment_For_Loading_Manifest_Resource()
        {

            var ddexAssy = typeof(CrmObjectSelector).Assembly;


            // Get the stream that holds the resource
            // NOTE1: Make sure not to close this stream!
            // NOTE2: Also be very careful to match the case
            //        on the resource name itself

            var names = ddexAssy.GetManifestResourceNames();

            foreach (var item in names)
            {
                Console.WriteLine(item);
            }




        }

    }
}