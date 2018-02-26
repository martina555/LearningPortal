using LearningPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LearningPortal.Extensions
{
    public interface IUserExtensions
    {
        Task<IActionResult> CreateRoleAddToUser(string role, ApplicationUser user);

    }
}
