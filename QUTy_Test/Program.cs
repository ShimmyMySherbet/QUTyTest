using System;
using System.IO.Ports;
using System.Threading;
using QUTyTest.Models;

namespace QUTyTest
{
    internal class Program
    {
        private static string SelectedPort { get; } = null;
        public static TestingClient Client { get; } = new TestingClient();

        private static SerialPort OpenPort()
        {
            if (!string.IsNullOrEmpty(SelectedPort))
            {
                return new SerialPort(SelectedPort, 9600);
            }
            while (true)
            {
                Console.Write("Enter the port name the QUTy is attach to (e.g., COM3): ");
                var portName = Console.ReadLine();
                try
                {
                    var port = new SerialPort(portName, 9600);
                    port.Open();
                    return port;
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Failed to open port, it is currently in use by another process.");
                    Console.WriteLine("Make sure your PlatformIO Port Monitor isn't running.");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to open port: {ex.Message}");
                }
            }
        }

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += CancelRequested;

            Client.LoadTests();

            while (true)
            {
                var port = OpenPort();

                Console.WriteLine("Please reset your QUTy board (waiting 4 sec...)");
                Thread.Sleep(4000);

                var task = Client.RunTests(port);
                task.Wait();

                port.Close();
                port.Dispose();
                Console.WriteLine($"Serial Port closed. Press any key to re-run tests");
                Console.ReadKey();
            }
        }

        private static void CancelRequested(object sender, ConsoleCancelEventArgs e)
        {
            if (Client.CurrentDevice != null)
            {
                Console.WriteLine("Aborting tests...");
                e.Cancel = true;
                Client.Cancel();
            }
        }
    }
}