using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CashBook.Data.Model
{
    public class DatabaseInitializer
    {
        public void PerformInitialize()
        {
            //SqlCeEngine engine = new SqlCeEngine("Data Source = AdventureWorks.sdf");
            //engine.Shrink();

            var context = new CashBookContainer();
            //context.ContextOptions.UseLegacyPreserveChangesBehavior
        }
    }
}
