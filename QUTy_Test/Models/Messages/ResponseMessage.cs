using QUTyTest.Interfaces;

namespace QUTyTest.Models.Messages
{
    public class ResponseMessage : IQUTyMessage
    {
        public EMessageType Type { get; }

        public ResponseMessage(EMessageType type)
        {
            Type = type;
        }

        public string GetContent() => Type.ToString();
    }
}