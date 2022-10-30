using QUTyTest.Interfaces;

namespace QUTyTest.Models.Messages
{
    public class Disconnected : IQUTyMessage
    {
        public EMessageType Type => EMessageType.Disconnected;

        public string GetContent() => "The QUTy was unexpectedly disconnected";
    }
}