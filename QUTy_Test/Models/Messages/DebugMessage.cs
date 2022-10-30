using QUTyTest.Interfaces;

namespace QUTyTest.Models.Messages
{
    public class DebugMessage : IQUTyMessage
    {
        public EMessageType Type => EMessageType.Debug;

        private string _Message { get; }

        public DebugMessage(string message)
        {
            _Message = message;
        }

        public string GetContent() => $"[Debug] {_Message}";
    }
}