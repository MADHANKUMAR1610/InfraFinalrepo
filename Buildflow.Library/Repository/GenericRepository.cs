using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Library.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected BuildflowAppContext Context;
        private readonly ILogger<GenericRepository<T>> _logger;
        protected DbSet<T> DbSet;
        public GenericRepository(
            BuildflowAppContext context, ILogger<GenericRepository<T>> logger)
        {
            Context = context;
            DbSet = Context.Set<T>();
            _logger = logger;
        }
        public async Task<bool> Add(T entity)
        {
            await DbSet.AddAsync(entity);
            return true;
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression);
        }
        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression)
        {
            _logger.LogInformation("entered GetAll " + nameof(T));
            return await DbSet.Where(expression).ToListAsync();
        }
        public async Task<T?> GetById(dynamic id)
        {
            _logger.LogInformation("entered getbyod " + nameof(T));

            return await DbSet.FindAsync(id);
        }
        public async Task<T?> GetById(Expression<Func<T, bool>> expression)
        {
            _logger.LogInformation("entered getbyid " + nameof(T));

            return await DbSet.Where(expression).SingleOrDefaultAsync();
        }
        public async Task<bool> Remove(dynamic id)
        {
            var t = await DbSet.FindAsync(id);
            if (t == null) return false;
            DbSet.Remove(t);
            return true;
        }
        public Task<bool> Update(T entity)
        {
            DbSet.Update(entity);
            return Task.FromResult(true);
        }
        public async Task<bool> Delete(dynamic id)
        {
            var t = await DbSet.FindAsync(id);
            if (t == null) return false;
            t.is_active = false;
            return true;
        }

        public async Task<IEnumerable<T>> GetAllDataAsync(
    Expression<Func<T, bool>> expression,
    Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            _logger.LogInformation("entered GetAll " + nameof(T));

            IQueryable<T> query = DbSet.Where(expression);

            if (include != null)
            {
                query = include(query);
            }

            return await query.ToListAsync();
        }
    }
}
