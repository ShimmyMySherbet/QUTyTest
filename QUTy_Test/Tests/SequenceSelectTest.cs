using System;
using System.Threading;
using System.Threading.Tasks;
using QUTyTest.Attributes;
using QUTyTest.Interfaces;
using QUTyTest.Models;

namespace QUTyTest.Tests
{
    [Order(0)]
    public class SequenceSelectTest : IQUTyTest
    {
        private string[] _Valid = new[] { "ff", "a0", "00", "aa", "2f" };
        private string[] _Invalid = new[] { "f-", "dz", "0H", ".9", "\\0" };

        public async Task Test(QUTy device, CancellationToken token)
        {
            foreach (var v in _Valid)
            {
                Console.WriteLine($"Setting selected index to {v.ToUpper()}...");
                device.Write($"\\\\i{v}");
                await device.ExpectResponse();
                await Task.Delay(800);
            }
            await Task.Delay(800);

            foreach (var v in _Invalid)
            {
                Console.WriteLine($"Setting selected index to {v.ToUpper()}...");
                device.Write($"\\\\i{v}");
                await device.ExpectResponse(EMessageType.Nack);
                await Task.Delay(800);
            }
        }
    }
}