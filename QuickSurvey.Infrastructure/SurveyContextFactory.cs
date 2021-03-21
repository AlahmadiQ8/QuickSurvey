using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QuickSurvey.Infrastructure
{
    class SurveyContextFactory : IDesignTimeDbContextFactory<SurveyContext>
    {
        public SurveyContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SurveyContext>()
                .UseSqlServer(SurveyContext.ConnectionString);
            return new SurveyContext(optionsBuilder.Options);
        }
    }
}
