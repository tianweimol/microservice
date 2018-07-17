using System;
using System.Collections.Generic;
using System.Text;
using Thrift.Contract.Contract.UserContract;

namespace ThriftTest.Server.ImplContract
{
    public class UserServiceImpl : UserService.Iface
    {
        private static List<User> userList { get; set; } = new List<User>();
        public User Get(int id)
        {
            return new User() {
                Age=1,
                Id=11,
                IsVIP=true,
                Name="Mol",
                Remark="mol's remark"
            };
        }

        public List<User> GetAll()
        {
            for (int i = 0; i < 15; i++)
            {
                userList.Add(new User(i,"Name"+i,i+20));
            }
            return userList;
        }

        public SaveResult Save(User user)
        {
            userList.Add(user);
            Console.WriteLine($"保存用户， {user.Id}");
            return SaveResult.SUCCESS;
        }
    }
}
