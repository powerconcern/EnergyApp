using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace RelationTest.Pages_Customers
{
    public class DetailsModel : PageModel
    {
        private readonly EnergyApp.Data.ApplicationDbContext _context;

        public DetailsModel(EnergyApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Customer Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer = await _context.Customers.FirstOrDefaultAsync(m => m.ID == id);

            if (Customer == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
