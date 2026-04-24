using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DemoMvc.Models
{
    public class RolesIndexViewModel
    {
        public IEnumerable<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)System.Math.Ceiling((double)TotalCount / PageSize);
    }
}
