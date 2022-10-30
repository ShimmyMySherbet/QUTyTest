using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QUTyTest.Interfaces;
using QUTyTest.Models.Messages;

namespace QUTyTest.Models
{
    public class QUTyReader
    {
        public SerialPort Port { get; }

        public int MessagesAvailable => _MessageOut.Count;

        public bool IsMessageAvailable => MessagesAvailable > 0;

        public EMessageType CurrentMessageType { get; private set; } = EMessageType.Wait;

        public bool IsRunning => _ReadTask != null && !_ReadTask.IsCompleted;

        private CancellationToken _Token { get; }

        private Queue<IQUTyMessage> _MessageOut { get; } = new Queue<IQUTyMessage>();
        private SemaphoreSlim _Semaphore { get; } = new SemaphoreSlim(0);

        private Task _ReadTask { get; set; }
        private int _DiscardResponses { get; set; } = 0;

        private char[] AllowInitialCharacters = new[] { '?', '#' };

        public QUTyReader(SerialPort port, CancellationToken token)
        {
            Port = port;
            _Token = token;
        }

        public void Start()
        {
            _ReadTask = Task.Run(Read);
        }


        public async Task<IQUTyMessage> ReadMessageAsync(CancellationToken token = default)
        {
            await _Semaphore.WaitAsync(token);
            if (token.IsCancellationRequested)
            {
                return null;
            }
            return _MessageOut.Dequeue();
        }

        /// <summary>
        /// Marks the next Ack or Nack to be discarded
        /// Used to clear responses from background commands
        /// </summary>
        public void DiscardNextAck()
        {
            _DiscardResponses++;
        }

        public async Task<IQUTyMessage> ReadResponse(TimeSpan timeout)
        {
            var token = new CancellationTokenSource();
            token.CancelAfter(timeout);

            try
            {
                var invalids = 0;
                while (true)
                {
                    var msg = await ReadMessageAsync(token.Token);
                    if (msg == null)
                    {
                        return null;
                    }
                    if (msg.Type == EMessageType.Debug)
                    {
                        Console.WriteLine(msg.GetContent());
                        continue;
                    }
                    else if (msg.Type == EMessageType.Invalid)
                    {
                        Console.WriteLine($"Fail: {msg.GetContent()}");
                        invalids++;

                        if (invalids >= 10)
                        {
                            throw new FullFailException();
                        }
                    }
                    return msg;
                }
            }
            catch (TaskCanceledException)
            {
                return null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        private void Read()
        {
            var sb = new StringBuilder();

            while (true)
            {
                var chaValue = Port.ReadChar();

                if (_Token.IsCancellationRequested)
                {
                    return;
                }

                if (chaValue == -1)
                {
                    Release(new Disconnected());
                    return;
                }

                var cha = (char)chaValue;

                if (cha == '\n')
                {
                    Dispatch(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    if (sb.Length == 0)
                    {
                        switch (cha)
                        {
                            case '?':
                                CurrentMessageType = EMessageType.Debug;    // Debug message
                                break;

                            case '#':
                                CurrentMessageType = EMessageType.Response;  // Ack or Nack
                                break;

                            default:
                                Release(new InvalidMessage($"Unexpected character '{cha}' read. Expected '?' or '#' at start of new message"));
                                continue;
                        }
                    }

                    sb.Append(cha);
                }
            }
        }

        private void Release(IQUTyMessage message)
        {
            if (message.Type == EMessageType.Debug)
            {
                Console.WriteLine(message.GetContent());
                return;
            }
            else if (message.Type == EMessageType.Ack || message.Type == EMessageType.Nack && _DiscardResponses > 0)
            {
                _DiscardResponses--;
                return;
            }

            _MessageOut.Enqueue(message);
            _Semaphore.Release(1);
        }

        private void Dispatch(string message)
        {
            if (message == "#ACK")
            {
                Release(new ResponseMessage(EMessageType.Ack));
            }
            else if (message == "#NACK")
            {
                Release(new ResponseMessage(EMessageType.Nack));
            }
            else if (message.StartsWith("? "))
            {
                Release(new DebugMessage(message.Substring(2)));
            }
            else
            {
                // Invalid messages

                if (message.Equals("#ACK", StringComparison.InvariantCultureIgnoreCase))
                {
                    Release(new InvalidMessage($"Read '{message}', Should be '#ACK' (upper-case)"));
                }
                else if (message.Equals("#NACK", StringComparison.InvariantCultureIgnoreCase))
                {
                    Release(new InvalidMessage($"Read '{message}', Should be '#NACK' (upper-case)"));
                }
                Release(new InvalidMessage($"Invalid response: '{message}'"));
            }
        }
    }
}