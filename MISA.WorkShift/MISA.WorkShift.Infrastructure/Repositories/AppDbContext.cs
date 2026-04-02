using Microsoft.EntityFrameworkCore;
using MISA.WorkShift.Core.Entities;
using System.Text.RegularExpressions;

namespace MISA.WorkShift.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyUser> CompanyUsers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<InvoiceRegistration> InvoiceRegistrations { get; set; }
        public DbSet<InvoiceTemplate> InvoiceTemplates { get; set; }
        public DbSet<RegDigitalSignature> RegDigitalSignatures { get; set; }
        public DbSet<RegTVAN> RegTVANs { get; set; }
        public DbSet<RegInvoiceType> RegInvoiceTypes { get; set; }
        public DbSet<RegTransmissionUnit> RegTransmissionUnits { get; set; }
        public DbSet<RegDependentUnit> RegDependentUnits { get; set; }
        public DbSet<RegSuspension> RegSuspensions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Composite Key cho bảng CompanyUser
            modelBuilder.Entity<CompanyUser>()
                .HasKey(cu => new { cu.CompanyId, cu.UserId });

            // Cấu hình Primary Key cho các bảng có tên ID đặc thù
            modelBuilder.Entity<InvoiceDetail>().HasKey(e => e.DetailId);
            modelBuilder.Entity<InvoiceRegistration>().HasKey(e => e.RegistrationId);
            modelBuilder.Entity<InvoiceTemplate>().HasKey(e => e.TemplateId);
            modelBuilder.Entity<RegDigitalSignature>().HasKey(e => e.SignatureId);
            modelBuilder.Entity<RegTransmissionUnit>().HasKey(e => e.TransmissionUnitId);
            modelBuilder.Entity<RegDependentUnit>().HasKey(e => e.DependentUnitId);
            modelBuilder.Entity<RegSuspension>().HasKey(e => e.SuspensionId);

            // Tự động cấu hình snake_case cho tất cả Table và Column
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Convert Table Name
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                {
                    entity.SetTableName(ToSnakeCase(tableName));
                }

                // Convert Column Names
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }
            }
        }

        private string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}