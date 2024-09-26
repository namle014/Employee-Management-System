using OA.Core.Models;
using OA.Domain.VModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Domain.Services
{
    public interface ISysProfileService
    {
        Task ChangePasswordProfile(UserChangePasswordVModel model);
        Task UpdateProfile(UserUpdateVModel model);
    }
}
