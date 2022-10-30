using System;
using System.Threading;
using System.Threading.Tasks;
using QUTyTest.Attributes;
using QUTyTest.Interfaces;
using QUTyTest.Models;

namespace QUTyTest.Tests
{
    [Order(1)]
    public class FirstSequenceTest : IQUTyTest
    {
        public async Task Test(QUTy device, CancellationToken token)
        {
            Console.WriteLine($"Selecting index 0...");
            device.Write("\\\\i00");
            await device.ExpectResponse(EMessageType.Ack);

            Console.WriteLine($"Starting sequence playback...");
            device.Write("\\\\s");
            await device.ExpectResponse(EMessageType.Ack);

            Console.WriteLine("Sequence should be playing.");

            await Task.Delay(5000);

            Console.WriteLine("QUTy should have stopped playing the tune by now, and returned to sequence selection");
            device.SendVisual();
        }
    }
}