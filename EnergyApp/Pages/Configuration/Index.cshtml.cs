using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;
using Microsoft.AspNetCore.Authorization;

namespace EnergyApp.Pages_Configuration
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public IndexModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Configuration> Configuration { get;set; }

        public async Task OnGetAsync()
        {
            Configuration = await _context.Configurations.ToListAsync();
        }
    }
}
