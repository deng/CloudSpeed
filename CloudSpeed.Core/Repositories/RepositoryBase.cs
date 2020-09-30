using Microsoft.EntityFrameworkCore;

namespace CloudSpeed.Repositories
{
    public abstract class RepositoryBase<T> where T : DbContext
    {
        protected T DbContext { get; private set; }

        protected RepositoryBase(T dbContext)
        {
            DbContext = dbContext;
        }
    }
}
