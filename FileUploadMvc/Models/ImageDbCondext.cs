using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploadMvc.Models
{
    public class ImageDbCondext : DbContext 
    {
        public ImageDbCondext(DbContextOptions<ImageDbCondext> options) : base(options)
        {

        }
        public DbSet<ImageModel> Images { get; set; }
    }
}
