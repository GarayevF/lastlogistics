using Logistics.ViewModels;
using ApplianceRepair.Extensions;
using Logistics.DataAccessLayer;
using Logistics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ApplianceRepair.Helpers;
using NuGet.Packaging.Signing;

namespace Logistics.Areas.Admin.Controllers
{
    [Area("Admin")]
	//[Authorize(Roles = "SuperAdmin")]
	public class ServicesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public ServicesController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        #region Index
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Service> services = _db.Services.Where(x => !x.IsDeleted && x.Language.Culture == "az-Latn-AZ")
                .Include(a => a.ServiceServiceSections)
                .ThenInclude(s => s.ServiceSection);

            return View(PageNatedList<Service>.Create(services, pageIndex, 5, 5));
        }
        #endregion

        #region Detail
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Service service = await _db.Services.Include(a=>a.ServiceServiceSections).ThenInclude(s=>s.ServiceSection).SingleOrDefaultAsync(x => x.Id == id);
            if (service == null)
            {
                return BadRequest();
            }
            return View(service);
        }
        #endregion

        #region Create
        public async Task<IActionResult> Create()
        {
            ViewBag.ServicesSection = await _db.ServiceSections.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Languages = await _db.Languages.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<Service> services)
        {
            ViewBag.ServicesSection = await _db.ServiceSections.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Languages = await _db.Languages.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }

            #region Icon
            if (services[0].IconPhoto != null)
            {
                if (!(services[0].IconPhoto.CheckFileContenttype("image/jpeg") || services[0].IconPhoto.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("IconPhoto", $"{services[0].IconPhoto.FileName} adli fayl novu duzgun deyil");
                    return View(services);
                }

                if (services[0].IconPhoto.CheckFileLength(10240))
                {
                    ModelState.AddModelError("IconPhoto", $"{services[0].IconPhoto.FileName} adli fayl hecmi coxdur");
                    return View(services);
                }

                services[0].Icon = await services[0].IconPhoto.CreateFileAsync(_env, "assets", "img", "services");
            }
            else
            {
                ModelState.AddModelError("IconPhoto", "Image is empty");
                return View(services);
            }
            #endregion

            #region Photo 1
            if (services[0].Photo_1 != null)
            {
                if (!(services[0].Photo_1.CheckFileContenttype("image/jpeg") || services[0].Photo_1.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_1", $"{services[0].Photo_1.FileName} adli fayl novu duzgun deyil");
                    return View(services);
                }

                if (services[0].Photo_1.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo_1", $"{services[0].Photo_1.FileName} adli fayl hecmi coxdur");
                    return View(services);
                }

                services[0].Image_1 = await services[0].Photo_1.CreateFileAsync(_env, "assets", "img", "services");
            }
            else
            {
                ModelState.AddModelError("Photo_1", "Image is empty");
                return View(services);
            }
            #endregion

            #region Photo 2
            if (services[0].Photo_2 != null)
            {
                if (!(services[0].Photo_2.CheckFileContenttype("image/jpeg") || services[0].Photo_2.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_2", $"{services[0].Photo_2.FileName} adli fayl novu duzgun deyil");
                    return View(services);
                }

                if (services[0].Photo_2.CheckFileLength(15360))
                {
                    ModelState.AddModelError("Photo_2", $"{services[0].Photo_2.FileName} adli fayl hecmi coxdur");
                    return View(services);
                }

                services[0].Image_2 = await services[0].Photo_2.CreateFileAsync(_env, "assets", "img", "services");
            }
            else
            {
                ModelState.AddModelError("Photo_2", "Image is empty");
                return View(services);
            }
            #endregion

            #region Photo 3
            if (services[0].Photo_3 != null)
            {
                if (!(services[0].Photo_3.CheckFileContenttype("image/jpeg") || services[0].Photo_3.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_3", $"{services[0].Photo_3.FileName} adli fayl novu duzgun deyil");
                    return View(services);
                }

                if (services[0].Photo_3.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo_3", $"{services[0].Photo_3.FileName} adli fayl hecmi coxdur");
                    return View(services);
                }

                services[0].Image_3 = await services[0].Photo_3.CreateFileAsync(_env, "assets", "img", "services");
            }
            else
            {
                ModelState.AddModelError("Photo_3", "Image is empty");
                return View(services);
            }
            #endregion

            #region Service Section Relation
            Service test = await _db.Services.OrderByDescending(a => a.Id).FirstOrDefaultAsync();
            foreach (Service service in services)
            {
                service.Image_1 = services[0].Image_1;
                service.Image_2 = services[0].Image_2;
                service.Image_3 = services[0].Image_3;
                service.Icon = services[0].Icon;
                service.LanguageGroup = test != null ? test.LanguageGroup + 1 : 1;

                List<ServiceServiceSection> serviceServiceSections = new List<ServiceServiceSection>();
                foreach (int serviceSectionId in service.ServiceSectionIds)
                {
                    ServiceServiceSection serviceServiceSection = new ServiceServiceSection()
                    {
                        ServiceId = service.Id,
                        ServiceSectionId = serviceSectionId,
                    };

                    serviceServiceSections.Add(serviceServiceSection);

                }
                service.ServiceServiceSections = serviceServiceSections;
                #endregion

                service.CreatedAt = DateTime.UtcNow.AddHours(4);

                await _db.Services.AddAsync(service);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Update
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null) return BadRequest();

            ViewBag.ServicesSection = await _db.ServiceSections.Where(x => x.IsDeleted == false).ToListAsync();
            List<Language> languages = await _db.Languages.ToListAsync();
            ViewBag.Languages = languages;

            Service? firstService = await _db.Services.Include(a => a.ServiceServiceSections)
                .ThenInclude(s => s.ServiceSection).FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstService == null) return NotFound();

            List<Service> services = await _db.Services.Include(a => a.ServiceServiceSections)
                .ThenInclude(s => s.ServiceSection).Where(c => c.LanguageGroup == firstService.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (Service service in services)
            {
                if (service == null) return NotFound();

                service.ServiceSectionIds = service.ServiceServiceSections?.Select(s => s.ServiceSectionId);
            }            

            return View(services);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, List<Service> services)
        {
            ViewBag.ServicesSection = await _db.ServiceSections.Where(x => x.IsDeleted == false).ToListAsync();
            ViewBag.Languages = await _db.Languages.ToListAsync();

            if (!ModelState.IsValid) return View(services);

            if (id == null) return BadRequest();

            Service? firstService = await _db.Services.Include(a => a.ServiceServiceSections)
            .ThenInclude(s => s.ServiceSection).FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            //foreach (Service service in services)
            //{
            //    if (firstService.LanguageGroup != service.LanguageGroup) return BadRequest();
            //}

            List<Service> dbServices = new List<Service>();

            if (firstService == null) return NotFound();

            dbServices = await _db.Services.Include(a => a.ServiceServiceSections)
                .ThenInclude(s => s.ServiceSection).Where(c => c.LanguageGroup == firstService.LanguageGroup && c.IsDeleted == false).ToListAsync();

            if(dbServices == null || dbServices.Count == 0) return NotFound();

            foreach (Service service in dbServices)
            {
                if (service == null) return NotFound();

                service.ServiceSectionIds = service.ServiceServiceSections?.Select(s => s.ServiceSectionId);
            }

            if (!ModelState.IsValid)
            {
                return View(services);
            }     

            #region Icon
            if (services[0].IconPhoto != null)
            {
                if (!(services[0].IconPhoto.CheckFileContenttype("image/jpeg") || services[0].IconPhoto.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("IconPhoto", $"{services[0].IconPhoto.FileName} adli fayl novu duzgun deyil");
                    return View(services[0]);
                }

                if (services[0].IconPhoto.CheckFileLength(10240))
                {
                    ModelState.AddModelError("IconPhoto", $"{services[0].IconPhoto.FileName} adli fayl hecmi coxdur");
                    return View(services[0]);
                }
                FileHelper.DeleteFile(dbServices[0].Icon, _env, "assets", "img", "services");
                foreach (Service dbService in dbServices)
                {
                    dbService.Icon = await services[0].IconPhoto.CreateFileAsync(_env, "assets", "img", "services");
                }
            }
            #endregion

            #region Photo 1
            if (services[0].Photo_1 != null)
            {
                if (!(services[0].Photo_1.CheckFileContenttype("image/jpeg") || services[0].Photo_1.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_1", $"{services[0].Photo_1.FileName} adli fayl novu duzgun deyil");
                    return View(services[0]);
                }

                if (services[0].Photo_1.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo_1", $"{services[0].Photo_1.FileName} adli fayl hecmi coxdur");
                    return View(services[0]);
                }
                FileHelper.DeleteFile(dbServices[0].Icon, _env, "assets", "img", "services");
                foreach (Service dbService in dbServices)
                {
                    dbService.Image_1 = await services[0].Photo_1.CreateFileAsync(_env, "assets", "img", "services");
                }
            }
            #endregion

            #region Photo 2
            if (services[0].Photo_2 != null)
            {
                if (!(services[0].Photo_2.CheckFileContenttype("image/jpeg") || services[0].Photo_2.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_2", $"{services[0].Photo_2.FileName} adli fayl novu duzgun deyil");
                    return View(services[0]);
                }

                if (services[0].Photo_2.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo_2", $"{services[0].Photo_2.FileName} adli fayl hecmi coxdur");
                    return View(services[0]);
                }
                FileHelper.DeleteFile(dbServices[0].Icon, _env, "assets", "img", "services");
                foreach (Service dbService in dbServices)
                {
                    dbService.Image_2 = await services[0].Photo_2.CreateFileAsync(_env, "assets", "img", "services");
                }
            }
            #endregion

            #region Photo 3
            if (services[0].Photo_3 != null)
            {
                if (!(services[0].Photo_3.CheckFileContenttype("image/jpeg") || services[0].Photo_3.CheckFileContenttype("image/png")))
                {
                    ModelState.AddModelError("Photo_3", $"{services[0].Photo_3.FileName} adli fayl novu duzgun deyil");
                    return View(services[0]);
                }

                if (services[0].Photo_3.CheckFileLength(10240))
                {
                    ModelState.AddModelError("Photo_3", $"{services[0].Photo_3.FileName} adli fayl hecmi coxdur");
                    return View(services[0]);
                }
                FileHelper.DeleteFile(dbServices[0].Icon, _env, "assets", "img", "services");
                foreach (Service dbService in dbServices)
                {
                    dbService.Image_3 = await services[0].Photo_3.CreateFileAsync(_env, "assets", "img", "services");
                }
            }
            #endregion

            foreach (Service dbService in dbServices)
            {
                _db.ServiceServiceSections.RemoveRange(dbService.ServiceServiceSections);
            }


            foreach (Service service in services)
            {
                List<ServiceServiceSection>? serviceServiceSections = new List<ServiceServiceSection>();
                foreach (int serviceSectionId in service.ServiceSectionIds)
                {
                    ServiceServiceSection serviceServiceSection = new ServiceServiceSection()
                    {
                        ServiceId = service.Id,
                        ServiceSectionId = serviceSectionId
                    };

                    serviceServiceSections.Add(serviceServiceSection);

                }

                Service dbService = dbServices.FirstOrDefault(s => s.LanguageId == service.LanguageId);

                dbService?.ServiceServiceSections?.AddRange(serviceServiceSections);
                dbService.Title = service.Title.Trim();
                dbService.Description = service.Description.Trim();
                dbService.Content = service.Content.Trim();
                dbService.SectionDescription = service.SectionDescription.Trim();
            }

            //foreach (Service dbService in dbServices)
            //{
            //    Service service = _db.Services.Include(a => a.ServiceServiceSections)
            //    .ThenInclude(s => s.ServiceSection).FirstOrDefault(s => s.LanguageGroup == dbService.LanguageGroup && s.LanguageId ==  dbService.LanguageId && s.IsDeleted == false);

            //    dbService.ServiceServiceSections = service.ServiceServiceSections;
            //    dbService.Title = service.Title.Trim();
            //    dbService.Description = service.Description.Trim();
            //    dbService.Content = service.Content.Trim();
            //    dbService.SectionDescription = service.SectionDescription.Trim();
            //}

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null) return BadRequest();

            List<Language> languages = await _db.Languages.ToListAsync();
            ViewBag.Languages = languages;

            Service? firstService = await _db.Services.Include(a => a.ServiceServiceSections)
                .ThenInclude(s => s.ServiceSection).FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (firstService == null) return NotFound();

            List<Service> services = await _db.Services.Include(a => a.ServiceServiceSections)
                .ThenInclude(s => s.ServiceSection).Where(c => c.LanguageGroup == firstService.LanguageGroup && c.IsDeleted == false).ToListAsync();

            foreach (Service service in services)
            {
                if (service == null) return NotFound();

                service.IsDeleted = true;
                service.DeletedBy = "system";
                service.DeletedAt = DateTime.UtcNow.AddHours(4);

            }
            
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        #endregion
    }
}
