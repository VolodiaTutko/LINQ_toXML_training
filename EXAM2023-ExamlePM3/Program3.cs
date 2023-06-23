using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

internal class Program2
{

    static void Main(string[] args)
    {

        string certificate_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM3\\Certificate.xml";
        var xmlCertificate = XElement.Load(certificate_path);

        string theory_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM3\\Theory.xml";
        var xmlTheory = XElement.Load(theory_path);

        string practise_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM3\\Practise.xml";
        var xmlPractise = XElement.Load(practise_path);

        string record_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM3\\Record.xml";
        var xmlRecord = XElement.Load(record_path);






        string TaskA_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM3\\TaskA.xml";
        string TaskB_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM3\\TaskB.xml";
        string TaskC_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM3\\TaskC.xml";
        string TaskD_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-ExamplePM3\\TaskD.xml";



        var certificates = (from a in xmlCertificate.Elements("Certificate")
                     select new
                     {
                         CertificateID = (uint)a.Element("CertificateID"),
                         DriverSurname = (string)a.Element("DriverSurname"),
                         Category = (string)a.Element("Category"),
                         DateCertificate = (DateTime)a.Element("DateCertificate")

                     }).ToList();



        var theories = (from b in xmlTheory.Elements("Theory")
                           select new
                           {
                               TheoryID = (uint)b.Element("TheoryID"),
                               TheoryQuestion = (string)b.Element("TheoryQuestion"),
                               TheoryMark = (uint)b.Element("TheoryMark")

                           }).ToList();

        var practises = (from b in xmlPractise.Elements("Practise")
                           select new
                           {
                               PractiseID = (uint)b.Element("PractiseID"),
                               Auto = (string)b.Element("Auto"),
                               PractiseMark = (uint)b.Element("PractiseMark")

                           }).ToList();

        var records = (from b in xmlRecord.Elements("Record")
                           select new
                           {
                               CertificateID = (uint)b.Element("CertificateID"),
                              ExamDate = (DateTime)b.Element("ExamDate"),
                               TheoryID = (uint)b.Element("TheoryID"),
                               PractiseID = (uint)b.Element("PractiseID")


                           }).ToList();

        
        var taskB = from record in records
                    join certificate in certificates on record.CertificateID equals certificate.CertificateID
                    join theory in theories on record.TheoryID equals theory.TheoryID
                    join practise in practises on record.PractiseID equals practise.PractiseID
                    select new
                    {
                        DriverSurname = certificate.DriverSurname,
                        Category = certificate.Category,
                        Thermin = (certificate.DateCertificate - record.ExamDate).TotalDays,
                        Thermins = $"{record.ExamDate.ToShortDateString()} - {certificate.DateCertificate.ToShortDateString()}",
                        Mark = theory.TheoryMark + practise.PractiseMark,
                        TheoryMark = theory.TheoryMark,
                        Question = theory.TheoryQuestion

                    };

        var XtaskA = new XElement("TaskA",
           from item in taskB
          orderby item.DriverSurname
           select new XElement("Certificate",  
               new XElement("DriverSurname", item.DriverSurname),
                new XElement("Category", item.Category),
                 new XElement("Thermins", item.Thermins),
                   new XElement("Thermin_in_days", item.Thermin)
               )
           );

        XtaskA.Save(TaskA_path);

        // B) 

        

        var XtaskB = new XElement("TaskB",
           from item in taskB
           orderby item.DriverSurname
           select new XElement("Certificate",
               new XElement("DriverSurname", item.DriverSurname),
                new XElement("Category", item.Category),
                 new XElement("Thermins", item.Thermins),
                   new XElement("Thermin_in_days", item.Thermin),
                   new XElement("SumofMark", item.Mark)
               )
           );

        XtaskB.Save(TaskB_path);


        //C)

        var XtaskC = new XElement("TaskC",
          from item in taskB
          where item.Thermin < 2*365
          group item by new {item.Category} into g
          select new XElement("Category",
              new XElement("CategoryTitle", g.Key.Category),
              from gr in g
              select new XElement("DriverSurname",  gr.DriverSurname)                          
              )
          );

        XtaskC.Save(TaskC_path);

        //D)
        var XtaskD = new XElement($"TaskD",
          from item in taskB
          group item by new { item.Category,  } into g
          let minMark = g.Min(it=>it.TheoryMark)
          select new XElement("Category",
              new XElement("CategoryTitle", g.Key.Category),
              from gr in g
              where gr.TheoryMark == minMark
              select new XElement("Question", gr.Question))          
                  
          );
        XtaskD.Save(TaskD_path);



    }
}