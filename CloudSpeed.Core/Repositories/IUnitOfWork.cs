using System.Threading.Tasks;

namespace CloudSpeed.Repositories
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}
