using System;
using System.Threading;
using System.Threading.Tasks;
using QUTyTest.Attributes;
using QUTyTest.Interfaces;
using QUTyTest.Models;
using QUTyTest.Models.Sequencing;

namespace QUTyTest.Tests
{
    [Order(-1)]
    public class TestSequenceTest : IQUTyTest
    {
        public async Task Test(QUTy device, CancellationToken token)
        {
            Console.WriteLine("Opening test mode...");
            device.Write("\\\\t");
            await device.ExpectResponse();
            Console.WriteLine("Testing invalid sequence...");
            device.Write("\\\\uAAAAAAAAAAAAZZZAAAAAAAAAAAAAAAAAE");
            await device.ExpectResponse(EMessageType.Nack, printPassed: true);
            Console.WriteLine();

            Console.WriteLine("Testing valid sequence: 1Hz Flash Siren");

            var sequence = Sequences.Build1HzSiren(addChecksum: true, checksumInitial: 'u');

            device.Write($"\\\\u{sequence}");

            await device.ExpectResponse(EMessageType.Ack, printPassed: true);

            Console.WriteLine("Sequence should be playing now, and play for 7 seconds");
            await Task.Delay(7000);
            Console.WriteLine($"Sequence should have terminated now.");
            Console.WriteLine();
            Console.WriteLine("Exit back to sequence test");
            device.Write("\\\\e");
            await device.ExpectResponse();
            await Task.Delay(4000);
        }
    }
}