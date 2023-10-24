using ApplianceRepair.Extensions;
using ApplianceRepair.Helpers;
using Logistics.DataAccessLayer;
using Logistics.Models;
using Microsoft.AspNetCore.Authorization;
using Logistics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Areas.Admin.Controllers
{
    [Area("Admin")]
	//[Authorize(Roles = "SuperAdmin")]
	public class CustomersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public CustomersController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;

        }

        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Customer> query = _db.Customers.Where(x => !x.IsDeleted);
            return View(PageNatedList<Customer>.Create(query, pageIndex, 5, 5));
        }
        #endregion

        #region Create
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            #region AboutPhoto_1 Create
            if (customer.Photo != null)
            {
                if (!(customer.Photo.CheckFileContenttype("image/jpeg") || customer.Photo.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo", $"{customer.Photo.FileName} adli fayl novu duzgun deyil");
                    return View(customer);
                }

                if (customer.Photo.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo", $"{customer.Photo.FileName} adli fayl hecmi coxdur");
                    return View(customer);
                }

                customer.Image = await customer.Photo.CreateFileAsync(_env, "assets", "img", "customers");
            }
            else
            {
                ModelState.AddModelError("Photo", "Image is empty");
                return View(customer);
            }
            #endregion

            customer.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _db.Customers.AddAsync(customer);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            Customer dbCustomer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (dbCustomer == null)
            {
                return NotFound();
            }
            return View(dbCustomer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Customer customer)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Customer dbCustomer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (dbCustomer == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            #region Photo
            if (customer.Photo != null)
            {
                if (!(customer.Photo.CheckFileContenttype("image/jpeg") || customer.Photo.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo", $"{customer.Photo.FileName} adli fayl novu duzgun deyil");
                    return View(customer);
                }

                if (customer.Photo.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo", $"{customer.Photo.FileName} adli fayl hecmi coxdur");
                    return View(customer);
                }

                FileHelper.DeleteFile(dbCustomer.Image, _env, "assets", "img", "customers");
                dbCustomer.Image = await customer.Photo.CreateFileAsync(_env, "assets", "img", "customers");
            }
            #endregion


            dbCustomer.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region IsDeleted
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Customer customer = await _db.Customers
                .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);

            if (customer == null) return NotFound();

            customer.IsDeleted = true;
            customer.DeletedBy = "system";
            customer.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion
    }
}
