using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QUTyTest.Models
{
    public class QUTy
    {
        public SerialPort Port { get; }

        public QUTyReader Reader { get; }

        public QUTySync Sync { get; }

        public int TestsRan => Failed + Passed + OtherTests;

        public int Failed { get; private set; }
        public int Passed { get; private set; }
        public int OtherTests { get; private set; }

        public QUTy(SerialPort port, CancellationToken token)
        {
            Port = port;
            Reader = new QUTyReader(Port, token);
            Sync = new QUTySync(this, token);
        }

        /// <summary>
        /// Reads the next non-debug message from the device with a timeout. If it matches the expected response, it counts as a pass.
        /// <paramref name="type"/> can be null to expect no response within the timeout period.
        /// </summary>
        public async Task<bool> ExpectResponse(EMessageType? type = EMessageType.Ack, int timeoutSec = 5, bool printPassed = false)
        {
            var response = await Reader.ReadResponse(TimeSpan.FromSeconds(timeoutSec));

            if (type != response?.Type)
            {
                Console.WriteLine($"Fail: Device responded with {response?.Type.ToString() ?? "nothing"} ({response?.GetContent()})");
                Console.WriteLine($"   Expected: {(type.HasValue ? type.ToString() : "nothing")}");
                SendFailed();
                return false;
            }
            else if (printPassed)
            {
                SendPassed();
                Console.WriteLine($"Pass: Device responded with {response?.Type.ToString() ?? "nothing"}");
            }
            return true;
        }

        /// <summary>
        /// Encodes a message in ASCII and writes it to the QUTy Serial
        /// </summary>
        public void Write(string message)
        {
            var buffer = Encoding.ASCII.GetBytes(message);

            lock (Port)
            {
                Port.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Indicates that a test has passed
        /// </summary>
        public void SendPassed()
        {
            Passed++;
        }

        /// <summary>
        /// Indicates that a test has failed
        /// </summary>
        public void SendFailed()
        {
            Failed++;
        }

        /// <summary>
        /// Indicates that a test has completed, but isn't able to be auto-marked
        /// </summary>
        public void SendVisual()
        {
            OtherTests++;
        }
    }
}