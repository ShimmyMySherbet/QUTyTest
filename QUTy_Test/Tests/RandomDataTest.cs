using System;
using System.Threading;
using System.Threading.Tasks;
using QUTyTest.Attributes;
using QUTyTest.Interfaces;
using QUTyTest.Models;

namespace QUTyTest.Tests
{
    [Order(5)]
    public class RandomDataTest : IQUTyTest
    {
        private string[] _Garbage = new[]
        {
            "fh76y3rgnf34qgf0q3fgn03qg",
            "hfn\\8423fgn038f03q87f038\\ffre",
            "\0\0\0\0r"
        };

        private string[] _InvalidCommands = new[]
        {
            "\\\\mfffewfwe",
            "\\\\rddddd",
        };

        private string[] _InvalidCase = new[]
        {
            "\\\\S",
            "\\\\T",
            "\\\\E",
            "\\\\P",
            "\\\\Y",
            "\\\\I00"
        };

        public async Task Test(QUTy device, CancellationToken token)
        {
            Console.WriteLine("Testing garbage input...");
            foreach (var message in _Garbage)
            {
                device.Write(message);
                await device.ExpectResponse(null, 2);
            }

            Console.WriteLine("Testing invalid input...");
            foreach (var message in _InvalidCommands)
            {
                device.Write(message);
                await device.ExpectResponse(null);
            }

            Console.WriteLine("Testing bad case commands...");
            foreach (var message in _InvalidCase)
            {
                device.Write(message);
                await device.ExpectResponse(null);
            }
        }
    }
}