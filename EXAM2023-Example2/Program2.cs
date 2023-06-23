using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

internal class Program2
{

    static void Main(string[] args)
    {

        string show_path = "C:\\Users\\HP\\source\\repos\\EXAM_2023_Example2\\Show.xml";
        var xmlShow = XElement.Load(show_path);

        string collective_path = "C:\\Users\\HP\\source\\repos\\EXAM_2023_Example2\\Collective.xml";
        var xmlCollective = XElement.Load(collective_path);

        string record_path = "C:\\Users\\HP\\source\\repos\\EXAM_2023_Example2\\Record.xml";
        var xmlRecord = XElement.Load(record_path);

        




        string TaskA_path = "C:\\Users\\HP\\source\\repos\\EXAM_2023_Example2\\TaskA.xml";
        string TaskB_path = "C:\\Users\\HP\\source\\repos\\EXAM_2023_Example2\\TaskB.xml";
        string TaskC_path = "C:\\Users\\HP\\source\\repos\\EXAM_2023_Example2\\TaskC.xml";
        string TaskD_path = "C:\\Users\\HP\\source\\repos\\EXAM_2023_Example2\\TaskD.xml";



        var shows = (from a in xmlShow.Elements("Show")
                      select new
                      {
                          ShowID = (uint)a.Element("ShowID"),
                          Time = (DateTime)a.Element("Time"),
                          Location = (string)a.Element("Location")

                      }).ToList();


        var collectives = (from b in xmlCollective.Elements("Collective")
                          select new
                          {
                              CollectiveID = (uint)b.Element("CollectiveID"),
                              CollectiveName = (string)b.Element("CollectiveName")

                          }).ToList();      



        var records = (from d in xmlRecord.Elements("Record")
                       select new
                       {

                           ShowID = (uint)d.Element("ShowID"),
                           Date = (DateTime)d.Element("Date"),
                           Title = (string)d.Element("Title"),
                           CollectiveID = (uint)d.Element("CollectiveID")
                       }).ToList();


        //A) 
        var taskA = from record in records
                    join show in shows on record.ShowID equals show.ShowID
                    join collective in collectives on record.CollectiveID equals collective.CollectiveID
                    select new
                    {
                        Title = record.Title,
                        Date = record.Date.ToShortDateString(),
                        CollectiveName = collective.CollectiveName
                    };

        var XtaskA = new XElement("TaskA",
           from item in taskA
           orderby item.Date
           select new XElement("Show",
               new XElement("Title", item.Title),
                new XElement("Date", item.Date),
                   new XElement("CollectiveName", item.CollectiveName)
               )
           );

        XtaskA.Save(TaskA_path);



        // B) 

        var taskB = from record in records
                    join show in shows on record.ShowID equals show.ShowID
                    join collective in collectives on record.CollectiveID equals collective.CollectiveID
                    select new
                    {
                        Title = record.Title,
                        Date = record.Date.ToShortDateString(),
                        CollectiveName = collective.CollectiveName,
                        Time = show.Time.ToShortTimeString(),
                        Location = show.Location

                    };

        var XtaskB = new XElement("TaskB",
          from item in taskB
          orderby item.Date
          select new XElement("Show",
              new XElement("Title", item.Title),
               new XElement("Date", item.Date),
                  new XElement("CollectiveName", item.CollectiveName),
                   new XElement("Time", item.Time),
                    new XElement("Location", item.Location)
              )
          );

        XtaskB.Save(TaskB_path);


        // C) 

        var COLLECTIVE = "Tvorchi";


        var XtaskC = new XElement("TaskC",
         from item in taskB
         where item.CollectiveName == COLLECTIVE
         orderby item.Title
         group item by new {item.Title} into g
         select new XElement($"{COLLECTIVE}",
             new XElement("Title", g.Key.Title),
             from gr in g 
             select  new XElement("Date", gr.Date)
              
                 
             )
         );

        XtaskC.Save(TaskC_path);


        //D) 
        var XtaskD = new XElement($"TaskD",
           from item in taskB
           orderby item.Location
           group item by new { item.Location } into g
           let maxCount = g.GroupBy(gr => gr.CollectiveName).Max(h => h.Count())
           select new XElement("Location",
               new XElement("LocationName", g.Key.Location),
               from gr in g
               group gr by new { gr.CollectiveName } into h
               where h.Count() == maxCount
               select new XElement("CollectiveName", h.Key.CollectiveName,
               new XElement("Count", h.Count())
                   ))


           );

                    
        XtaskD.Save(TaskD_path);





    }
}