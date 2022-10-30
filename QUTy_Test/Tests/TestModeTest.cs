using System;
using System.Threading;
using System.Threading.Tasks;
using QUTyTest.Attributes;
using QUTyTest.Interfaces;
using QUTyTest.Models;

namespace QUTyTest.Tests
{
    [Order(1)]
    public class TestModeTest : IQUTyTest
    {
        public async Task Test(QUTy device, CancellationToken token)
        {
            Console.WriteLine("Switching to test mode...");

            device.Write("\\\\t");
            await device.ExpectResponse(EMessageType.Ack);

            Console.WriteLine();
            Console.WriteLine("Device should be in test mode now.");
            Console.WriteLine("The screen should be displaying '--'");

            await Task.Delay(5000);

            Console.WriteLine();
            Console.WriteLine("Exiting test mode...");

            device.Write("\\\\e");
            await device.ExpectResponse(EMessageType.Ack);

            Console.WriteLine("Device should now be back at sequence select");
            await Task.Delay(2000);
        }
    }
}