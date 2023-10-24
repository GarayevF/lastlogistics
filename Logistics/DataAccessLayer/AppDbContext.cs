using Logistics.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Logistics.DataAccessLayer
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<About> Abouts { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<ContainerType> ContainerTypes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Incoterms> Incoterms { get; set; }
        public DbSet<IncotermsSection> IncotermsSections { get; set; }
        public DbSet<NewsContent> NewsContents { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<PriorityCard> PriorityCards { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceSection> ServiceSections { get; set; }
        public DbSet<ServiceServiceSection> ServiceServiceSections { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Statistics> Statistics { get; set; }
        public DbSet<Color> Colors { get; set; }

        //For Language 
        public DbSet<Language> Languages { get; set; }
        public DbSet<StringResource> StringResources { get; set; }
    }


}
