using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using eform.Models;

namespace eform.CustomMigration
{
    public class eFormDBInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            DefaultSeed.importDefaultRole(context);
            DefaultSeed.importDefaultAdmin(context);
            base.Seed(context);
        }
    }
}