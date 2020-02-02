using EShopping.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EShopping.Models.Data;

namespace EShopping.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pageslist;
            using (EShoppingDb db = new EShoppingDb())
            {
                //init list
                pageslist = db.pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
                return View(pageslist);
        }

        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            if ( ! ModelState.IsValid )
            {
                return View(model);
            }
            using (EShoppingDb db = new EShoppingDb())
            {
                string slug;

                PageDTO dto = new PageDTO();
                dto.Title = model.Title;

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                if (db.pages.Any(p => p.Title == model.Title) || db.pages.Any(p => p.Slug == model.Slug))
                {
                    ModelState.AddModelError("","this page(title or slug) already exists");
                    return View(model);
                }

                //dto the rest 
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = model.Sorting;

                db.pages.Add(dto);
                db.SaveChanges();
            }

            TempData["Success_message"] = "you have addedd a new page ";
            return RedirectToAction("AddPage");

        }


        [HttpGet]
        public ActionResult EditPage(int id )
        {
            PageVM model;

            using (EShoppingDb db = new EShoppingDb())
            {
                var dto = db.pages.Find(id);
                if (dto == null)
                {
                    return Content("page is not exists ");
                }

                model = new PageVM(dto);


            }
            
            return View(model);
                
        }

        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {

         if ( ! ModelState.IsValid )
            {
                return View(model);
            }

           
            using (EShoppingDb db = new EShoppingDb())
            {
                int id = model.Id;
                string slug="home";

                PageDTO dto = db.pages.Find(id);
                dto.Title = model.Title;

                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }

                }
                if (db.pages.Where(p=>p.Id !=model.Id).Any(p => p.Title == model.Title) || db.pages.Where(p => p.Id != model.Id).Any(p => p.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "this page(title or slug) already exists");
                    return View(model);
                }

                //dto the rest 
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = model.Sorting;

                db.SaveChanges();

                TempData["Edited_message"] = "page edited successfully !";
            }

            return RedirectToAction("EditPage");
        }


        [HttpGet]
        public ActionResult PagesDetails(int id )
        {
            PageVM model;
            using (EShoppingDb db = new EShoppingDb())
            {
                var dto = db.pages.Find(id);
                if (dto == null)
                {
                    return Content("page is not exists ");
                }

                model = new PageVM(dto);


            }

            return View(model);
        }

        public ActionResult DeletePages(int id)
        {
            using (EShoppingDb db = new EShoppingDb())
            {
             
               PageDTO dto = db.pages.Find(id);
                db.pages.Remove(dto);

                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public void ReorderPages(int [] id)
        {
            using (EShoppingDb db = new EShoppingDb())
            {
                int count = 1;
                PageDTO dto;
                foreach (var pageId in id)
                {
                    dto = db.pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }



        [HttpGet]
        public ActionResult EditSidebar()
        {
            SidebarVM model;
            using (EShoppingDb db = new EShoppingDb())
            {
                SidebarDTO dto = db.Sidebar.Find(1);
                model = new SidebarVM(dto);

            }
                return View(model);
        }

        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (EShoppingDb db = new EShoppingDb())
            {
                SidebarDTO dto = db.Sidebar.Find(1);

                dto.Body = model.Body;
                db.SaveChanges();

               
            }
            TempData["Edited_message"] = "Sidebar edited successfully !";
            return RedirectToAction("EditSidebar");
        }
    }
}