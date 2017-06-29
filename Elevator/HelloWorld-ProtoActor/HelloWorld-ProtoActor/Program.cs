using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld_ProtoActor
{
    class Program
    {
        static void Main(string[] args)
        {
            var props = Actor.FromProducer(() => new HelloActor());
            var pid = Actor.Spawn(props);
            pid.Tell(new HelloMessage { Who = "Haishi" });
            Console.ReadKey();
        }
    }
}
