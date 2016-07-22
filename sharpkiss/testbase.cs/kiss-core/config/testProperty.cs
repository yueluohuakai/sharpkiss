using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace testbase.cs.kiss_core
{
   public class testProperty
    {
        public void testPropertyCS()
        {
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                string x = "";
            }

        }

        public string test_1 { get; set; }
    }
}
