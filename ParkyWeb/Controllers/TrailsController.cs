using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly ITrailRepository _trRepo;

        public TrailsController(ITrailRepository trRepo , INationalParkRepository npRepo)
        {
            _trRepo = trRepo;
            _npRepo = npRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);

            TrailsVM objVM = new TrailsVM()
            {
                NationalParkList = npList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Trail = new Trail()
            };

            if (id == null)
            {
                // For Create/Insert
                return View(objVM);
            }


            objVM.Trail = await _trRepo.GetAsync(SD.TrailsAPIPath, id.GetValueOrDefault());
            if (objVM.Trail == null)
            {
                return NotFound();
            }

            // For Update
            return View(objVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM obj)
        {
            if (ModelState.IsValid)
            {

                // create or update the object 
                if (obj.Trail.Id == 0)
                {
                    await _trRepo.CreateAsync(SD.TrailsAPIPath, obj.Trail);
                }
                else
                {
                    await _trRepo.UpdateAsync(SD.TrailsAPIPath + obj.Trail.Id, obj.Trail);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);

                TrailsVM objVM = new TrailsVM()
                {
                    NationalParkList = npList.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                    Trail = obj.Trail
                };

                return View(objVM);
            }
        }

        public async Task<IActionResult> GetAllTrails()
        {
            return Json(new { data = await _trRepo.GetAllAsync(SD.TrailsAPIPath) });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _trRepo.DeleteAsync(SD.TrailsAPIPath, id);
            if (status)
            {
                return Json(new { success = true , message = $"Trail with id = {id} is Deleted successful" });
            }
            return Json(new { success = false, message = "There are an error while deleting this Trail" });

        }
    }

}
