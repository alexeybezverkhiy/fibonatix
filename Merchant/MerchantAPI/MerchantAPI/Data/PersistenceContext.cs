using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace MerchantAPI.Data
{
    public class PersistenceContext : DbContext
    {
        public PersistenceContext() : base("PersistenceContext")
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<MerchantCallback> MerchantCallbacks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}