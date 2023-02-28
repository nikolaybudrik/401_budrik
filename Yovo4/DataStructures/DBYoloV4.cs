using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YOLOv4MLNet.DataStructures;

namespace Lol.Yovo4.DataStructures
{
    internal class DBYoloV4
    {
        [Key]
        public int ImageID { get; set; }
        public string Path { get; set; }
        public float BBox0 { get; set; }
        public float BBox1 { get; set; }
        public float BBox2 { get; set; }
        public float BBox3 { get; set; }
        public string Label { get; set; }
        public float Confidence { get; set; }
    }
    class LibraryContext : DbContext
    {
        public DbSet<DBYoloV4> DBYoloV4s { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder o) => o.UseSqlite(@"Data Source = D:\Dev\Aplication\Lol\Lol\library.db");
    }
}
