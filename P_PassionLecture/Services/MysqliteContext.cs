using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P_PassionLecture.Models;

namespace P_PassionLecture.Services
{
    public class MysqliteContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BookTag> BookTags { get; set; }

        private string _dbPath;

        public MysqliteContext(string dbPath)
        {
            _dbPath = dbPath;
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Filename={_dbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookTag>().HasKey(bt => new {bt.BookId, bt.TagId});

            modelBuilder.Entity<BookTag>().HasOne(bt => bt.Tag).WithMany(t => t.BookTags).HasForeignKey(bt => bt.TagId);
        }
    }
}
