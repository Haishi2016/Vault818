using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld_ProtoActor
{
    internal class HelloActor : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            var message = context.Message;
            if (message is HelloMessage m)
            {
                Console.WriteLine($"Hello World, {m.Who}!");
            }
            return Actor.Done;
        }
    }
}
