using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace QUTyTest.Models
{
    public class QUTySync
    {
        public QUTy Device { get; }
        public bool SyncActive => _Task != null && _Task.IsCompleted;
        private CancellationToken Token { get; }
        private Task _Task { get; set; }

        public QUTySync(QUTy device, CancellationToken token)
        {
            Device = device;
            Token = token;
        }

        public void StartSync()
        {
            _Task = Task.Run(Worker);
        }

        private async Task Worker()
        {
            while (!Token.IsCancellationRequested)
            {
                await Task.Delay(4000, Token);
                Token.ThrowIfCancellationRequested();
                SendSyncNonblocking();
            }
            Debug.WriteLine("Exit Sync");
        }

        private void SendSyncNonblocking()
        {
            Task.Run(() =>
            {
                Debug.WriteLine("Send Sync");
                Device.Write("\\\\y");
                Device.Reader.DiscardNextAck();
            });
        }
    }
}