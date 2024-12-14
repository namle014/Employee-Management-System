using AutoMapper;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;
namespace OA.Service
{
    public class BaseService<TEntity, TCreateVModel, TUpdateVModel, TGetByIdVModel, TGetAllVModel, TExport>
        where TEntity : BaseEntity
        where TGetByIdVModel : class
        where TExport : class
    {
        private readonly IBaseRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        public BaseService(IBaseRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<ResponseResult> GetById(int id)
        {
            var result = new ResponseResult();
            var entity = await _repository.GetById(id);
            if (entity != null)
            {
                GetByIdEntry(entity);
                result.Data = _mapper.Map<TEntity, TGetByIdVModel>(entity);
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            };
            return result;
        }

        public virtual async Task Create(TCreateVModel model)
        {
            var entityCreated = _mapper.Map<TCreateVModel, TEntity>(model);
            var createdResult = await _repository.Create(entityCreated);
            if (!createdResult.Success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "Object"));
            }
        }

        public virtual async Task Update(TUpdateVModel model)
        {
            var entity = await _repository.GetById((model as dynamic)?.Id);
            if (entity != null)
            {
                entity = _mapper.Map(model, entity);
                var updatedResult = await _repository.Update(entity);
                if (!updatedResult.Success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, "Object"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        public virtual async Task<ResponseResult> UpdateMany(IEnumerable<TUpdateVModel> models)
        {
            var result = new ResponseResult();
            foreach (var model in models)
            {
                var entity = await _repository.GetById((model as dynamic)?.Id);
                entity = _mapper.Map(model, entity);
                result = await _repository.Update(entity);
                if (!result.Success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, "Object"));
                }
            }
            return result;
        }

        public virtual async Task ChangeStatus(int id)
        {
            var entity = await _repository.GetById(id);
            if (entity != null)
            {
                entity.IsActive = !entity.IsActive;
                var updatedResult = await _repository.Update(entity);
                if (!updatedResult.Success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, "Object"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        public virtual async Task Remove(int id)
        {
            var entity = await _repository.GetById(id);
            if (entity != null)
            {
                var removedResult = await _repository.Remove(entity.Id);
                if (!removedResult.Success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, "Object"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        public virtual void GetAllEntry(TEntity entity)
        {
            //_repository.EntryReference(entity, x => x.LanguageId);
            // override this function in child class if needed
        }

        public virtual void GetByIdEntry(TEntity entity)
        {
            //_repository.EntryReference(entity, x => x.LanguageId);
            // override this function in child class if needed
        }
    }
}
