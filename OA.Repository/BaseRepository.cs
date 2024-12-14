using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using System.Linq.Expressions;
namespace OA.Repository
{
    public class BaseRepository<T> : GlobalVariables, IBaseRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<T> _entities;
        private readonly ILogger _logger;
        public BaseRepository(ApplicationDbContext dbContext, ILogger<BaseRepository<T>> logger, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _entities = dbContext.Set<T>();
            _logger = logger;
        }
        public async Task<Pagination> GetAllPagination(int pageNumber, int pageSize, Expression<Func<T, bool>>? where = null,
            Expression<Func<T, dynamic>>? orderDesc = null, Expression<Func<T, dynamic>>? orderAsc = null)
        {
            IQueryable<T> query = _entities; //.AsNoTracking(); if use AsNoTracking, you cant use EntryReference()
            if (where != null)
            {
                query = query.Where(where);
            }
            if (orderAsc != null)
            {
                query = query.OrderBy(orderAsc);
            }
            if (orderDesc != null)
            {
                query = query.OrderByDescending(orderDesc);
            }
            var data = await Task.FromResult(query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());
            return new Pagination
            {
                Records = data,
                TotalRecords = query.Count()
            };
        }
        public async Task<T?> GetById(int id)
        {
            var entity = await _entities.FindAsync(id);
            return entity;
        }
        public async Task<ResponseResult> Create(T entity)
        {
            var result = new ResponseResult();
            if (entity != null)
            {
                entity.CreatedDate = DateTime.Now;
                entity.CreatedBy = GlobalUserName;
                entity.IsActive = entity.IsActive ? entity.IsActive : CommonConstants.Status.InActive;
                _entities.Add(entity);
                result.Data = await SaveChanges(result) ? entity : null;
            }
            return result;
        }
        public async Task<ResponseResult> Update(T entity)
        {
            var result = new ResponseResult();
            if (entity != null)
            {
                T? data = await GetById(entity.Id);
                if (data != null)
                {
                    entity.UpdatedDate = DateTime.Now;
                    entity.UpdatedBy = GlobalUserName;
                    _dbContext.Entry(data).CurrentValues.SetValues(entity);
                    _dbContext.Entry(data).Property(e => e.CreatedDate).IsModified = false;
                    _dbContext.Entry(data).Property(e => e.CreatedBy).IsModified = false;
                    result.Data = await SaveChanges(result) ? entity : null;
                }
            }
            return result;
        }
        public async Task<ResponseResult> UpdateCountView(T entity)
        {
            var result = new ResponseResult();
            if (entity != null)
            {
                T? data = await GetById(entity.Id);
                if (data != null)
                {
                    _dbContext.Entry(data).CurrentValues.SetValues(entity);
                    _dbContext.Entry(data).Property(e => e.IsActive).IsModified = false;
                    _dbContext.Entry(data).Property(e => e.CreatedBy).IsModified = false;
                    _dbContext.Entry(data).Property(e => e.CreatedDate).IsModified = false;
                    _dbContext.Entry(data).Property(e => e.CreatedBy).IsModified = false;
                    result.Data = await SaveChanges(result) ? entity : null;
                }
            }
            return result;
        }
        public async Task<ResponseResult> UpdateMany(IEnumerable<T> entities)
        {
            var result = new ResponseResult();
            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    T? data = await GetById(entity.Id);
                    if (data != null)
                    {
                        entity.UpdatedDate = DateTime.Now;
                        entity.UpdatedBy = GlobalUserName;
                        _dbContext.Entry(data).CurrentValues.SetValues(entity);
                        _dbContext.Entry(data).Property(e => e.CreatedDate).IsModified = false;
                        _dbContext.Entry(data).Property(e => e.CreatedBy).IsModified = false;
                        result.Data = await SaveChanges(result) ? entity : null;
                    }

                }
            }
            return result;
        }
        public async Task<ResponseResult> Remove(int id)
        {
            var result = new ResponseResult();
            T? entity = await GetById(id);
            if (entity != null)
            {
                _entities.Remove(entity);
                await SaveChanges(result);
            }
            return result;
        }
        public async Task<ResponseResult> RemoveAll(IEnumerable<T> entities)
        {
            var result = new ResponseResult();
            _entities.RemoveRange(entities);
            await SaveChanges(result);
            return result;
        }
        public IQueryable<T> AsQueryable()
        {
            var query = _entities.AsQueryable();
            return query;
        }
        public async Task<IEnumerable<T>> Where(Expression<Func<T, bool>> where)
        {
            return await Task.FromResult(_entities.Where(where).ToList());
        }
        public void EntryReference(T entity, Expression<Func<T, dynamic?>> entityReference)
        {
            _dbContext.Entry(entity).Reference(entityReference).Load();
        }
        public void EntryCollection(T entity, Expression<Func<T, IEnumerable<dynamic>>> entityCollection)
        {
            _dbContext.Entry(entity).Collection(entityCollection).Load();
        }
        public async Task<bool> SaveChanges(ResponseResult result)
        {
            try
            {
                result.Success = await _dbContext.SaveChangesAsync() >= 0;
            }
            catch (Exception ex)
            {
                var message = Utilities.MakeExceptionMessage(ex);
                _logger.LogError(message);
                result.Success = false;
            }
            return result.Success;
        }
    }
}
