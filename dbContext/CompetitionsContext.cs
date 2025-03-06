using site.classes;
using site.models;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;

namespace site.dbContext
{
    public class CompetitionsContext : DbContext
    {
        static private string dbpath;
        static CompetitionsContext()
        {
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            //var exeDirInfo = new DirectoryInfo(exeDir);
            //var projectDir = exeDirInfo.Parent.Parent.FullName;
            dbpath = $@"{exeDir}\files\sqlitedb\konkurses.db";
        }

        public CompetitionsContext() : base(new SQLiteConnection()
        { ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = dbpath }.ConnectionString }, true)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<CompetitionsModel> Competitions { get; set; }

    }

}