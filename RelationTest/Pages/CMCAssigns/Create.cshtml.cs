using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EnergyApp.Data;

namespace RelationTest.Pages_CMCAssigns
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
        ViewData["ChargerID"] = new SelectList(_context.Charger, "ID", "ID");
        ViewData["CustomerID"] = new SelectList(_context.Customers, "ID", "ID");
        ViewData["MeterID"] = new SelectList(_context.Meter, "ID", "ID");
            return Page();
        }

        [BindProperty]
        public CMCAssign CMCAssign { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.CMCAssign.Add(CMCAssign);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}