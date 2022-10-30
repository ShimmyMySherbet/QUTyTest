using System;
using System.Threading;
using System.Threading.Tasks;
using QUTyTest.Attributes;
using QUTyTest.Interfaces;
using QUTyTest.Models;

namespace QUTyTest.Tests
{
    [Order(2)]
    public class PausePlayTest : IQUTyTest
    {
        public async Task Test(QUTy device, CancellationToken token)
        {
            Console.WriteLine("Selecting sequence index 4...");
            device.Write("\\\\i04");
            await device.ExpectResponse();

            device.Write("\\\\s");
            await device.ExpectResponse();
            Console.WriteLine("The 7-Segment display should be flashing at a rate of 0.5Hz..");
            Console.WriteLine("Only the left digit should be visible.");
            await Task.Delay(4000);
            Console.WriteLine();
            Console.WriteLine("Pausing...");
            device.Write("\\\\p");
            await device.ExpectResponse();
            Console.WriteLine("Playback should now be paused (display not blinking)");
            await Task.Delay(3000);

            for (int i = 0; i < 4; i++)
            {
                token.ThrowIfCancellationRequested();
                Console.WriteLine("Advancing 1 step...");
                device.Write("\\\\n");
                await device.ExpectResponse();
                Console.WriteLine("The device's screen should have toggled.");
                await Task.Delay(2000);
            }

            Console.WriteLine("Exiting to sequence select...");
            device.Write("\\\\e");
            await device.ExpectResponse();
        }
    }
}