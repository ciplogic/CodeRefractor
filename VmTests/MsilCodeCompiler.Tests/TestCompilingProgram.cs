using MsilCodeCompiler.Tests.Shared;
using NUnit.Framework;

namespace MsilCodeCompiler.Tests
{
    [TestFixture]
    public class TestCompilingProgram : CompilingProgramBase
    {
        [Test]
        public void TestCompilation()
        {
            Assert.IsTrue(TryCompileCSharp(@"Console.Write(2);"));
        }

        [Test]
        public void TestPrimesCount()
        {
            var bodyOfMain = @"
            var len = 5000000;
            var primes = 0;
            for(var i=2;i<len;i++)
            {
                if (i%2 == 0)
                    continue;
                var isPrime = true;
                for(var j = 2; j*j<=i;j++)
                {
                    if (i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if(isPrime)
                    primes++;
            }
            Console.Write(primes);
";
            Assert.IsTrue(TryCSharpMain(bodyOfMain));
            
        }
    }
}
