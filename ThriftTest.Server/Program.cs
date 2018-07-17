using System;
using Thrift.Server;
using Thrift.Transport;
using ThriftTest.Server.ImplContract;

namespace ThriftTest.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TServerTransport transport = new TServerSocket(8800);
            var processor = new Thrift.Contract.Contract.UserContract.UserService.Processor(new UserServiceImpl());
            TServer server = new TThreadPoolServer(processor, transport);
            server.Serve();
            Console.ReadKey();
        }
    }
}
