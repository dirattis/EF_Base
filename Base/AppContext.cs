using PHS.Model;
using EntityFramework.DynamicFilters;
using System.Data.Entity;
//using EntityFramework.DynamicFilters;

namespace PHS.Data
{
    public partial class AppContext : DbContext
    {
        public AppContext()
            : base("DefaultConnection")
        {
            //Desligado o LazyLoading do EF não permitindo carregar dados filhos após a carga da entidade
            this.Configuration.LazyLoadingEnabled = false;
        }

        public AppContext(string connectionString)
            : base(connectionString)
        {
            //Desligado o LazyLoading do EF não permitindo carregar dados filhos após a carga da entidade
            this.Configuration.LazyLoadingEnabled = false;
        }

        #region Tabelas Gerais
        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Siteplan> Siteplans { get; set; }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Não retorna itens excluídos por padrão
            // modelBuilder.Filter("Deleted", (EntityDB e) => e.Deleted, false);
            #region [ config ]
            modelBuilder.Entity<Config>()
                .Property(e => e.Key)
                .IsUnicode(false);

            modelBuilder.Entity<Config>()
                .Property(e => e.Category)
                .IsUnicode(false);

            modelBuilder.Entity<Config>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Config>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<Config>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Config>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);
            #endregion

            #region [ Log ]
            modelBuilder.Entity<Log>()
                .Property(e => e.Level)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Source)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Uri)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Message)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Detail)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);
            #endregion

            #region [ SitePlan ]
            modelBuilder.Entity<Siteplan>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Siteplan>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Siteplan>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Siteplan>()
                .HasMany(e => e.Siteplan1)
                .WithOptional(e => e.Siteplan2)
                .HasForeignKey(e => e.ParentId);
            #endregion
        }
    }
}
