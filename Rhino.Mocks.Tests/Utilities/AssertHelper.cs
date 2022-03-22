using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rhino.Mocks
{
    public static class AssertHelper
    {
        public static T Throws<T>(string message, Action testCode) where T : Exception
        {
            var ex = Assert.Throws<T>(testCode);
            Assert.Equal(message, ex.Message);

            return ex;
        }
    }
}
