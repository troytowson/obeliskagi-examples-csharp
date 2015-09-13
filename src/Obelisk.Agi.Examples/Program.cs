using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Obelisk.Agi;
using Obelisk.Agi.Bootstrappers.Structuremap;
using Obelisk.Agi.Commands;

using StructureMap;

namespace Obelisk.TestHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ObeliskConfiguration(IPAddress.Any, 4573);
            configuration.UseBootstrapper<MyStructuremapBootstrapper>();

            var agi = ObeliskAgi.Create(configuration);
            agi.Start();
            Console.ReadLine();
            agi.Stop();
        }
    }

    public class MyStructuremapBootstrapper : StructuremapBootstrapper
    {
        protected override void Configure(IContainer container)
        {
            base.Configure(container);
            container.Configure(c => c.For<IObeliskScript>().Use<MyScript>().Named("FlowHandler"));
        }
    }

    public class MyScript : IObeliskScript
    {
        public async Task RunAsync(IObeliskChannel channel)
        {
            try
            {
                Console.WriteLine("Call Started: {0} on: {1}", channel.Context.ChannelName, Thread.CurrentThread.ManagedThreadId);
                await channel.SendCommandAsync(new ExecuteCommand("RINGING"));
                Console.WriteLine("Call RINGING: {0} on: {1}", channel.Context.ChannelName, Thread.CurrentThread.ManagedThreadId);
                await channel.SendCommandAsync(new WaitCommand(1));
                Console.WriteLine("Call WAIT: {0} on: {1}", channel.Context.ChannelName, Thread.CurrentThread.ManagedThreadId);
                await channel.SendCommandAsync(new AnswerCommand());
                Console.WriteLine("Call ANSWER: {0} on: {1}", channel.Context.ChannelName, Thread.CurrentThread.ManagedThreadId);
                await channel.SendCommandAsync(new WaitCommand(1));
                Console.WriteLine("Call WAIT: {0} on: {1}", channel.Context.ChannelName, Thread.CurrentThread.ManagedThreadId);
                await channel.SendCommandAsync(new HangUpCommand(channel.Context.ChannelName));
                Console.WriteLine("Call Ended: {0} on: {1}", channel.Context.ChannelName, Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception)
            {
                Console.WriteLine("Error: {0} on: {1}", channel.Context.ChannelName, Thread.CurrentThread.ManagedThreadId);
            }
        }
    }
}
