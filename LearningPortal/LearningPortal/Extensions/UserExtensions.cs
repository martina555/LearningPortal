using LearningPortal.Data;
using LearningPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Extensions
{
    public class UserExtensions : IUserExtensions
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserExtensions(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }



        public async Task<IActionResult> CreateRoleAddToUser(string role, ApplicationUser user)
        {
            IdentityRole roleExists = await _roleManager.FindByNameAsync(role);
            if (roleExists == null)
            {
                roleExists = new IdentityRole { Name = role, NormalizedName = role };
                await _roleManager.CreateAsync(roleExists);
            }
            if (!await _userManager.IsInRoleAsync(user, roleExists.Name))
            {
                await _userManager.AddToRoleAsync(user, roleExists.Name);
                await _signInManager.RefreshSignInAsync(user);
            }
            return null;
        }

    }
}
