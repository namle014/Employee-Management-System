using AngleSharp.Dom;
using Aspose.Pdf;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace OA.Service
{
    public class TimekeepingService : GlobalVariables, ITimekeepingService
    {
        private readonly IBaseRepository<SysConfiguration> _sysConfigRepo;
        private readonly ApplicationDbContext _dbContext;
        private DbSet<Timekeeping> _timekeepings;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
        string _nameService = "Timekeeping";
        private DbSet<Department> _departments;

        public TimekeepingService(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, IBaseRepository<SysConfiguration> sysConfigRepo,
            IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _sysConfigRepo = sysConfigRepo;
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _timekeepings = dbContext.Set<Timekeeping>();
            _departments = dbContext.Set<Department>();
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ResponseResult> GetAllDepartments()
        {
            var result = new ResponseResult();
            var data = _departments.AsQueryable();
            result.Data = await data.ToListAsync();
            return result;
        }

        public async Task<ResponseResult> Search(FilterTimekeepingVModel model)
        {
            var result = new ResponseResult();

            var query = _timekeepings.AsQueryable();

            if (!string.IsNullOrEmpty(model.UserId))
            {
                query = query.Where(t => t.UserId == model.UserId);
            }

            if (model.StartDate.HasValue)
            {
                query = query.Where(t => t.Date >= model.StartDate.Value);
            }

            if (model.EndDate.HasValue)
            {
                query = query.Where(t => t.Date <= model.EndDate.Value);
            }

            if (model.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == model.IsActive.Value);
            }

            var timekeepingList = await query.ToListAsync();

            var timekeepingGrouped = timekeepingList.GroupBy(t => t.UserId);

            var timekeepingListMapped = new List<TimekeepingGetAllVModel>();

            foreach (var group in timekeepingGrouped)
            {
                var user = await _userManager.FindByIdAsync(group.Key);
                if (user == null)
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, $"UserId = {group.Key}"));
                }

                foreach (var timekeeping in group)
                {
                    var entityMapped = _mapper.Map<Timekeeping, TimekeepingGetAllVModel>(timekeeping);
                    entityMapped.FullName = user.FullName;
                    timekeepingListMapped.Add(entityMapped);
                }
            }

            result.Data = timekeepingListMapped;

            return result;
        }

        public async Task<ResponseResult> GetById(int id)
        {
            var result = new ResponseResult();

            var entity = await _timekeepings.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            var user = await _userManager.FindByIdAsync(entity.UserId);
            if (user == null)
            {
                throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, "User"));
            }

            var entityMapped = _mapper.Map<Timekeeping, TimekeepingGetByIdVModel>(entity);
            entityMapped.FullName = user.FullName;

            result.Data = entityMapped;

            return result;
        }

        public async Task Create(TimekeepingCreateVModel model)
        {
            var entity = _mapper.Map<TimekeepingCreateVModel, Timekeeping>(model);

            entity.IsActive = CommonConstants.Status.Active;

            _timekeepings.Add(entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
            }
        }

        public async Task Update(TimekeepingUpdateVModel model)
        {
            var entity = await _timekeepings.FindAsync(model.Id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            _mapper.Map(model, entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;

            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }

        public async Task ChangeStatus(int id)
        {
            var entity = await _timekeepings.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            entity.IsActive = !entity.IsActive;

            bool success = await _dbContext.SaveChangesAsync() > 0;

            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }

        public async Task Remove(int id)
        {
            var entity = await _timekeepings.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            _timekeepings.Remove(entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, _nameService));
            }
        }

        public async Task<ResponseResult> CreateUser(TimekeepingCreateUserVModel model)
        {
            if (string.IsNullOrEmpty(GlobalUserId))
            {
                throw new ForbiddenException("Vui lòng đăng nhập!");
            }

            var morning_time = await _sysConfigRepo.Where(x => x.Key == "MORNING_TIME" && x.Type == "workship");
            var lunch_break = await _sysConfigRepo.Where(x => x.Key == "LUNCH_BREAK" && x.Type == "workship");
            var afternoon_time = await _sysConfigRepo.Where(x => x.Key == "AFTERNOON_TIME" && x.Type == "workship");
            var quitting_time = await _sysConfigRepo.Where(x => x.Key == "QUITTING_TIME" && x.Type == "workship");
            var ipAddress = (await _sysConfigRepo.Where(x => x.Key == "IP_ADDRESS" && x.Type == "ip")).FirstOrDefault();
            var listIP = new List<string>();

            if (ipAddress != null && !string.IsNullOrEmpty(ipAddress.Value))
            {
                listIP = ipAddress.Value.Split(',')
                                .Select(ip => ip.Trim().Replace("[", "").Replace("]", ""))  // Loại bỏ dấu ngoặc vuông
                                .ToList();
            }

            if (morning_time.Count() == 0)
            {
                throw new BadRequestException("Không có thời gian chấm công sáng trong hệ thống!");
            }

            if (afternoon_time.Count() == 0)
            {
                throw new BadRequestException("Không có thời gian chấm công chiều trong hệ thống!");
            }

            bool status = false;

            if (listIP.Count() != 0)
            {
                if (listIP.Contains(model.IPAddress))
                {
                    status = true;
                }
            }

            var entity = new Timekeeping
            {
                UserId = GlobalUserId,
                Date = DateTime.Now,
                CheckInTime = DateTime.Now.TimeOfDay,
                CheckInIP = model.IPAddress,
                CheckOutTime = null,
                IsActive = true,
                Status = status,
            };

            _timekeepings.Add(entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
            }

            var result = new ResponseResult();

            if (status)
            {
                result.Data = new
                {
                    Id = entity.Id
                };
            }
            else
            {
                result.Data = new
                {
                    Id = 0
                };
            }

            return result;
        }

        public async Task CheckOut(CheckOutVModel model)
        {
            if (GlobalUserId == "" || GlobalUserId == null)
            {
                throw new ForbiddenException("Vui lòng đăng nhập!");
            }

            var attendance = await _timekeepings.Where(x => x.Id == model.Id).FirstOrDefaultAsync();

            if (attendance == null)
            {
                throw new BadRequestException("Không tìm thấy chấm công!");
            }

            attendance.CheckOutTime = DateTime.Now.TimeOfDay;

            if (attendance.CheckOutTime.HasValue)
            {
                attendance.TotalHours = (attendance.CheckOutTime.Value - attendance.CheckInTime).TotalHours;
            }
            else
            {
                attendance.TotalHours = 0;
            }

            _timekeepings.Update(attendance);

            bool success = await _dbContext.SaveChangesAsync() > 0;

            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }

        public async Task<ResponseResult> SearchForUser(FilterTimekeepingForUserVModel model)
        {
            var result = new ResponseResult();

            if (string.IsNullOrEmpty(GlobalUserId))
            {
                throw new ForbiddenException("Vui lòng đăng nhập!");
            }

            var morning_time = (await _sysConfigRepo.Where(x => x.Key == "MORNING_TIME" && x.Type == "workship")).FirstOrDefault();
            var lunch_break = (await _sysConfigRepo.Where(x => x.Key == "LUNCH_BREAK" && x.Type == "workship")).FirstOrDefault();
            var afternoon_time = (await _sysConfigRepo.Where(x => x.Key == "AFTERNOON_TIME" && x.Type == "workship")).FirstOrDefault();
            var quitting_time = (await _sysConfigRepo.Where(x => x.Key == "QUITTING_TIME" && x.Type == "workship")).FirstOrDefault();

            if (morning_time == null)
            {
                throw new BadRequestException("Không có thời gian chấm công sáng trong hệ thống!");
            }

            if (afternoon_time == null)
            {
                throw new BadRequestException("Không có thời gian chấm công chiều trong hệ thống!");
            }

            if (lunch_break == null)
            {
                throw new BadRequestException("Không có thời gian nghỉ trưa trong hệ thống!");
            }

            if (quitting_time == null)
            {
                throw new BadRequestException("Không có thời gian ra về trong hệ thống!");
            }

            if (!TimeSpan.TryParse(morning_time.Value, out var morningTimeSpan))
            {
                throw new BadRequestException("Không thể chuyển đổi thời gian sáng trong hệ thống!");
            }

            // Chuyển các thời gian khác như lunch_break, afternoon_time, quitting_time thành TimeSpan nếu cần
            TimeSpan lunchBreakTimeSpan;
            if (!TimeSpan.TryParse(lunch_break.Value, out lunchBreakTimeSpan))
            {
                throw new BadRequestException("Không thể chuyển đổi thời gian nghỉ trưa trong hệ thống!");
            }

            TimeSpan afternoonTimeSpan;
            if (!TimeSpan.TryParse(afternoon_time.Value, out afternoonTimeSpan))
            {
                throw new BadRequestException("Không thể chuyển đổi thời gian chiều trong hệ thống!");
            }

            TimeSpan quittingTimeSpan;
            if (!TimeSpan.TryParse(quitting_time.Value, out quittingTimeSpan))
            {
                throw new BadRequestException("Không thể chuyển đổi thời gian ra về trong hệ thống!");
            }

            var records = await _timekeepings
                        .Where(x => x.UserId == GlobalUserId
                                    && (!model.StartDate.HasValue || x.Date >= model.StartDate.Value)
                                    && (!model.EndDate.HasValue || x.Date <= model.EndDate.Value)
                                    && ((x.Status == true && x.Status == false)
                                    || (model.IsLate == true && (
                                            (x.CheckInTime < new TimeSpan(13, 0, 0) && x.CheckInTime > morningTimeSpan)
                                            || (x.CheckInTime >= new TimeSpan(13, 0, 0) && x.CheckInTime > afternoonTimeSpan)
                                        ))
                                        || (model.IsOnTime == true && (
                                            (x.CheckInTime <= morningTimeSpan && x.CheckInTime <= new TimeSpan(13, 0, 0) && (x.CheckOutTime == null || x.CheckOutTime >= lunchBreakTimeSpan))
                                            || (x.CheckInTime <= afternoonTimeSpan && (x.CheckOutTime == null || x.CheckOutTime >= quittingTimeSpan))
                                        ))
                                        || (model.IsEarly == true && (
                                            (x.CheckOutTime != null &&
                                            ((x.CheckOutTime < lunchBreakTimeSpan && x.CheckOutTime < new TimeSpan(13, 0, 0)) ||
                                                (x.CheckOutTime >= new TimeSpan(13, 0, 0) && x.CheckOutTime < quittingTimeSpan)))
                        ))
                        ))
                        .AsQueryable()
                        .ToListAsync();

            var totalRecords = records.Count();

            var notifications = records
                                    .Skip((model.PageNumber - 1) * model.PageSize)
                                    .Take(model.PageSize)
                                    .ToList();

            result.Success = true;
            result.Data = new Pagination();

            result.Data.TotalRecords = totalRecords;
            result.Data.Records = notifications;

            return result;
        }

        public async Task<ResponseResult> GetByDate(FilterTimekeepingGetByDateVModel model)
        {
            var result = new ResponseResult();

            if (string.IsNullOrEmpty(GlobalUserId))
            {
                throw new ForbiddenException("Vui lòng đăng nhập!");
            }
            var records = await _timekeepings
                        .Where(x => x.UserId == GlobalUserId
                                    && x.Date.Date == model.Date.Date)
                        .ToListAsync();

            result.Data = records;

            return result;
        }
    }
}
