using System.Threading;
using System.Threading.Tasks;
using QUTyTest.Models;

namespace QUTyTest.Interfaces
{
    public interface IQUTyTest
    {
        Task Test(QUTy device, CancellationToken token);
    }
}