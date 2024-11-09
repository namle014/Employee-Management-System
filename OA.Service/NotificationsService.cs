using AngleSharp.Dom;
using Aspose.Pdf;
using Aspose.Pdf.Operators;
using AutoMapper;
using Ganss.Xss;
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
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace OA.Service
{
    public class NotificationsService : GlobalVariables, INotificationsService
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<Notifications> _notifications;
        private DbSet<UserNotifications> _userNotifications;
        private DbSet<NotificationFiles> _notificationFiles;
        private DbSet<SysFile> _sysFileRepo;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
        string _nameService = "Notifications";
        private readonly HtmlSanitizer _sanitizer;

        public NotificationsService(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, HtmlSanitizer sanitizer,
                                    IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _notifications = dbContext.Set<Notifications>();
            _userNotifications = dbContext.Set<UserNotifications>();
            _notificationFiles = dbContext.Set<NotificationFiles>();
            _sysFileRepo = dbContext.Set<SysFile>();
            _sanitizer = sanitizer;
            _userManager = userManager;
            _mapper = mapper;


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

        public async Task<ResponseResult> Search(FilterNotificationsVModel model)
        {
            var result = new ResponseResult();

            var query = _notifications.AsNoTracking()
                .Where(x => (x.IsActive == model.IsActive) &&
                            (string.IsNullOrEmpty(model.Type) || x.Type == model.Type) &&
                            (string.IsNullOrEmpty(model.Title) || x.Title.ToLowerInvariant().Contains(model.Title.ToLowerInvariant())) &&
                            (model.SentDate == null || (model.SentDate.Value.Day == x.SentTime.Day && model.SentDate.Value.Month == x.SentTime.Month && model.SentDate.Value.Year == x.SentTime.Year)));
            int pageSize = model.PageSize > 0 ? model.PageSize : 100;
            int pageNumber = model.PageNumber > 0 ? model.PageNumber : 1;

            var totalRecords = await query.CountAsync();

            var list = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => _mapper.Map<NotificationsGetAllVModel>(x))
                .ToListAsync();

            result.Data = new Pagination();
            result.Data.Records = list;
            result.Data.TotalRecords = totalRecords;

            return result;
        }

        public async Task<ResponseResult> SearchForUser(FilterNotificationsForUserVModel model)
        {
            var result = new ResponseResult();

            var query = from un in _userNotifications.AsNoTracking()
                        join n in _dbContext.Notifications.AsNoTracking()
                            on un.NotificationId equals n.Id
                        where un.UserId == model.UserId &&
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

            int pageSize = model.PageSize > 0 ? model.PageSize : 100;
            int pageNumber = model.PageNumber > 0 ? model.PageNumber : 1;

            var totalRecords = await query.CountAsync();

            var list = await query
                .OrderByDescending(x => x.SentTime)
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
                _notifications.Add(entityCreate);

                var success = await _dbContext.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
                }

                var notificationId = entityCreate.Id;

                if (model.ListUser != null)
                {
                    var listUserNoti = model.ListUser.Select(id => new UserNotifications
                    {
                        NotificationId = notificationId,
                        UserId = id
                    }).ToList();

                    await _userNotifications.AddRangeAsync(listUserNoti);
                }

                if (model.ListFile != null)
                {
                    var listNotificationFiles = model.ListFile.Select(id => new NotificationFiles
                    {
                        NotificationId = notificationId,
                        FileId = id
                    }).ToList();

                    await _notificationFiles.AddRangeAsync(listNotificationFiles);
                }

                success = await _dbContext.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message);
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

                var user = await _userManager.FindByIdAsync(notification.UserId);
                if (user == null)
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, "User update"));
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

                var success = await _dbContext.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
                }

                if (model.ListUser != null)
                {
                    var existingUserNoti = _userNotifications.Where(un => un.NotificationId == model.Id);
                    _userNotifications.RemoveRange(existingUserNoti);

                    var listUserNoti = model.ListUser.Select(id => new UserNotifications
                    {
                        NotificationId = model.Id,
                        UserId = id
                    }).ToList();

                    await _userNotifications.AddRangeAsync(listUserNoti);
                }

                if (model.ListFile != null)
                {
                    var existingNotificationFiles = _notificationFiles.Where(nf => nf.NotificationId == model.Id);
                    _notificationFiles.RemoveRange(existingNotificationFiles);

                    var listNotificationFiles = model.ListFile.Select(id => new NotificationFiles
                    {
                        NotificationId = model.Id,
                        FileId = id
                    }).ToList();

                    await _notificationFiles.AddRangeAsync(listNotificationFiles);
                }

                success = await _dbContext.SaveChangesAsync() > 0;
                if (!success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
                }

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

        public async Task ChangeStatusForUser(NotificationsUpdateReadVModel model)
        {
            var entity = await _userNotifications.FindAsync(model.Id);

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

        public async Task ChangeAllRead(NotificationsUpdateAllReadVModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            var notiList = await _userNotifications
                .Where(x => x.IsRead == false && model.UserId == x.UserId)
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
    }
}
