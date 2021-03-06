using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EnergyApp.Data;

namespace RelationTest.Pages_Meters
{
    public class CreateModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public CreateModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Meter Meter { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Meter.Add(Meter);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}