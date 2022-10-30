using QUTyTest.Interfaces;

namespace QUTyTest.Models.Messages
{
    public class InvalidMessage : IQUTyMessage
    {
        public EMessageType Type => EMessageType.Invalid;

        private string Content;

        public InvalidMessage(string content)
        {
            Content = content;
        }

        public string GetContent() => Content;
    }
}