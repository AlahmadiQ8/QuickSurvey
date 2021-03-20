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
                .UseSqlite(@"Data Source=C:\Users\mmoha\Documents\survey.db");
            return new SurveyContext(optionsBuilder.Options);
        }
    }
}
