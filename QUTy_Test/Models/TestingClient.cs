using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using QUTyTest.Attributes;
using QUTyTest.Interfaces;

namespace QUTyTest.Models
{
    public class TestingClient
    {
        public QUTy CurrentDevice { get; private set; }

        public List<IQUTyTest> Tests { get; } = new List<IQUTyTest>();

        private CancellationTokenSource _TokenSource { get; set; } = new CancellationTokenSource();

        public void LoadTests()
        {
            var tests = new List<(IQUTyTest test, int order)>();
            foreach (var testType in typeof(TestingClient).Assembly
                .GetExportedTypes()
                .Where(x => typeof(IQUTyTest).IsAssignableFrom(x) &&
                            !x.IsAbstract && !x.IsInterface))
            {
                var instance = (IQUTyTest)Activator.CreateInstance(testType);

                var order = testType.GetCustomAttribute<OrderAttribute>();

                tests.Add((instance, order?.Order ?? 100));
            }

            foreach (var test in tests.OrderBy(x => x.order))
            {
                Tests.Add(test.test);
            }
        }

        public void Cancel() => _TokenSource?.Cancel();

        public async Task RunTests(SerialPort port)
        {
            if (_TokenSource != null && !_TokenSource.IsCancellationRequested)
            {
                _TokenSource.Cancel();
            }

            _TokenSource = new CancellationTokenSource();

            var device = new QUTy(port, _TokenSource.Token);

            CurrentDevice = device;

            Console.WriteLine($"Running {Tests.Count} Tests...");

            device.Reader.Start();

            foreach (var test in Tests)
            {
                if (_TokenSource.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    await test.Test(device, _TokenSource.Token);

                    if (device.Reader.MessagesAvailable > 0)
                    {

                        var r = await device.Reader.ReadResponse(TimeSpan.FromSeconds(2));

                        if (r != null)
                        {
                            Console.WriteLine($"WARN: Excess response after test. Message: {r.Type} ({r.GetContent()}). Expected nothing");
                        }
                    }
                    await Task.Delay(1000);
                    Console.WriteLine();
                    Console.WriteLine();

                }
                catch(FullFailException)
                {
                    Console.WriteLine("Device sent too much invalid data. Testing aborted.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error running test: {ex.Message}");
                    continue;
                }

                if (device.Failed > 0)
                {
                    break;
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Testing Completed.");
            Console.WriteLine($"Tests ran: {device.TestsRan}. Passed: {device.Passed}, Failed: {device.Failed}");

            if (device.Failed == 0)
            {
                Console.WriteLine("All tests passed, but remember, this is not an official marking tool.");
                Console.WriteLine("Be sure to carefully review your code before submitting it.");
            }

            _TokenSource.Cancel();
            CurrentDevice = null;
        }
    }
}