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

        string player_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example4\\Player.xml";
        var xmlPlayer = XElement.Load(player_path);

        string fc_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example4\\FC.xml";
        var xmlFC = XElement.Load(fc_path);

        string history_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example4\\HistoryTransfer.xml";
        var xmlHistory = XElement.Load(history_path);

        



        string TaskA_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example4\\TaskA.xml";
        string TaskB_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example4\\TaskB.xml";
        string TaskC_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example4\\TaskC.xml";
        string TaskD_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example4\\TaskD.xml";



        var players = (from a in xmlPlayer.Elements("Player")
                     select new
                     {
                         PlayerID = (uint)a.Element("PlayerID"),
                         Name = (string)a.Element("Name"),
                         Surname = (string)a.Element("Surname"),
                         TransferPrice = (uint)a.Element("TransferPrice")

                     }).ToList();

        var fcs = (from a in xmlFC.Elements("FC")
                       select new
                       {
                           FCID = (uint)a.Element("FCID"),
                           FCName = (string)a.Element("FCName"),
                           TransferBudget = (uint)a.Element("TransferBudget")

                       }).ToList();

        var histories = (from a in xmlHistory.Elements("History")
                       select new
                       {
                           PlayerID = (uint)a.Element("PlayerID"),
                           FROMID = (uint)a.Element("FCIDfrom"),
                           TOID = (uint)a.Element("FCIDto"),
                           Transfer = (uint)a.Element("Transfer"),
                           Date = (DateTime)a.Element("Date")

                       }).ToList();

        Console.WriteLine(histories.Count());


        // A) 
        var taskA = from history in histories
                    join player in players on history.PlayerID equals player.PlayerID
                    join FCFROM in fcs on history.FROMID equals FCFROM.FCID
                    join FCTO in fcs on history.TOID equals FCTO.FCID
                    select new
                    {
                        Player = player.Surname + " " + player.Name.FirstOrDefault() + ".",
                        FROM_TO = FCFROM.FCName + " - " + FCTO.FCName,
                        TransferSum = history.Transfer,
                        Date = history.Date.ToShortDateString(),
                        From = FCFROM.FCName,
                        To = FCTO.FCName

                    };

        var XtaskA = new XElement("TaskA",
            from item in taskA
            orderby item.Player
           select new XElement("Transfer",
              new XElement("Player", item.Player),
               new XElement("From_To", item.FROM_TO),
                new XElement("TransferSum", item.TransferSum)
                  
              )
          );

        XtaskA.Save(TaskA_path);


        // B) 
        var XtaskB = new XElement("TaskB",
            from item in taskA
            orderby item.Date descending
            select new XElement("Transfer",
               new XElement("Player", item.Player),
                new XElement("From_To", item.FROM_TO),
                 new XElement("TransferSum", item.TransferSum),
                 new XElement("Date", item.Date)

               )
          );

        XtaskB.Save(TaskB_path);

        // C)
        var XtaskC = new XElement("TaskC",
            from item in taskA
            group item by new { item.From } into g
            select new XElement("Club",
               new XElement("ClubNameSel", g.Key.From),
               from gr in g
                select new XElement("Players",
                 new XElement("Player", gr.Player)
                 
                 )
               )
          );

        XtaskC.Save(TaskC_path);






    }
}