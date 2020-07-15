using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _npRepo;

        public NationalParksController(INationalParkRepository npRepo)
        {
            _npRepo = npRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            // empty object
            NationalPark obj = new NationalPark();
            if (id == null)
            {
                // For Create/Insert
                return View(obj);
            }

            obj = await _npRepo.GetAsync(SD.NationalParkAPIPath, id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            // For Update
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark obj)
        {
            if (ModelState.IsValid)
            {
                // Get The image from file input
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0) // if choose one
                {
                    byte[] p1 = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    obj.Picture = p1;
                }
                else // if don't need to change
                {
                    var objFromDb = await _npRepo.GetAsync(SD.NationalParkAPIPath, obj.Id);
                    obj.Picture = objFromDb.Picture;
                }

                // create or update the object 
                if (obj.Id == 0)
                {
                    await _npRepo.CreateAsync(SD.NationalParkAPIPath, obj);
                }
                else
                {
                    await _npRepo.UpdateAsync(SD.NationalParkAPIPath + obj.Id, obj);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
        }

        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new { data = await _npRepo.GetAllAsync(SD.NationalParkAPIPath) });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _npRepo.DeleteAsync(SD.NationalParkAPIPath, id);
            if (status)
            {
                return Json(new { success = true , message = $"National park with id = {id} is Deleted successful" });
            }
            return Json(new { success = false, message = "There are an error while deleting this National Park" });

        }
    }

}
