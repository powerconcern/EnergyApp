using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace EnergyApp.Pages_Chargers
{
    public class DeleteModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public DeleteModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Charger Charger { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Charger = await _context.Chargers.FirstOrDefaultAsync(m => m.ID == id);

            if (Charger == null)
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

            Charger = await _context.Chargers.FindAsync(id);

            if (Charger != null)
            {
                _context.Chargers.Remove(Charger);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
