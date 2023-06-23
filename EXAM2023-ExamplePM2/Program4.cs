using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

internal class Program4
{

    static void Main(string[] args)
    {

        string test_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM2\\Test.xml";
        var xmlTest = XElement.Load(test_path);

        string platform_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM2\\Platforma.xml";
        var xmlPlatform = XElement.Load(platform_path);

        string test_server_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM2\\TestServer.xml";
        var xmlTestServer = XElement.Load(test_server_path);

        string record_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM2\\Record.xml";
        var xmlRecord = XElement.Load(record_path);



        string TaskA_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM2\\TaskA.xml";
        string TaskB_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM2\\TaskB.xml";
        string TaskC_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM2\\TaskC.xml";
        string TaskD_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM2\\TaskD.xml";



        var tests = (from a in xmlTest.Elements("Test")
                     select new
                     {
                         TestID = (uint)a.Element("TestID"),
                         UserLogin = (string)a.Element("UserLogin"),
                         ProgrammingLanguage = (string)a.Element("ProgramL"),
                         TestSize  = (uint)a.Element("TestSize"),

                     }).ToList();
        var platforms = (from a in xmlPlatform.Elements("Platform")
                     select new
                     {
                         PlatformID = (string)a.Element("PlatformID"),
                         PlatformName = (string)a.Element("PlatformName"),
                         

                     }).ToList();

        var test_servers = (from a in xmlTestServer.Elements("TestServer")
                     select new
                     {
                         TestServerID = (string)a.Element("TestServerID"),
                         PlatformID = (string)a.Element("PlatformID")
                     }).ToList();

        var records = (from a in xmlRecord.Elements("Record")
                            select new
                            {
                                TestID = (uint)a.Element("TestID"),
                                DateTime = (DateTime)a.Element("DateTime"),
                                TestServerID = (string)a.Element("TestServerID"),
                                Result = (string)a.Element("Result"),
                            }).ToList();

        Console.WriteLine(platforms.Count());


        //A) файл де подана інформація про усі тести у такому вигляді :
        //< ід тесту, логін користувача, дата і час тестування , результат тестування> вміст впорядкувати за логіном;
        var taskA = from record in records
                    join test in tests on record.TestID equals test.TestID
                    select new
                    {
                        TestID = test.TestID,
                        UserLogin = test.UserLogin,
                        DateTime = record.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Result = record.Result

                    };

        var XtaskA = new XElement("TaskA",
           from item in taskA
           orderby item.UserLogin
           select new XElement("Test",
               new XElement("TestID", item.TestID),
                new XElement("UserLogin", item.UserLogin),
                 new XElement("DateTime", item.DateTime),
                   new XElement("Result", item.Result)
               )
           );

        XtaskA.Save(TaskA_path);


        //B) файл подібний до попереднього але доповнений назвою платформи і впорядкований за цією назвою.

        var joined = from test_server in test_servers
                     join platform in platforms on test_server.PlatformID equals platform.PlatformID
                     select new
                     {
                         TestServerID = test_server.TestServerID,
                         PlatformID = platform.PlatformID,
                         PlatformName = platform.PlatformName,

                     };

        var taskB = from record in records
                    join test in tests on record.TestID equals test.TestID
                    join jn in joined on record.TestServerID equals jn.TestServerID                  
                    select new
                    {
                        TestID = test.TestID,
                        UserLogin = test.UserLogin,
                        DateTime = record.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Result = record.Result,
                        PlatformName = jn.PlatformName,
                        TestSize = test.TestSize,
                        ProgramLanguage = test.ProgrammingLanguage
                       
                    };

        var XtaskB = new XElement("TaskB",
          from item in taskB
          orderby item.PlatformName
          select new XElement("Test",
              new XElement("TestID", item.TestID),
               new XElement("UserLogin", item.UserLogin),
                new XElement("DateTime", item.DateTime),
                  new XElement("Result", item.Result),
                  new XElement("PlatformName", item.PlatformName)
              )
          );

        XtaskB.Save(TaskB_path);

        // C)  файл для кожної платформи вказано сумарний обсяг відтестованого коду.

         var XtaskC = new XElement("TaskC",
         from item in taskB
         group item by new {item.PlatformName} into g       
         select new XElement("Platform",
             new XElement("PlatformName", g.Key.PlatformName),
             new XElement("ALL_SIZE",g.Sum(it=> it.TestSize)
             
             ))
         );

        XtaskC.Save(TaskC_path);


        // D)  файл в якому для кожного логіна(впорядкований по алфавіту)
        // вказано для кожної використаної мови відсоток успішних тестів.



        var taskD = from item in taskB
                    group item by item.UserLogin into g
                    orderby g.Key
                    select new
                    {
                        UserLogin = g.Key,
                        TestLanguages = from gr in g
                                        group gr by gr.ProgramLanguage into h
                                        let successfulTests = h.Count(x => x.Result == "CORRECT")
                                        let totalTests = h.Count()
                                        let successRate = totalTests > 0 ? (double)successfulTests / totalTests * 100 : 0
                                        orderby h.Key
                                        select new
                                        {
                                            ProgrammingLanguage = h.Key,
                                            SuccessRate = successRate
                                        }
                    };

        var XtaskD = new XElement("TaskD",
                        from item in taskD
                        select new XElement("UserLogins",
                            new XElement("UserLogin", item.UserLogin),
                            from language in item.TestLanguages
                            select new XElement("ProgrammingLanguage",
                                new XAttribute("Name", language.ProgrammingLanguage),
                                new XAttribute("SuccessRate", language.SuccessRate.ToString("0.00") + "%")
                            )
                        )
                    );

        XtaskD.Save(TaskD_path);

        

    }
}