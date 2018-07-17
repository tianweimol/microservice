using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Xunit;
using System.Linq;

namespace ConsulTest
{
    class Program
    {
        static AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        static string dataFromServer = "原始";
        static void Main(string[] args)
        {
            //Console.WriteLine(Base64Encode(Encoding.UTF8, "kyygapiserver:123456"));
            //Console.WriteLine(Base64Encode(Encoding.Unicode, "kyygapiserver:123456"));
            //Console.WriteLine(Base64Encode(Encoding.ASCII, "kyygapiserver:123456"));
            //Console.WriteLine(Base64Encode(Encoding.Default, "kyygapiserver:123456"));

            //var val = Regex.Replace(@"/api/customer/{parentid}", @"{\w}", "**");
            //Regex reg = new Regex(@"{\w*}");
            //var str = reg.Replace(@"/api/customer/{parentid}", "**");
            //Console.WriteLine(val);
            //Console.WriteLine(str);

            //Task task = Task.Factory.StartNew(() =>
            //{
            //    GetDataFromServer();
            //});

            //autoResetEvent.WaitOne(2000);

            ////Thread got the signal
            //Console.WriteLine(dataFromServer);

            // db
            int i = 2;
            int a = i++ + ++i;
            Console.WriteLine(a);
            for (int j = 0; j < 100; j++)
            {
                i = i++;
            }
            Console.WriteLine(i);
            return;
            var comment = new NewsComment()
            {
                CommentText = "CommentText",
                Id = 1
            };
            var cloneEntity = CloneHelper<NewsComment>.Clone(comment);
            Console.WriteLine ("CommentText".Equals( cloneEntity.CommentText));
            Console.WriteLine(1==cloneEntity.Id);
            Console.WriteLine(comment.Equals(cloneEntity));


            var ser = new SerClass()
            {
                CommentText = "CommentText",
                Id = 1
            };
            var cloneSerEntity = CloneHelper<SerClass>.Clone(ser);
            Console.WriteLine("CommentText".Equals(cloneSerEntity.CommentText));
            Console.WriteLine(1 == cloneSerEntity.Id);
            Console.WriteLine(ser.Equals(cloneSerEntity));
            Console.ReadKey();
        }


        private static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }

        static void ThreadMethod()
        {
            while (!autoResetEvent.WaitOne(TimeSpan.FromSeconds(2)))
            {
                Console.WriteLine("Continue");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            Console.WriteLine("Thread got signal");
        }

        static void GetDataFromServer()
        {
            //Calling any webservice to get data
            Thread.Sleep(TimeSpan.FromSeconds(4));
            dataFromServer = "Webservice data";
            autoResetEvent.Set();
        }
    }


    public class CloneHelper<T> where T : class, new()
    {
        public static T Clone(T input)
        {
            if (typeof(T).IsSerializable)
            {
                // 如果可以序列化
                using (Stream objectStream = new MemoryStream())
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(objectStream, input);
                    objectStream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(objectStream);
                }
            }
            else
            {
                // 如果不能序列化，则反射
                var re = new T();
                var properties = input.GetType().GetProperties().ToList();
                properties.ForEach(item => item.SetValue(re, item.GetValue(input, null)));
                return re;
            }
        }
    }

    public class CloneHelperTester
    {
        public void test()
        {
            var comment = new NewsComment()
            {
                CommentText = "CommentText",
                Id = 1
            };
            var cloneEntity = CloneHelper<NewsComment>.Clone(comment);
            Assert.Equal("CommentText", cloneEntity.CommentText);
            Assert.Equal(1, cloneEntity.Id);
            Assert.False(comment.Equals(cloneEntity));

        }
    }

    public class NewsComment
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
    }
    [Serializable]
    public class SerClass
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
    }
}
