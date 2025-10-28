using Microsoft.EntityFrameworkCore;
using hehehe.Models;

namespace hehehe.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<UserForm> UserForms { get; set; }
        
        public DbSet<InitUserForm> InitUserForm { get; set; }

        public DbSet<YeuCauDinhChinh> YeuCauDinhChinh { get; set; }
        
        public DbSet<UserStdInfo> UserStdInfos { get; set; }
        
        public DbSet<User_UploadFile> UserUploadFiles { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>()
                .HasOne(i => i.InitUserForm)
                .WithOne(u => u.User)
                .HasForeignKey<InitUserForm>(i => i.MaNhapHoc)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<UserForm>()
                .HasOne(u => u.InitUserForm)
                .WithOne(i => i.UserForm)
                .HasForeignKey<UserForm>(u => u.MaNhapHoc)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserStdInfo)
                .WithOne(i => i.User)
                .HasForeignKey<UserStdInfo>(i => i.MaNhapHoc)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<User_UploadFile>()
                .HasKey(x => new { x.MaNhapHoc, x.FileType }); // Composite Key
            
            modelBuilder.Entity<User_UploadFile>()
                .HasOne(x => x.User)
                .WithMany() // hoặc WithMany(u => u.UploadFiles) nếu bạn có navigation ngược
                .HasForeignKey(x => x.MaNhapHoc);
        }
        
    }
}