using System.Net;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;

internal class Program1
{

    static void Main(string[] args)
    {

        string event_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example1\\Event.xml";
        var xmlEvent = XElement.Load(event_path);

        string category_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example1\\Category.xml";
        var xmlCategory = XElement.Load(category_path);

        string agency_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example1\\Agency.xml";
        var xmlAgency = XElement.Load(agency_path);

        string record_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example1\\Records.xml";
        var xmlRecord = XElement.Load(record_path);




        string TaskA_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example1\\TaskA.xml";
        string TaskB_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example1\\TaskB.xml";
        string TaskC_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example1\\TaskC.xml";
        string TaskD_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example1\\TaskD.xml";



        var events = (from eve in xmlEvent.Elements("Event")
                      select new
                      {
                          EventID = (uint)eve.Element("EventID"),
                          DateTime = (DateTime)eve.Element("DateTime"),
                          TextSize = (uint)eve.Element("TextSize")

                      }).ToList();


        var categories = (from cat in xmlCategory.Elements("Category")
                          select new
                          {
                              CategoryID = (uint)cat.Element("CategoryID"),
                              CategoryName = (string)cat.Element("CategoryName"),
                              Location = (string)cat.Element("Location")
                          }).ToList();



        var agencies = (from ag in xmlAgency.Elements("Agency")
                        select new
                        {
                            AgencyID = (string)ag.Element("AgencyID"),
                            AgencyName = (string)ag.Element("AgencyName")

                        }).ToList();



        var records = (from rec in xmlRecord.Elements("Record")
                       select new
                       {

                           EventID = (uint)rec.Element("EventID"),
                           Title = (string)rec.Element("Title"),
                           CategoryID = (uint)rec.Element("CategoryID"),
                           AgencyID = (string)rec.Element("AgencyID")
                       }).ToList();

        // A) 
        var taskA = from record in records
                    join eve in events on record.EventID equals eve.EventID
                    join category in categories on record.CategoryID equals category.CategoryID
                    join agency in agencies on record.AgencyID equals agency.AgencyID
                    orderby eve.DateTime descending
                    select new
                    {
                        Title = record.Title,
                        Date = eve.DateTime.ToShortDateString(),
                        AgencyName = agency.AgencyName
                    };

        var XtaskA = new XElement("TaskA",
            from item in taskA
            select new XElement("Event",
                new XElement("Title", item.Title),
                 new XElement("Date", item.Date),
                    new XElement("AgencyName", item.AgencyName)
                )
            );
        XtaskA.Save(TaskA_path);

        // B) 

        var taskB = from record in records
                    join eve in events on record.EventID equals eve.EventID
                    join category in categories on record.CategoryID equals category.CategoryID
                    join agency in agencies on record.AgencyID equals agency.AgencyID
                    orderby eve.DateTime descending
                    select new
                    {
                        Title = record.Title,
                        Date = eve.DateTime.ToShortDateString(),
                        AgencyName = agency.AgencyName,
                        Location = category.Location

                    };

        var XtaskB = new XElement("TaskB",
            from item in taskB
            group item by new { item.Date } into g
            select new XElement("Dates",
                new XElement("Date", g.Key.Date),
                from gr in g
                group new { gr.Title, gr.AgencyName } by new { gr.Location } into h
                select new XElement("Location", h.Key.Location,
                new XElement("Events",
                 new XElement("Title", h.Select(item => item.Title),
                    new XElement("AgencyName", h.Select(item => item.AgencyName)
                )))))

            );
        XtaskB.Save(TaskB_path);

        // C) 
        var LOCATION = "INSIDE";


        var XtaskC = new XElement($"{LOCATION}",
            from item in taskB
            where item.Location == LOCATION
            group item by new { item.Date } into g
            select new XElement("Dates",
                new XElement("Date", g.Key.Date),
               from gr in g
               select new XElement("Event",
                new XElement("Title", gr.Title)
                    )
                )
            );
        XtaskC.Save(TaskC_path);


        // D) 


        var taskD = from record in records
                    join eve in events on record.EventID equals eve.EventID
                    join category in categories on record.CategoryID equals category.CategoryID
                    join agency in agencies on record.AgencyID equals agency.AgencyID
                    select new
                    {
                        Title = record.Title,
                        Date = eve.DateTime.ToShortDateString(),
                        AgencyName = agency.AgencyName,
                        Location = category.Location

                    };

        var XtaskD = new XElement($"Dates",
            from item in taskD
            orderby item.AgencyName
            group new {item.Date, item.Title, item.AgencyName, item.Location} by new {item.Date} into g
            let maxCount = g.GroupBy(gr => gr.AgencyName).Max(h => h.Count())
            select new XElement("Dates",
                new XElement("Date", g.Key.Date),
                from gr in g
                group gr by new {gr.AgencyName} into h
                where h.Count() == maxCount
                select new XElement("AgencyName", h.Key.AgencyName,
                new XElement("Count", h.Count())
                    ))
            
                
            );
        XtaskD.Save(TaskD_path);

       


    }
     

    
}