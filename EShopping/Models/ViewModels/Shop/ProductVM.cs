using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EShopping.Models.Data;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EShopping.Models.ViewModels
{
    public class ProductVM
    {

        public ProductVM()
        {

        }
        public ProductVM(ProductDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Description = row.Description;
            Price = row.Price;
            CategoryId = row.CategoryId;
            CategoryName = row.CategoryName;
            ImageName = row.ImageName;
        }



        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem>Categories { get; set; }
        public IEnumerable<string>GalleryImages { get; set; }

    }
}