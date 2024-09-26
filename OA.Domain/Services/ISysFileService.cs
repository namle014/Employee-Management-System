using OA.Core.Models;
using OA.Core.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using System.Threading.Tasks;

namespace OA.Domain.Services
{
    public interface ISysFileService : IBaseService<SysFile, SysFileCreateVModel, SysFileUpdateVModel, SysFileGetByIdVModel, SysFileGetAllVModel>
    {
        Task<ResponseResult> GetAllByType(string fileType, int pageSize, int pageNumber);
        Task<ResponseResult> CreateBase64(SysFileCreateBase64VModel model);
        Task FileChunks(FileChunk fileChunk);
        Task<ResponseResult> UploadAvatar(FileChunk fileChunk);
        Task<ResponseResult> GetAll(FilterSysFileVModel model);
        Task<ResponseResult> RemoveByUrl(string url);
    }
}
