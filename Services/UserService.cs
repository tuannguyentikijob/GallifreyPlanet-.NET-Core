﻿using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.AccountManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly BlogService _blogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GallifreyPlanetContext _context;

        public UserService(
            UserManager<User> userManager,
            BlogService blogService,
            IHttpContextAccessor httpContextAccessor,
            GallifreyPlanetContext context
        )
        {
            _userManager = userManager;
            _blogService = blogService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
        }

        public async Task<User?> GetUserAsyncByUserName(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<List<User>?> GetUsersAsync()
        {
            return await _context.User.ToListAsync();
        }

        public async Task<User?> GetUserAsyncById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<List<ActiveSessionViewModel>> GetActiveSessionsAsyncByUser(string userId)
        {
            return await _context.UserSession
                .Where(us => us.UserId == userId && us.LogoutTime == null)
                .Select(us => new ActiveSessionViewModel
                {
                    Id = us.Id.ToString(),
                    DeviceName = us.DeviceName,
                    Location = us.Location,
                    LoginTime = us.LoginTime
                })
                .ToListAsync();
        }

        public async Task<List<LoginHistory>> GetLoginHistoriesAsyncByUserId(string userId)
        {
            return await _context.LoginHistory
                .Where(lh => lh.UserId == userId)
                .OrderByDescending(lh => lh.LoginTime)
                .Take(count: 10)
                .ToListAsync();
        }

        public async Task<IdentityResult> UpdateProfileAsync(User user, EditProfileViewModel model)
        {
            user.Name = model.Name;
            user.Email = model.Email;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdatePrivacySettingsAsync(User user, PrivacySettingsViewModel model)
        {
            user.ShowEmail = model.ShowEmail;
            user.AllowChat = model.AllowMessagesFromNonFriends;
            user.AllowAddFriend = model.AllowAddFriend;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateNotificationSettingsAsync(User user, NotificationSettingsViewModel model)
        {
            user.EmailNotifications = model.EmailNotifications;
            user.PushNotifications = model.PushNotifications;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<string> GenerateTwoFactorTokenAsync(User user)
        {
            return await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider: "Authenticator");
        }

        public async Task<bool> VerifyTwoFactorTokenAsync(User user, string verificationCode)
        {
            return await _userManager.VerifyTwoFactorTokenAsync(user, tokenProvider: "Authenticator", verificationCode);
        }

        public async Task<IdentityResult> SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            return await _userManager.SetTwoFactorEnabledAsync(user, enabled);
        }

        public async Task TerminateSessionAsync(string userId, string sessionId)
        {
            UserSession? session = await _context.UserSession
                .FirstOrDefaultAsync(us => us.Id.ToString() == sessionId && us.UserId == userId);

            if (session != null)
            {
                session.LogoutTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddLoginHistoryAsync(string userId, string ipAddress, string userAgent)
        {
            LoginHistory? loginHistory = new LoginHistory
            {
                UserId = userId,
                LoginTime = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            _context.LoginHistory.Add(loginHistory);
            await _context.SaveChangesAsync();
        }
    }
}
