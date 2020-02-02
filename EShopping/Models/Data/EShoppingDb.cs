using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EShopping.Models.Data
{
    public class EShoppingDb:DbContext
    {
        public DbSet<PageDTO>pages { get; set; }
        public DbSet<SidebarDTO>Sidebar { get; set; }
        public DbSet<CategoryDTO> Categories { get; set; }
    }
}