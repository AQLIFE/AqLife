using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUnitTest.Application
{
    [CollectionDefinition]
    public class IntegrationTestCollection
    : ICollectionFixture<IntegrationTestFixture>
    {
    }
}
