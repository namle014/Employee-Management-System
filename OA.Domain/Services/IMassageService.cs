using OA.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OA.Core.VModels;

namespace OA.Core.Services
{
    public interface IMassageService
    {
        Task Create(MessageCreateVModel model);
        Task<ResponseResult> GetAll();
    }
}
