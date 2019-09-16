using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EnergyApp.Data;

namespace EnergyApp.Pages_Partners
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
        public Partner Partner { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Partners.Add(Partner);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}