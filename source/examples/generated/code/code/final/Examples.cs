using System;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using NUnit.Framework;
using Realms;
using Realms.Sync;
using TaskStatus = dotnet.TaskStatus;
using Task = dotnet.Task;
using User = dotnet.User;


namespace UnitTests
{
    public class Examples
    {
        App app;
        ObjectId testTaskId;
        Realms.Sync.User user;
        SyncConfiguration config;
        const string myRealmAppId = "tuts-tijya";

        [OneTimeSetUp]
        public async System.Threading.Tasks.Task Setup()
        {
            app = App.Create(myRealmAppId);
            user = app.LogInAsync(Credentials.EmailPassword("foo@foo.com", "foobar")).Result;
            config = new SyncConfiguration("myPartition", user);
            var realm = await Realm.GetInstanceAsync(config);
            var synchronousRealm = Realm.GetInstance(config);
            var testTask = new Task
            {
                Name = "Do this thing",
                Status = TaskStatus.Open.ToString()
            };

            realm.Write(() =>
            {
                realm.Add(testTask);
            });
            testTaskId = testTask._id;
            return;
        }

        [Test]
        public void OpensLocalRealm()
        {
            var pathToDb = Directory.GetCurrentDirectory() + "/db";
            if (!File.Exists(pathToDb))
            {
                Directory.CreateDirectory(pathToDb);
            }
            var tempConfig = new RealmConfiguration(pathToDb + "/my.realm")
            {
                IsReadOnly = false,
            };
            var realm = Realm.GetInstance(tempConfig);

            realm.Dispose();

            var config = new RealmConfiguration(pathToDb + "/my.realm")
            {
                IsReadOnly = true,
            };
            var localRealm = Realm.GetInstance(config);
            Assert.IsNotNull(localRealm);
            localRealm.Dispose();
            try
            {
                Directory.Delete(pathToDb, true);
            } catch (Exception)
            {

            }
        }

        [Test]
        public async System.Threading.Tasks.Task GetsSyncedTasks()
        {
            var user = app.LogInAsync(Credentials.Anonymous()).Result;
            config = new SyncConfiguration("myPartition", user);
            var realm = await Realm.GetInstanceAsync(config);
            var tasks = realm.All<Task>();
            Assert.AreEqual(1, tasks.Count(),"Get All");
            tasks = realm.All<Task>().Where(t => t.Status == "Open");
            Assert.AreEqual(1, tasks.Count(), "Get Some");
            return;
        }

        [Test]
        public async System.Threading.Tasks.Task ScopesARealm()
        {
            config = new SyncConfiguration("myPartition", user);
            using var realm = await Realm.GetInstanceAsync(config);
            var allTasks = realm.All<Task>();
        }

        [Test]
        public async System.Threading.Tasks.Task ModifiesATask()
        {
            config = new SyncConfiguration("myPartition", user);
            var realm = await Realm.GetInstanceAsync(config);
            var t = realm.All<Task>()
                .Where(t => t._id == testTaskId)
                .FirstOrDefault();

            realm.Write(() =>
            {
                t.Status = TaskStatus.InProgress.ToString();
            });

            var allTasks = realm.All<Task>().ToList();
            Assert.AreEqual(1, allTasks.Count);
            Assert.AreEqual(TaskStatus.InProgress.ToString(), allTasks.First().Status);

            return;
        }

        [Test]
        public async System.Threading.Tasks.Task LogsOnManyWays()
        {
            {
                var user = await app.LogInAsync(Credentials.Anonymous());
                Assert.AreEqual(UserState.LoggedIn, user.State);
                await user.LogOutAsync();
            }
            {
                var user = await app.LogInAsync(
                    Credentials.EmailPassword("caleb@mongodb.com", "shhhItsASektrit!"));
                Assert.AreEqual(UserState.LoggedIn, user.State);
                await user.LogOutAsync();
            }
            {
                var apiKey = "eRECwv1e6gkLEse99XokWOgegzoguEkwmvYvXk08zAucG4kXmZu7TTgV832SwFCv";
                var user = await app.LogInAsync(Credentials.ApiKey(apiKey));
                Assert.AreEqual(UserState.LoggedIn, user.State);
                await user.LogOutAsync();
            }
            {
                var functionParameters = new
                {
                    username = "caleb",
                    password = "shhhItsASektrit!",
                    IQ = 42,
                    isCool = false
                };

                var user =
                    await app.LogInAsync(Credentials.Function(functionParameters));
                Assert.AreEqual(UserState.LoggedIn, user.State);
                await user.LogOutAsync();
            }
            {
                var jwt_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkNhbGViIiwiaWF0IjoxNjAxNjc4ODcyLCJleHAiOjI1MTYyMzkwMjIsImF1ZCI6InR1dHMtdGlqeWEifQ.LHbeSI2FDWrlUVOBxe-rasuFiW-etv2Gu5e3eAa6Y6k";
                var user =
                    await app.LogInAsync(Credentials.JWT(jwt_token));
                Assert.AreEqual(UserState.LoggedIn, user.State);
                await user.LogOutAsync();
            }
            try
            {
                var facebookToken = "";
                var user =
                    await app.LogInAsync(Credentials.Facebook(facebookToken));
            }
            catch (Exception e)
            {
                Assert.AreEqual("InvalidSession: authentication via 'oauth2-facebook' is unsupported", e.Message);
            }
            try
            {
                var googleAuthCode = "";
                var user =
                    await app.LogInAsync(Credentials.Google(googleAuthCode));
            }
            catch (Exception e)
            {
                Assert.AreEqual("InvalidSession: authentication via 'oauth2-google' is unsupported", e.Message);
            }
            try
            {
                var appleToken = "";
                var user =
                    await app.LogInAsync(Credentials.Apple(appleToken));
            }

            catch (Exception e)
            {
                Assert.AreEqual("InvalidSession: authentication via 'oauth2-apple' is unsupported", e.Message);
            }
        }

        [Test]
        public async System.Threading.Tasks.Task CallsAFunction()
        {
            var bsonValue = await
                user.Functions.CallAsync("sum", 2, 40);

            // The result must now be cast to Int32:
            var sum = bsonValue.ToInt32();

            // Or use the generic overloads to avoid casting the BsonValue:
            sum = await
               user.Functions.CallAsync<int>("sum", 2, 40);
            Assert.AreEqual(42, sum);
            var task = await user.Functions.CallAsync<MyClass>
                ("getTask", "5f7f7638024a99f41a3c8de4");

            var name = task.Name;
            return;

            //{ "_id":{ "$oid":"5f0f69dc4eeabfd3366be2be"},"_partition":"myPartition","name":"do this NOW","status":"Closed"}
        }

       [Test]
        public async System.Threading.Tasks.Task CreateEmbeddedObj()
        {
            // OPEN A REALM
            app = App.Create(myRealmAppId);
            user = app.LogInAsync(Credentials.EmailPassword("foo@foo.com", "foobar")).Result;
            config = new SyncConfiguration("myPartition", user);
            var realm = await Realm.GetInstanceAsync(config);



            // create a User (a user is an assignee)
            var myUser = new User {
                Name = "Test User 001"
            };

            realm.Write(() =>
            {
                realm.Add(myUser);
            });

            // create a Task that has an embedded Assignee 

            var taskItem = new Task
            {
                Name = "Go Jogging",
                Assignee = myUser

            };

            realm.Write(() =>
            {
                realm.Add(taskItem);
            });


            var id = taskItem._id;

            var allTasks = realm.All<Task>().Where(t => t._id == taskItem._id).ToList();

            Assert.IsNotNull(allTasks);
            Assert.AreEqual(allTasks.Count, 1);

        }

        [OneTimeTearDown]
        public async System.Threading.Tasks.Task TearDown()
        {
            config = new SyncConfiguration("myPartition", user);
            using (var realm = await Realm.GetInstanceAsync(config))
            {
                realm.Write(() =>
                {
                    realm.RemoveAll<Task>();
                });

                var realmUser = await app.LogInAsync(Credentials.Anonymous());
                await realmUser.LogOutAsync();
            }
            return;
        }
 
    }

    public class MyClass : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; }

        [MapTo("name")]
        [Required]
        public string Name { get; set; }
       
        public MyClass()
        {
            this.Id = ObjectId.GenerateNewId();
        }
    }
}