using System;
using System.Collections.Generic;
using System.Text;

namespace BaseApi.Common
{
   public class IdHandler
    {
        public static Guid GetNewGuid()
        {
            return Guid.NewGuid();
        }
    }
}
