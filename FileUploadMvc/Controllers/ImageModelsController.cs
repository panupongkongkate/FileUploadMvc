using FileUploadMvc.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploadMvc.Controllers
{
    public class ImageModelsController : Controller
    {
        private readonly ImageDbCondext _context;

        public IWebHostEnvironment _HostEnvironment { get; }

        public ImageModelsController(ImageDbCondext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _HostEnvironment = hostEnvironment;
        }

        // GET: ImageModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Images.ToListAsync());
        }

        // GET: ImageModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imageModel = await _context.Images
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (imageModel == null)
            {
                return NotFound();
            }

            return View(imageModel);
        }

        // GET: ImageModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ImageModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImageID,title,ImageFile")] ImageModel imageModel)
        {
            if (ModelState.IsValid)
            {
                //save image to wwwroot folder
                string wwwRootPath = _HostEnvironment.WebRootPath;

                string Filename = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);

                string extension = Path.GetExtension(imageModel.ImageFile.FileName);

                //DateTime.Now ใช้เพราะ จะได้ไม่ซ้ำ
                Filename = Filename + DateTime.Now.ToString("yymmssfff") + extension;

                imageModel.ImageName = Filename;

                string path = Path.Combine(wwwRootPath + "/Images/" + Filename);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await imageModel.ImageFile.CopyToAsync(fileStream);
                }

                //insert record
                _context.Add(imageModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(imageModel);
        }

        // GET: ImageModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imageModel = await _context.Images.FindAsync(id);
            if (imageModel == null)
            {
                return NotFound();
            }
            return View(imageModel);
        }

        // POST: ImageModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImageID,title,ImageName,ImageFile")] ImageModel imageModel)
        {
            if (id != imageModel.ImageID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageModel.ImageFile != null)
                    {
                        //delete image form wwwroot/image
                        var ImagePath = Path.Combine(_HostEnvironment.WebRootPath, "Images", imageModel.ImageName);

                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);

                            //save image to wwwroot folder
                            string wwwRootPath = _HostEnvironment.WebRootPath;

                            string Filename = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);

                            string extension = Path.GetExtension(imageModel.ImageFile.FileName);

                            Filename = Filename + DateTime.Now.ToString("yymmssfff") + extension;

                            imageModel.ImageName = Filename;

                            string path = Path.Combine(wwwRootPath + "/Images/" + Filename);

                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await imageModel.ImageFile.CopyToAsync(fileStream);
                            }
                        }
                    }
                    //update record
                    _context.Update(imageModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageModelExists(imageModel.ImageID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(imageModel);
        }

        // GET: ImageModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imageModel = await _context.Images
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (imageModel == null)
            {
                return NotFound();
            }

            return View(imageModel);
        }

        // POST: ImageModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var imageModel = await _context.Images.FindAsync(id);

                //delete image form wwwroot/image
                var ImagePath = Path.Combine(_HostEnvironment.WebRootPath, "Images", imageModel.ImageName);

                if (System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                }
                //delete image in SQL Sever
                _context.Images.Remove(imageModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool ImageModelExists(int id)
        {
            return _context.Images.Any(e => e.ImageID == id);
        }
    }
}