using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace EnergyApp.Pages_Configuration
{
    public class DeleteModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public DeleteModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Configuration Configuration { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Configuration = await _context.Configurations.FirstOrDefaultAsync(m => m.ConfigurationID == id);

            if (Configuration == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Configuration = await _context.Configurations.FindAsync(id);

            if (Configuration != null)
            {
                _context.Configurations.Remove(Configuration);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
