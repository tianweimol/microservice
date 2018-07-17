using System;
using Thrift.Contract.Contract.UserContract;
using Thrift.Protocol;
using Thrift.Transport;

namespace ThriftTest.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TTransport transport = new TSocket("localhost", 8800))
            using (TProtocol protocol = new TBinaryProtocol(transport))
            using (var clientUser = new UserService.Client(protocol))
            {
                transport.Open();
                clientUser.Save(new User() {
                    Age=50,
                    Id=50,
                    Name="Jack"
                });
                var re = clientUser.GetAll();
                foreach (var u in re)
                {
                    Console.WriteLine($"{u.Id},{u.Name}");
                }


                
            }
            Console.WriteLine("Hello World!");
        }
    }
}
