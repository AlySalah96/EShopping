using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EShopping.Models.ViewModels.Shop;
using EShopping.Models.Data;
using EShopping.Models.ViewModels;
using System.IO;
using System.Web.Helpers;
using PagedList;

namespace EShopping.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            List<CategoryVM> CategoryVMList;
            using (EShoppingDb db = new EShoppingDb())
            {
                //init list
                CategoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            return View(CategoryVMList);
        }


        //post: admin/shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;
            using (EShoppingDb db = new EShoppingDb())
            {
                if (db.Categories.Any(c => c.Name == catName))
                    return "titletaken";

                CategoryDTO dto=new CategoryDTO();
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;
                db.Categories.Add(dto);
                db.SaveChanges();

                id = dto.Id.ToString();

            }
                return id;
        }


        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (EShoppingDb db = new EShoppingDb())
            {
                int count = 1;
                CategoryDTO dto;
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }


        public ActionResult DeleteCategory(int id)
        {
            using (EShoppingDb db = new EShoppingDb())
            {

                CategoryDTO dto = db.Categories.Find(id);
                db.Categories.Remove(dto);

                db.SaveChanges();

            }
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (EShoppingDb db = new EShoppingDb())
            {
                if (db.Categories.Any(c => c.Name == newCatName))
                    return "titletaken";
                CategoryDTO dto = db.Categories.Find(id);
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();
                db.SaveChanges();
            }
            return "ok";
        }


        [HttpGet]
        public ActionResult AddProduct()
        {
            ProductVM model = new ProductVM();
            using (EShoppingDb db = new EShoppingDb())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
                return View(model);
        }

        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                using (EShoppingDb db = new EShoppingDb())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
                    return View(model);
                }

            }


            // check if the produc name is unique
            using (EShoppingDb db = new EShoppingDb())
            {
                if (db.Products.Any(p => p.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
                    ModelState.AddModelError("", "the product name is taken !");
                    return View(model);
                }
            }


            //declare prod id 
            int id;
            using (EShoppingDb db = new EShoppingDb())
            {
                ProductDTO prod = new ProductDTO();
                prod.Name = model.Name;
                prod.Slug = model.Name.Replace(" ", "-").ToLower();
                prod.Description = model.Description;
                prod.Price = model.Price;
                prod.CategoryId = model.CategoryId;

                CategoryDTO cat = db.Categories.Find(Convert.ToInt32(prod.CategoryId));
                prod.CategoryName = cat.Name;

                db.Products.Add(prod);
                db.SaveChanges();

                //get id after put in DB
                id = prod.Id;

            }
            TempData["Success_message"] = "Product added successfully !";
             
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\"+id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() +"\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            //check the file is uploaded 
            if (file != null && file.ContentLength > 0)
            {
                string extension = file.ContentType.ToLower();
                if (extension != "image/jpg"
                   && extension != "image/jpeg"
                   && extension != "image/pjpeg"
                   && extension != "image/gif"
                   && extension != "image/x-png"
                   && extension != "image/png")
                {
                    using (EShoppingDb db = new EShoppingDb())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
                        ModelState.AddModelError("", "Image was not uploaded or wrong image formate !");
                        return View(model);

                    }


                }
                string imageName = file.FileName;

                using (EShoppingDb db = new EShoppingDb())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }
                //set original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path1 = string.Format("{0}\\{1}", pathString3, imageName);

                //save original
                file.SaveAs(path);

                //create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path1);
             


            }

            return RedirectToAction("AddProduct");
        }

        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            List<ProductVM> listOfProductVM;

            var pageNumber = page ?? 1; // if page =null put one .

            using (EShoppingDb db = new EShoppingDb())
            {
                listOfProductVM = db.Products.ToArray()
                    .Where(p => catId == null || catId == 0 || p.CategoryId == catId)
                    .Select(p => new ProductVM(p))
                    .ToList();

                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                ViewBag.SelecteCat = catId.ToString();
                var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber,3);

                ViewBag.onePageOfProducts = onePageOfProducts;
            }


                return View(listOfProductVM);
        }



    }
}