using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheCloudShopsCache
{
    class Program
    {
        static string EndPoint = "";
        static string Password = "";
        static IDatabase cache;

        static void Main(string[] args)
        {
            ConfigurationOptions config = new ConfigurationOptions();
            config.EndPoints.Add(EndPoint);
            config.Password = Password;
            config.Ssl = true;
            ConnectionMultiplexer redisHostConnection = ConnectionMultiplexer.Connect(config);
            cache = redisHostConnection.GetDatabase();

            //run command
            RunCommand();

            //Working with strings
            PlayWithStringItem().Wait();
            //Working with integers
            PlayWithIntItem().Wait();
            //Working with Lists
            PlayWithListItem().Wait();
            //Working with Sets
            PlayWithSetItem().Wait();
            //Working with Hashes
            PlayWithHashItem().Wait();

            //General Approach. Cache-aside pattern
            GeneralCachingApproach(0).Wait();            
        }

        private static async Task<Client> GeneralCachingApproach(int ClientId)
        {
            var theKey = new RedisKey($"client:{ClientId}");

            if (await cache.KeyExistsAsync(theKey)) //check if the key exists.
            { 
                HashEntry[] hash = await cache.HashGetAllAsync(theKey);
                Console.WriteLine("General approach, load from cache");

                //might trow error if the obj structure updated
                return new Client  
                {
                    ID = int.Parse(hash.First(k => k.Name == "ID").Value),  
                    Name = hash.First(k => k.Name == "Name").Value            
                };
            }
            else
            {
                Client client = LoadClientFromDB(); //fake db load
                Console.WriteLine("General approach, load DB");

                //convert key to Hash
                await cache.HashSetAsync(theKey,
                    new HashEntry[] {
                        new HashEntry( new RedisValue( "ID" ), new RedisValue( client.ID.ToString() )),
                        new HashEntry( new RedisValue( "Name" ), new RedisValue( client.Name )),
                    }
                 );

                //create key with TTL
                await cache.KeyExpireAsync(theKey, TimeSpan.FromMinutes(1));

                Console.WriteLine("General approach, put in cache");
                return client;
            }           
        }

        private static Client LoadClientFromDB()
        {
            return new Client() { ID = 0, Name = "TheCloudShops" }; 
        }

        private static void RunCommand()
        {
            var cacheCommand = "CLIENT LIST";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            Console.WriteLine("Cache response : \n" + cache.Execute("CLIENT", "LIST").ToString());
        }

        private static async Task PlayWithStringItem()
        {
            RedisKey key = new RedisKey("Msg");
            RedisValue value = new RedisValue("Hello");
            
            await cache.StringSetAsync(key, value).ContinueWith(a => Console.WriteLine($"Create {key} = {value}")); ;

            await cache.StringAppendAsync(key,", world!").ContinueWith(a => Console.WriteLine($"append {key}"));

            await cache.StringGetAsync(key).ContinueWith(value => Console.WriteLine($"Now {key} is {value.Result}"));

            await cache.KeyDeleteAsync(key);

            Console.WriteLine("-------------------------------------------------");
        }

        private static async Task PlayWithIntItem()
        {            
            RedisKey key = new RedisKey("Counter");
            RedisValue value = new RedisValue("1");            

            await cache.StringSetAsync(key,value, TimeSpan.FromMinutes(1))
                .ContinueWith(a => Console.WriteLine($"Create {key} = {value}")); ;

            await cache.StringIncrementAsync(key)
                .ContinueWith(a => Console.WriteLine($"Increment {key}"));

            await cache.StringGetAsync(key)
                .ContinueWith(value => Console.WriteLine($"Now {key} is {value.Result}"));

            Console.WriteLine("-------------------------------------------------");

        }

        private static async Task PlayWithListItem()
        {
            RedisKey key = new RedisKey("WeekDays");
            RedisValue value1 = new RedisValue("Monday");
            RedisValue value2 = new RedisValue("Tuesday");
            RedisValue value3 = new RedisValue("Wednesday");

            await cache.ListLeftPushAsync(key, value1).ContinueWith(a => Console.WriteLine($"Add {value1}"));
            await cache.ListLeftPushAsync(key, value2).ContinueWith(a => Console.WriteLine($"Add {value2}"));
            
            await cache.ListLeftPushAsync(key, value1).ContinueWith(a => Console.WriteLine($"Add {value1}"));
            await cache.ListLeftPushAsync(key, value2).ContinueWith(a => Console.WriteLine($"Add {value2}"));


            await cache.ListRangeAsync(key)
                .ContinueWith(r =>
                {
                    Console.WriteLine($"Now LIST contains:");
                    foreach (var value in r.Result)
                    {
                        Console.WriteLine($"\t{value}");
                    }
                });


            await cache.ListLeftPopAsync(key).ContinueWith(a => Console.WriteLine($"Lpop value: {value2}"));
            await cache.KeyDeleteAsync(key);
            Console.WriteLine("-------------------------------------------------");
        }


        private static async Task PlayWithSetItem()
        {
            RedisKey key = new RedisKey("WeekDays");
            RedisValue value1 = new RedisValue("Monday");
            RedisValue value2 = new RedisValue("Tuesday");
            RedisValue value3 = new RedisValue("Wednesday");

            await cache.SetAddAsync(key, value1).ContinueWith(a => Console.WriteLine($"Add {value1}"));
            await cache.SetAddAsync(key, value2).ContinueWith(a => Console.WriteLine($"Add {value2}"));
            await cache.SetAddAsync(key, value3).ContinueWith(a => Console.WriteLine($"Add {value3}"));
            await cache.SetAddAsync(key, value1).ContinueWith(a => Console.WriteLine($"Add another {value1}")); //add value1 again

            await cache.SetMembersAsync(key)
                .ContinueWith(r =>
                {
                    Console.WriteLine($"Now SET contains:");
                    foreach (var value in r.Result)
                    {
                        Console.WriteLine($"\t{value}");
                    }
                });

            await cache.SetRandomMemberAsync(key).ContinueWith(a => Console.WriteLine($"Random member of set: {a.Result}"));
            await cache.KeyDeleteAsync(key);
            Console.WriteLine("-------------------------------------------------");
        }


        private static async Task PlayWithHashItem()
        {
            RedisKey key = new RedisKey("client-hash");
            var values = new HashEntry[] {
                        new HashEntry( new RedisValue( "ID" ), new RedisValue( "777" )),
                        new HashEntry( new RedisValue( "Name" ), new RedisValue( "TheCloudShops" )),
                    };

            await cache.HashSetAsync(key, values).ContinueWith(a => Console.WriteLine($"Set Hash"));

 
            await cache.HashGetAllAsync(key)
                .ContinueWith(r =>
                {
                    Console.WriteLine($"Now HASH contains:");
                    
                    foreach (HashEntry value in r.Result)
                    {
                        Console.WriteLine($"\t{value.Name}-{value.Value}");
                    }
                });

            await cache.KeyDeleteAsync(key);
            Console.WriteLine("-------------------------------------------------");
        }
    }
    
    public class Client
    {
        public string Name { get; set; }
        public int ID { get; set; }

        public override string ToString()
        {
            return $"Client {ID}:{Name}";
        }
    }

}
