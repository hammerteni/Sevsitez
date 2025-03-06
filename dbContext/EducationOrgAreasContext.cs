using site.classes;
using site.models;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;

namespace site.dbContext
{
    public class EducationOrgAreasContext : DbContext
    {
        static private string dbpath;
        static EducationOrgAreasContext()
        {
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            //var exeDirInfo = new DirectoryInfo(exeDir);
            //var projectDir = exeDirInfo.Parent.Parent.FullName;
            dbpath = $@"{exeDir}\files\sqlitedb\educaion_org_areas.db";
        }

        public EducationOrgAreasContext() : base(new SQLiteConnection()
        { ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = dbpath }.ConnectionString }, true)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<EducationOrganizations> EducationOrganization { get; set; }

        public DbSet<RegionAreaCities> RegionAreaCity { get; set; }

        public DbSet<EducationOrganizationTypes> EducationOrganizationType { get; set; }
    }
}