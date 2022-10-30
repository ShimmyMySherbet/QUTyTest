using QUTyTest.Models;

namespace QUTyTest.Interfaces
{
    public interface IQUTyMessage
    {
        EMessageType Type { get; }

        string GetContent();
    }
}