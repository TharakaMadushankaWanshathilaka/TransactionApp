using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TransactionApp.Models;

namespace TransactionApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly TransactionDbContext _context;

        public TransactionController(TransactionDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
              return _context.Transaction != null ? 
                          View(await _context.Transaction.ToListAsync()) :
                          Problem("Entity set 'TransactionDbContext.Transaction'  is null.");
        }

        // GET: Transaction/AddOrEdit
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {   if (id == 0)
                return View(new TransactionModel() );
            else
            {
                var transactionModel = await _context.Transaction.FindAsync(id);
                if (transactionModel == null)
                {
                    return NotFound();
                }
                return View(transactionModel);
            }
        }
        
        // POST: Transaction/AddOrEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("TransactionId,AccountNumber,BeneficiaryName,BankName,SWIFTCode,Amount,Date")] TransactionModel transactionModel)
        {
            
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    transactionModel.Date = DateTime.Now;
                    _context.Add(transactionModel);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        _context.Update(transactionModel);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TransactionModelExists(transactionModel.TransactionId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
             
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this,"_ViewAll",_context.Transaction.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", transactionModel) });
        }
                
        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transaction == null)
            {
                return Problem("Entity set 'TransactionDbContext.Transaction'  is null.");
            }
            var transactionModel = await _context.Transaction.FindAsync(id);
            if (transactionModel != null)
            {
                _context.Transaction.Remove(transactionModel);
            }
            
            await _context.SaveChangesAsync();
            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Transaction.ToList()) });
        }

        private bool TransactionModelExists(int id)
        {
          return (_context.Transaction?.Any(e => e.TransactionId == id)).GetValueOrDefault();
        }
    }
}
