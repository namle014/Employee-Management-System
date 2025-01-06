using Aspose.Pdf;
using AutoMapper;
using Employee_Management_System.Hubs;
using Ganss.Xss;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OA.Service
{
    public class NotificationsService : GlobalVariables, INotificationsService
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<Notifications> _notifications;
        private DbSet<UserNotifications> _userNotifications;
        private DbSet<NotificationFiles> _notificationFiles;
        private DbSet<NotificationDepartments> _notificationDepts;
        private DbSet<NotificationRoles> _notificationRoles;
        private DbSet<SysFile> _sysFileRepo;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
        string _nameService = "Notifications";
        private readonly HtmlSanitizer _sanitizer;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly RoleManager<AspNetRole> _roleManager;
        private readonly IBaseRepository<Department> _deptRepo;

        public NotificationsService(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, HtmlSanitizer sanitizer,
                                    RoleManager<AspNetRole> roleManager, IBaseRepository<Department> deptRepo,
                                    IHubContext<NotificationHub> hubContext, IMapper mapper,
                                    IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _notifications = dbContext.Set<Notifications>();
            _userNotifications = dbContext.Set<UserNotifications>();
            _notificationFiles = dbContext.Set<NotificationFiles>();
            _notificationDepts = dbContext.Set<NotificationDepartments>();
            _notificationRoles = dbContext.Set<NotificationRoles>();
            _sysFileRepo = dbContext.Set<SysFile>();
            _sanitizer = sanitizer;
            _roleManager = roleManager;
            _deptRepo = deptRepo;
            _userManager = userManager;
            _mapper = mapper;
            _hubContext = hubContext;

            _sanitizer.AllowedTags.Clear();

            // Thẻ định dạng văn bản cơ bản
            _sanitizer.AllowedTags.Add("p");
            _sanitizer.AllowedTags.Add("br");
            _sanitizer.AllowedTags.Add("hr");
            _sanitizer.AllowedTags.Add("span");
            _sanitizer.AllowedTags.Add("div");

            // Định dạng text
            _sanitizer.AllowedTags.Add("b");
            _sanitizer.AllowedTags.Add("i");
            _sanitizer.AllowedTags.Add("u");
            _sanitizer.AllowedTags.Add("strong");
            _sanitizer.AllowedTags.Add("em");
            _sanitizer.AllowedTags.Add("mark");
            _sanitizer.AllowedTags.Add("small");
            _sanitizer.AllowedTags.Add("del");
            _sanitizer.AllowedTags.Add("ins");
            _sanitizer.AllowedTags.Add("sub");
            _sanitizer.AllowedTags.Add("sup");

            // Danh sách
            _sanitizer.AllowedTags.Add("ul");
            _sanitizer.AllowedTags.Add("ol");
            _sanitizer.AllowedTags.Add("li");

            // Links và media
            _sanitizer.AllowedTags.Add("a");
            _sanitizer.AllowedTags.Add("img");
            _sanitizer.AllowedTags.Add("video");
            _sanitizer.AllowedTags.Add("audio");
            _sanitizer.AllowedTags.Add("iframe"); // Cho embedded content

            // Bảng
            _sanitizer.AllowedTags.Add("table");
            _sanitizer.AllowedTags.Add("thead");
            _sanitizer.AllowedTags.Add("tbody");
            _sanitizer.AllowedTags.Add("tr");
            _sanitizer.AllowedTags.Add("th");
            _sanitizer.AllowedTags.Add("td");

            // Cho phép các thuộc tính style và class
            _sanitizer.AllowedAttributes.Add("class");
            _sanitizer.AllowedAttributes.Add("style");
            _sanitizer.AllowedAttributes.Add("href");
            _sanitizer.AllowedAttributes.Add("src");
            _sanitizer.AllowedAttributes.Add("alt");
            _sanitizer.AllowedAttributes.Add("width");
            _sanitizer.AllowedAttributes.Add("height");
            _sanitizer.AllowedAttributes.Add("target");
            _sanitizer.AllowedAttributes.Add("rel");
            _sanitizer.AllowedAttributes.Add("title");
            _sanitizer.AllowedAttributes.Add("data-*"); // Cho phép data attributes

            // Cho phép các CSS properties phổ biến
            _sanitizer.AllowedCssProperties.Add("color");
            _sanitizer.AllowedCssProperties.Add("background");
            _sanitizer.AllowedCssProperties.Add("background-color");
            _sanitizer.AllowedCssProperties.Add("font-family");
            _sanitizer.AllowedCssProperties.Add("font-size");
            _sanitizer.AllowedCssProperties.Add("font-weight");
            _sanitizer.AllowedCssProperties.Add("font-style");
            _sanitizer.AllowedCssProperties.Add("text-decoration");
            _sanitizer.AllowedCssProperties.Add("text-align");
            _sanitizer.AllowedCssProperties.Add("margin");
            _sanitizer.AllowedCssProperties.Add("padding");
            _sanitizer.AllowedCssProperties.Add("border");
            _sanitizer.AllowedCssProperties.Add("width");
            _sanitizer.AllowedCssProperties.Add("height");
            _sanitizer.AllowedCssProperties.Add("display");
            _sanitizer.AllowedCssProperties.Add("float");
            _sanitizer.AllowedCssProperties.Add("line-height");

            // Cho phép các URL schemes an toàn
            _sanitizer.AllowedSchemes.Add("http");
            _sanitizer.AllowedSchemes.Add("https");
            _sanitizer.AllowedSchemes.Add("mailto");
            _sanitizer.AllowedSchemes.Add("tel");
            _sanitizer.AllowedSchemes.Add("data"); // Cho base64 images

            // Cấu hình cho iframe (ví dụ: YouTube, Facebook embeds)
            _sanitizer.AllowedSchemes.Add("//");
            _sanitizer.AllowedCssProperties.Add("position");
            _sanitizer.AllowedCssProperties.Add("top");
            _sanitizer.AllowedCssProperties.Add("left");
            _sanitizer.AllowedCssProperties.Add("right");
            _sanitizer.AllowedCssProperties.Add("bottom");
        }

        public async Task<ResponseResult> StatNotificationByMonth(int month, int year)
        {
            var result = new ResponseResult();

            var query = _notifications.AsQueryable();

            var userNotify = _userNotifications.AsQueryable();

            var notifyOfMonth = await query
                 .Where(x => x.SentTime.Year == year && x.SentTime.Month == month).ToListAsync();

            var monthlyCounts = notifyOfMonth.Count();

            if (month == 1)
            {
                year--;
                month = 12;
            }
            else
            {
                month--;
            }

            var notifyOfPrevMonth = await query
                 .Where(x => x.SentTime.Year == year && x.SentTime.Month == month).ToListAsync();

            var monthlyCountsPrev = await query.Where(x => x.SentTime.Year == year && x.SentTime.Month == month).CountAsync();

            double growthRate = 0;

            if (monthlyCountsPrev > 0)
            {
                growthRate = (double)(monthlyCounts - monthlyCountsPrev) / monthlyCountsPrev * 100;
            }
            else
            {
                growthRate = monthlyCounts > 0 ? 100 : 0;
            }

            var readCounts = (from notification in notifyOfMonth
                              join userNotification in userNotify
                              on notification.Id equals userNotification.NotificationId
                              where userNotification.IsRead == true
                              select userNotification).Count();

            var totalCurrent = (from notification in notifyOfMonth
                                join userNotification in userNotify
                                on notification.Id equals userNotification.NotificationId
                                select userNotification).Count();

            var readCountsPrev = (from notification in notifyOfPrevMonth
                                  join userNotification in userNotify
                                  on notification.Id equals userNotification.NotificationId
                                  where userNotification.IsRead == true
                                  select userNotification).Count();

            var totalPrev = (from notification in notifyOfPrevMonth
                             join userNotification in userNotify
                             on notification.Id equals userNotification.NotificationId
                             select userNotification).Count();

            double x = totalCurrent != 0 ? 1.0 * readCounts / totalCurrent * 100 : 0;
            double y = totalPrev != 0 ? 1.0 * readCountsPrev / totalPrev * 100 : 0;

            double rateRead = 0;

            if (y > 0)
            {
                rateRead = (double)(x - y) / y * 100;
            }
            else
            {
                rateRead = x > 0 ? 100 : 0;
            }

            result.Data = new
            {
                Counts = monthlyCounts,
                Rate = growthRate,
                ReadCounts = x,
                RateRead = rateRead
            };

            return result;
        }

        public async Task<ResponseResult> StatNotificationByType(int year)
        {
            var result = new ResponseResult();

            var query = _notifications.AsQueryable();

            var listNotify = await query
                    .Where(x => x.SentTime.Year == year)
                    .GroupBy(x => x.Type)
                    .Select(x => new { Type = x.Key, Count = x.Count() })
                    .ToListAsync();

            var defaultTypes = new List<string>
                {
                    "Public", "Attendance", "Discipline", "Event", "Insurance", "Reward", "Benefit", "Salary"
                };

            // Tạo một dictionary với các loại thông báo mặc định và kết quả truy vấn
            var dictionary = defaultTypes.ToDictionary(
                type => type,
                type => listNotify.FirstOrDefault(x => x.Type == type)?.Count ?? 0
            );

            var totalCurrent = query.Where(x => x.SentTime.Year == year).Count();
            var totalPrev = query.Where(x => x.SentTime.Year == year - 1).Count();

            double rate = 0;

            if (totalPrev > 0)
            {
                rate = (double)(totalCurrent - totalPrev) / totalPrev * 100;
            }
            else
            {
                rate = totalCurrent > 0 ? 100 : 0;
            }

            result.Data = new
            {
                NotificationData = dictionary,
                Rate = rate,
            };

            return result;
        }

        public async Task<ResponseResult> CountNotifyReadByUser(FilterCountNotifyReadByUser model)
        {
            var result = new ResponseResult();

            var notificationQuery = _notifications.AsQueryable();

            var notifications = await notificationQuery
                            .Where(x =>
                                EF.Functions.DateDiffDay(model.StartDate, x.SentTime) >= 0 &&
                                EF.Functions.DateDiffDay(x.SentTime, model.EndDate) >= 0
                            )
                            .ToListAsync();

            var userNotifications = _userNotifications.AsQueryable();

            var users = await _userManager.Users.Where(x => model.FullName == null || x.FullName.ToLower().Contains(model.FullName.ToLower())).ToListAsync();

            var sysFile = _sysFileRepo.AsQueryable();

            var data = (from notification in notifications
                        join userNotify in userNotifications
                        on notification.Id equals userNotify.NotificationId
                        join user in users
                        on userNotify.UserId equals user.Id
                        join file in sysFile
                        on user.AvatarFileId equals file.Id into userFiles
                        from file in userFiles.DefaultIfEmpty()  // LEFT JOIN, sẽ có file là null nếu không có khớp
                        where userNotify.IsRead == true
                        group userNotify by new { user.Id, user.FullName, user.EmployeeId, file } into g
                        select new
                        {
                            UserId = g.Key.Id,
                            UserFullName = g.Key.FullName,
                            EmployeeId = g.Key.EmployeeId,
                            AvatarPath = g.Key.file != null && g.Key.file.Path != null ? "https://localhost:44381/" + g.Key.file.Path : null,
                            ReadCount = g.Count()
                        });


            var totalRecords = data.Count();

            data = data
                .Skip((model.PageNumber - 1) * 10)
                .Take(10).ToList();

            result.Data = new
            {
                Records = data,
                TotalRecords = totalRecords
            };

            return result;
        }

        public async Task<ResponseResult> Search(FilterNotificationsVModel model)
        {
            var result = new ResponseResult();

            var query = _notifications.AsNoTracking()
                .Where(x => (x.IsActive == model.IsActive) &&
                            (string.IsNullOrEmpty(model.Type) || x.Type == model.Type) &&
                            (string.IsNullOrEmpty(model.Title) || x.Title.ToLower().Contains(model.Title.ToLower())) &&
                            (model.SentDate == null || (model.SentDate.Value.Day == x.SentTime.Day && model.SentDate.Value.Month == x.SentTime.Month && model.SentDate.Value.Year == x.SentTime.Year)));

            int pageSize = model.PageSize > 0 ? model.PageSize : 100;
            int pageNumber = model.PageNumber > 0 ? model.PageNumber : 1;

            var totalRecords = await query.CountAsync();

            var notifications = await query
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync(); // Truy xuất danh sách đầu tiên

            var userNotifications = _userNotifications.AsQueryable();

            var list = new List<NotificationsGetAllVModel>();

            foreach (var x in notifications)
            {
                var user = await _userManager.FindByIdAsync(x.UserId);
                string? avatarPath = null;
                if (user?.AvatarFileId != null)
                {
                    var file = await _sysFileRepo.Where(x => x.Id == (int)user.AvatarFileId).FirstOrDefaultAsync();
                    avatarPath = file != null ? "https://localhost:44381/" + file.Path : null;
                }

                var receivedCount = await (from userNotification in userNotifications
                                           where x.Id == userNotification.NotificationId
                                           select x.Id).CountAsync();

                var readCount = await (from userNotification in userNotifications
                                       where x.Id == userNotification.NotificationId && userNotification.IsRead == true
                                       select x.Id).CountAsync();

                list.Add(new NotificationsGetAllVModel
                {
                    UserId = x.UserId,
                    Id = x.Id,
                    Title = x.Title,
                    Content = x.Content,
                    SentTime = x.SentTime,
                    Type = x.Type,
                    IsActive = x.IsActive,
                    FullName = user?.FullName,
                    AvatarPath = avatarPath,
                    ReceivedCount = receivedCount,
                    ReadCount = readCount,
                });
            }

            result.Data = new Pagination();
            result.Data.Records = list;
            result.Data.TotalRecords = totalRecords;

            return result;
        }

        public async Task<ResponseResult> GetById(int id)
        {
            var result = new ResponseResult();

            var entity = await _notifications.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }


            var notification = _mapper.Map<NotificationsGetByIdVModel>(entity);

            var user = await _userManager.FindByIdAsync(notification.UserId);
            if (user != null)
            {
                notification.FullName = user.FullName;
                var roles = await _userManager.GetRolesAsync(user);
                notification.Role = roles?.FirstOrDefault();
                notification.AvatarPath = user.AvatarFileId != null ? "https://localhost:44381/" + (await _sysFileRepo.FindAsync((int)user.AvatarFileId))?.Path : null;
            }

            var fileIds = await _notificationFiles.Where(x => x.NotificationId == id).Select(x => x.FileId).ToListAsync();

            var files = await _sysFileRepo
                .Where(x => fileIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Path
                })
                .ToListAsync();

            notification.ListFile = files.Select(x => ("https://localhost:44381/" + x.Path)).ToList();

            var userIds = await _userNotifications.Where(x => x.NotificationId == id).Select(x => x.UserId).ToListAsync();

            var users = await _userManager.Users.Where(x => userIds.Contains(x.Id)).ToListAsync();

            notification.ListUser = users.Select(x => x.FullName).ToList();

            var userReads = await _userNotifications.Where(x => x.NotificationId == id && x.IsRead == true).ToListAsync();

            notification.ListUserRead = userReads.Select(x => x.UserId).ToList();

            result.Data = notification;

            return result;
        }

        public async Task Create(NotificationsCreateVModel model)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, "User create"));
                }

                var trimmedContent = model.Content.Trim();

                var decodedContent = HttpUtility.HtmlDecode(trimmedContent);
                var sanitizedContent = _sanitizer.Sanitize(decodedContent);

                if (string.IsNullOrWhiteSpace(sanitizedContent))
                {
                    throw new ValidationException("Nội dung không hợp lệ sau khi xử lý");
                }

                model.Content = sanitizedContent;

                var entityCreate = _mapper.Map<Notifications>(model);
                entityCreate.SentTime = DateTime.UtcNow.AddHours(7);
                _notifications.Add(entityCreate);

                var success = await _dbContext.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
                }

                var notificationId = entityCreate.Id;

                var listSendUser = new List<NotificationsGetAllForUserVModel>();

                if (model.TypeToNotify == 3)
                {
                    if (model.ListUser != null && model.ListUser.Count() > 0)
                    {
                        var listUserNoti = model.ListUser.Select(id => new UserNotifications
                        {
                            NotificationId = notificationId,
                            UserId = id
                        }).ToList();

                        await _userNotifications.AddRangeAsync(listUserNoti);

                        success = await _dbContext.SaveChangesAsync() > 0;
                        if (!success)
                        {
                            throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
                        }

                        foreach (var userNoti in listUserNoti)
                        {
                            listSendUser.Add(new NotificationsGetAllForUserVModel()
                            {
                                Id = userNoti.Id,
                                UserId = userNoti.UserId,
                                Title = entityCreate.Title,
                                Content = entityCreate.Content,
                                SentTime = entityCreate.SentTime,
                                Type = entityCreate.Type,
                                IsRead = false,
                                NotificationId = notificationId,
                            });
                        }
                    }
                    else
                    {
                        throw new BadRequestException("Empty list user");
                    }
                }

                if (model.TypeToNotify == 1)
                {
                    var users = _userManager.Users.AsQueryable();
                    var listUserNoti = users.Select(user => new UserNotifications
                    {
                        NotificationId = notificationId,
                        UserId = user.Id
                    }).ToList();

                    await _userNotifications.AddRangeAsync(listUserNoti);

                    success = await _dbContext.SaveChangesAsync() > 0;
                    if (!success)
                    {
                        throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
                    }

                    foreach (var userNoti in listUserNoti)
                    {
                        listSendUser.Add(new NotificationsGetAllForUserVModel()
                        {
                            Id = userNoti.Id,
                            UserId = userNoti.UserId,
                            Title = entityCreate.Title,
                            Content = entityCreate.Content,
                            SentTime = entityCreate.SentTime,
                            Type = entityCreate.Type,
                            IsRead = false,
                            NotificationId = notificationId,
                        });
                    }
                }

                if (model.TypeToNotify == 2)
                {
                    var userIds = new HashSet<string>();
                    if (model.ListDept != null && model.ListDept.Count() > 0)
                    {
                        var listNotificationDepts = model.ListDept.Select(id => new NotificationDepartments
                        {
                            NotificationId = notificationId,
                            DepartmentId = id
                        }).ToList();

                        var usersInDepartments = await _dbContext.Users
                                        .Where(u => model.ListDept.Contains((int)u.DepartmentId))
                                        .Select(u => u.Id)
                                        .ToListAsync();
                        userIds.UnionWith(usersInDepartments);

                        await _notificationDepts.AddRangeAsync(listNotificationDepts);

                        success = await _dbContext.SaveChangesAsync() > 0;
                        if (!success)
                        {
                            throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
                        }
                    }

                    if (model.ListRole != null && model.ListRole.Count() > 0)
                    {
                        var usersInRoles = await _dbContext.UserRoles
                                        .Where(ur => model.ListRole.Contains(ur.RoleId))
                                        .Select(ur => ur.UserId)
                                        .ToListAsync();
                        userIds.IntersectWith(usersInRoles);

                        var listNotificationRoles = model.ListRole.Select(id => new NotificationRoles
                        {
                            NotificationId = notificationId,
                            RoleId = id
                        }).ToList();

                        await _notificationRoles.AddRangeAsync(listNotificationRoles);

                        success = await _dbContext.SaveChangesAsync() > 0;
                        if (!success)
                        {
                            throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
                        }
                    }

                    var listUserNoti = userIds.Select(userId => new UserNotifications
                    {
                        NotificationId = notificationId,
                        UserId = userId
                    }).ToList();

                    if (listUserNoti.Count() > 0)
                    {
                        await _userNotifications.AddRangeAsync(listUserNoti);

                        success = await _dbContext.SaveChangesAsync() > 0;
                        if (!success)
                        {
                            throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
                        }

                        foreach (var userNoti in listUserNoti)
                        {
                            listSendUser.Add(new NotificationsGetAllForUserVModel()
                            {
                                Id = userNoti.Id,
                                UserId = userNoti.UserId,
                                Title = entityCreate.Title,
                                Content = entityCreate.Content,
                                SentTime = entityCreate.SentTime,
                                Type = entityCreate.Type,
                                IsRead = false,
                                NotificationId = notificationId,
                            });
                        }
                    }
                }

                if (model.ListFile != null && model.ListFile.Count() > 0)
                {
                    var listNotificationFiles = model.ListFile.Select(id => new NotificationFiles
                    {
                        NotificationId = notificationId,
                        FileId = id
                    }).ToList();

                    await _notificationFiles.AddRangeAsync(listNotificationFiles);

                    success = await _dbContext.SaveChangesAsync() > 0;
                    if (!success)
                    {
                        throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
                    }
                }

                foreach (var sendUser in listSendUser)
                {
                    await _hubContext.Clients.User(sendUser.UserId).SendAsync("ReceiveNotifications", sendUser);
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message);
            }
        }

        public async Task<ResponseResult> GetCountIsNew()
        {
            if (GlobalUserId == "" || GlobalUserId == null)
            {
                throw new ForbiddenException("Vui lòng đăng nhập!");
            }

            var user = await _userManager.FindByIdAsync(GlobalUserId);
            if (user == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            var userNotifications = _userNotifications
                                        .Where(x => x.IsNew == true && x.UserId == GlobalUserId)
                                        .ToList();

            var result = new ResponseResult();
            result.Data = userNotifications.Count;

            return result;
        }

        public async Task UpdateIsNew()
        {
            if (GlobalUserId == "" || GlobalUserId == null)
            {
                throw new ForbiddenException("Vui lòng đăng nhập!");
            }

            var user = await _userManager.FindByIdAsync(GlobalUserId);
            if (user == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            var userNotifications = _userNotifications
                                        .Where(x => x.IsNew == true && x.UserId == GlobalUserId)
                                        .ToList();

            if (userNotifications.Count > 0)
            {
                userNotifications.ForEach(x => x.IsNew = false);

                var success = await _dbContext.SaveChangesAsync() > 0;

                if (!success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, "IsNew"));
                }
            }
        }

        public async Task Update(NotificationsUpdateVModel model)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var notification = await _notifications.FindAsync(model.Id);
                if (notification == null)
                {
                    throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                }

                var trimmedContent = model.Content.Trim();
                var decodedContent = HttpUtility.HtmlDecode(trimmedContent);
                var sanitizedContent = _sanitizer.Sanitize(decodedContent);

                if (string.IsNullOrWhiteSpace(sanitizedContent))
                {
                    throw new ValidationException("Nội dung không hợp lệ sau khi xử lý");
                }

                model.Content = sanitizedContent;

                _mapper.Map(model, notification);
                _notifications.Update(notification);

                await _dbContext.SaveChangesAsync();

                _dbContext.NotificationDepartments.RemoveRange(_dbContext.NotificationDepartments.Where(x => x.NotificationId == model.Id));
                _dbContext.NotificationRoles.RemoveRange(_dbContext.NotificationRoles.Where(x => x.NotificationId == model.Id));
                _dbContext.UserNotifications.RemoveRange(_dbContext.UserNotifications.Where(x => x.NotificationId == model.Id));
                _dbContext.NotificationFiles.RemoveRange(_dbContext.NotificationFiles.Where(x => x.NotificationId == model.Id));

                await _dbContext.SaveChangesAsync();


                if (model.TypeToNotify == 3)
                {
                    if (model.ListUser != null && model.ListUser.Any())
                    {
                        var userNotifications = model.ListUser.Select(userId => new UserNotifications
                        {
                            NotificationId = notification.Id,
                            UserId = userId
                        }).ToList();

                        await _dbContext.UserNotifications.AddRangeAsync(userNotifications);
                    }
                }
                else if (model.TypeToNotify == 1)
                {
                    var allUsers = await _userManager.Users.ToListAsync();
                    var userNotifications = allUsers.Select(user => new UserNotifications
                    {
                        NotificationId = notification.Id,
                        UserId = user.Id
                    }).ToList();

                    await _dbContext.UserNotifications.AddRangeAsync(userNotifications);
                }
                else if (model.TypeToNotify == 2)
                {
                    var userIds = new HashSet<string>();

                    if (model.ListDept != null && model.ListDept.Any())
                    {
                        var usersInDepartments = await _dbContext.Users
                            .Where(u => model.ListDept.Contains((int)u.DepartmentId))
                            .Select(u => u.Id)
                            .ToListAsync();
                        userIds.UnionWith(usersInDepartments);

                        var departmentNotifications = model.ListDept.Select(departmentId => new NotificationDepartments
                        {
                            NotificationId = notification.Id,
                            DepartmentId = departmentId
                        }).ToList();
                        await _dbContext.NotificationDepartments.AddRangeAsync(departmentNotifications);

                    }

                    if (model.ListRole != null && model.ListRole.Any())
                    {
                        var usersInRoles = await _dbContext.UserRoles
                           .Where(ur => model.ListRole.Contains(ur.RoleId))
                           .Select(ur => ur.UserId)
                           .ToListAsync();
                        userIds.IntersectWith(usersInRoles);

                        var roleNotifications = model.ListRole.Select(roleId => new NotificationRoles
                        {
                            NotificationId = notification.Id,
                            RoleId = roleId
                        }).ToList();

                        await _dbContext.NotificationRoles.AddRangeAsync(roleNotifications);
                    }

                    var userNotifications = userIds.Select(userId => new UserNotifications
                    {
                        NotificationId = notification.Id,
                        UserId = userId
                    }).ToList();

                    await _dbContext.UserNotifications.AddRangeAsync(userNotifications);
                }

                if (model.ListFile != null && model.ListFile.Any())
                {
                    var notificationFiles = model.ListFile.Select(fileId => new NotificationFiles
                    {
                        NotificationId = notification.Id,
                        FileId = fileId
                    }).ToList();

                    await _dbContext.NotificationFiles.AddRangeAsync(notificationFiles);
                }

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message);
            }
        }

        public async Task ChangeStatus(int id)
        {
            var notiEntity = await _notifications.FindAsync(id);

            if (notiEntity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            notiEntity.IsActive = !notiEntity.IsActive;

            var success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }

        public async Task ChangeRead(NotificationsUpdateReadVModel model)
        {
            var entity = await _userNotifications.FindAsync(model.Id);

            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            entity.IsRead = !entity.IsRead;

            var success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }

        public async Task ChangeStatusForUser()
        {
            if (GlobalUserId == "" || GlobalUserId == null)
            {
                throw new ForbiddenException("Vui lòng đăng nhập!");
            }

            var entity = await _userNotifications.FindAsync(GlobalUserId);

            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            entity.IsActive = !entity.IsActive;

            var success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }

        public async Task ChangeAllRead()
        {
            if (GlobalUserId == "" || GlobalUserId == null)
            {
                throw new ForbiddenException("Vui lòng đăng nhập!");
            }

            var user = await _userManager.FindByIdAsync(GlobalUserId);
            if (user == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            var notiList = await _userNotifications
                .Where(x => x.IsRead == false && GlobalUserId == x.UserId)
                .ToListAsync();

            if (!notiList.Any())
            {
                return;
            }

            notiList.ForEach(x => x.IsRead = true);

            var success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }

        public async Task Remove(int id)
        {
            var notiEntity = await _notifications.FindAsync(id);

            if (notiEntity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            _notifications.Remove(notiEntity);

            var success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, _nameService));
            }
        }

        public async Task<ResponseResult> SearchForUser(FilterNotificationsForUserVModel model)
        {
            var result = new ResponseResult();

            if (GlobalUserId == "" || GlobalUserId == null)
            {
                throw new ForbiddenException("Vui lòng đăng nhập!");
            }

            var query = from un in _userNotifications.AsNoTracking()
                        join n in _dbContext.Notifications.AsNoTracking()
                            on un.NotificationId equals n.Id
                        where un.UserId == GlobalUserId &&
                              (model.IsRead == null || model.IsRead == un.IsRead) &&
                              un.IsActive == model.IsActive
                        select new NotificationsGetAllForUserVModel
                        {
                            Id = un.Id,
                            UserId = un.UserId,
                            Title = n.Title,
                            Content = n.Content,
                            SentTime = n.SentTime,
                            Type = n.Type,
                            IsRead = un.IsRead,
                            NotificationId = n.Id,
                        };

            int pageSize = 100;
            int pageNumber = 1;

            var totalRecords = await query.CountAsync();

            var list = await query
                .OrderByDescending(x => x.SentTime)
                .ThenByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            result.Data = new Pagination
            {
                Records = list,
                TotalRecords = totalRecords
            };

            return result;
        }
    }
}
