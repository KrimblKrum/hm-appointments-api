using AppointmentsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace AppointmentsApi.Data
{
    public interface IAppointmentsDbContext
    {
        DbSet<AppointmentEntity>? Appointments { get; set; }
        DbSet<ClientEntity>?  Clients { get; set; }
        DbSet<ProviderEntity>? Providers { get; set; }
        DbSet<ScheduleEntity>? Schedules { get; set; }

        int SaveChanges();
    }

    [ExcludeFromCodeCoverage]
    public class AppointmentsDbContext : DbContext, IAppointmentsDbContext
    {
        protected readonly IConfiguration Configuration;

        public AppointmentsDbContext(IConfiguration configuration) : base()
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(Configuration.GetConnectionString("AppointmentsDatabase"));
            //this.SaveChanges();
        }

        public DbSet<AppointmentEntity>? Appointments { get; set; }
        public DbSet<ClientEntity>? Clients { get; set; }
        public DbSet<ProviderEntity>? Providers { get; set; }
        public DbSet<ScheduleEntity>? Schedules { get; set; }
    }
}
