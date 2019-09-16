using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace EnergyApp.Pages_Partners
{
    public class IndexModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public IndexModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Partner> Partner { get;set; }

        public async Task OnGetAsync()
        {
            Partner = await _context.Partners.ToListAsync();
        }
    }
}
