using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUnitTest.TestData
{
    public static class AccountTestData
    {
        public static TheoryData<string> InvalidDescData => new()
        {
            new string('A', 256),
            new string('B', 255),
            new string('C', 254),
            new string('C', 1)
        };
    }
}
