using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HTTPServerLib
{
    public interface ILogger
    {
        void Log(object message);
    }
}
