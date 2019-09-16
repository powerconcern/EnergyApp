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
    public class DeleteModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public DeleteModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Partner = await _context.Partners.FindAsync(id);

            if (Partner != null)
            {
                _context.Partners.Remove(Partner);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
