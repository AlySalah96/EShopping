using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EShopping.Models.ViewModels.Shop;
using EShopping.Models.Data;


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

    }
}