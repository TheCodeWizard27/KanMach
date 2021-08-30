using KanMach.Core;
using System;

namespace KanMach.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            KanGameEngineBuilder
                .CreateDefaultBuilder()
                .SetStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
