using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace EnergyApp.Pages_Partner
{
    public class DetailsModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public DetailsModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Partner Partner { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Partner = await _context.Partners.FirstOrDefaultAsync(m => m.ID == id);

            if (Partner == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
