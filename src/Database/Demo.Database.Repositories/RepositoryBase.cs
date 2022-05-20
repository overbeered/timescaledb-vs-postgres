using Microsoft.EntityFrameworkCore;

namespace Demo.Database.Repositories
{
    public abstract class RepositoryBase<TContext, T> : IRepositoryBase<T> where T : class where TContext : DbContext, IDisposable
    {
        protected TContext _context;

        public RepositoryBase(TContext context) => _context = context;

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                _context.Dispose();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }
    }
}
