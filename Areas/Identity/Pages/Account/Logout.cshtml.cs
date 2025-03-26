// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ClothesShopProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ClothesShopProject.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ClothesShopProjectUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ClothesShopProjectUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            HttpContext.Session.Clear();

            // Fix: Prevent redirecting to /Logout again
            if (string.IsNullOrEmpty(returnUrl) || returnUrl.Contains("Logout", StringComparison.OrdinalIgnoreCase))
            {
                returnUrl = Url.Content("~/"); // or "/Apparels/Index" if you want that
            }

            return LocalRedirect(returnUrl);
        }

    }
}
