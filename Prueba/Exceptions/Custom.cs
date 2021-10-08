using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductosManager.Exceptions
{
    public class Custom : Exception
    {
        public Custom(string message) { }

        public static void TestThrow()
        {
            throw new Custom("");
        }
    }
    
}
