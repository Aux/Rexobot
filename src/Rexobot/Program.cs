using System;
using System.Threading.Tasks;

namespace Rexobot
{
    class Program
    {
        static Task Main(string[] args)
            => new Startup(args).StartAsync();
    }
}
