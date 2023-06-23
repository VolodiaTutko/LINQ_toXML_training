using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

internal class Program_02_05
{

    static void Main(string[] args)
    {

        string categories_path = "C:\\Users\\HP\\source\\repos\\Tutko_02.05\\categories.xml";
        var xmlCategory = XElement.Load(categories_path);

        string operations_path = "C:\\Users\\HP\\source\\repos\\Tutko_02.05\\operations.xml";
        var xmlOperation = XElement.Load(operations_path);

        string receipts_path = "C:\\Users\\HP\\source\\repos\\Tutko_02.05\\service_receptions.xml";
        var xmlReceipt = XElement.Load(receipts_path);




        string TaskA_path = "C:\\Users\\HP\\source\\repos\\Tutko_02.05\\taskA.xml";
        string TaskB_path = "C:\\Users\\HP\\source\\repos\\Tutko_02.05\\taskB.xml";
        string TaskC_path = "C:\\Users\\HP\\source\\repos\\Tutko_02.05\\taskC.xml";




        var category = (from categor in xmlCategory.Elements("Category")
                        select new
                        {
                            CategoryID = (uint)categor.Element("CategoryID"),
                            CategoryName = (string)categor.Element("CategoryName"),
                            WarrantyMonth = (uint)categor.Element("WarrantyMonths")
                        }).ToList();


        var operation = (from op in xmlOperation.Elements("Operation")
                         select new
                         {
                             OperationID = (uint)op.Element("OperationID"),
                             OperationName = (string)op.Element("OperationName"),
                             Price = (uint)op.Element("Price"),
                         }).ToList();



        var receipt = (from rec in xmlReceipt.Elements("ServiceReceipt")
                       select new
                       {
                           CategoryID = (uint)rec.Element("CategoryID"),
                           ManufactoreDate = (DateTime)rec.Element("ManufactureDate"),
                           ServiceDate = (DateTime)rec.Element("ServiceDate"),
                           OperationID = (uint)rec.Element("OperationID")



                       }).ToList();

        
        // A)xml-файл, де для кожної категорiї виробiв (впорядкування у лексико-графiчному порядку)
        // вка-зати кiлькiсть кожної з виконаних операцiй у форматi <назва операцiї: кiлькiсть>,
        // цей перелiквпорядкувати у спадному порядку за кiлькiстю;


        var taskA = from rec in receipt
                    join cat in category on rec.CategoryID equals cat.CategoryID
                    join op in operation on rec.OperationID equals op.OperationID
                    group new { rec, op, cat } by new { cat.CategoryName } into g
                    select new
                    {
                        CategoryName = g.Key.CategoryName,
                        Operations = g.Select(item => new
                        {
                            OperationName = item.op.OperationName,
                            Price = item.op.Price

                        })
                    };


        var xmlTaskA = new XElement("TaskA",
            from item in taskA
            orderby item.CategoryName
            select new XElement("Category",
             new XElement("CategoryName", item.CategoryName),
            from it in item.Operations
            group it by it.OperationName into g
            orderby g.Count() descending
            select new XElement("OperationName", g.Key,
                new XElement("Count", g.Count())
            )
            )
        ); 
        xmlTaskA.Save(TaskA_path);



        // B) xml-файл, де для кожної категорiї виробiв (впорядкування у лексико-графiчному порядку)
        // вка-зати зароблену суму по кожнiй операцiї <назва операцiї: сума>,
        // цей перелiк впорядкований успадному порядку стосовно суми;

        var xmlTaskB = new XElement("TaskB",
           from item in taskA
           orderby item.CategoryName
           select new XElement("Category",
            new XElement("CategoryName", item.CategoryName),
           from it in item.Operations
           group it by new { it.OperationName, it.Price } into g
           orderby g.Count()*g.Key.Price descending
           select new XElement("OperationName", g.Key.OperationName,
               new XElement("Sum", g.Count()*g.Key.Price)
           )
           )
       ); 
        xmlTaskB.Save(TaskB_path);



        //C) xml-файл, де для заданої категорiї виробiв вказати кiлькiсть виконаних операцiй для виробiв на гарантiї,
        //перелiк впорядкований у спадному порядку за кiлькiстю.

        var taskC = from rec in receipt
                    join cat in category on rec.CategoryID equals cat.CategoryID
                    join op in operation on rec.OperationID equals op.OperationID
                    group new { rec, op, cat } by new { cat.CategoryName } into g
                    select new
                    {
                        CategoryName = g.Key.CategoryName,
                        OperationCount = g.Count(item => (item.rec.ServiceDate - item.rec.ManufactoreDate).Days <= item.cat.WarrantyMonth * 30)
                    };

        var xmlTaskC = new XElement("TaskC",
            from item in taskC
            orderby item.OperationCount descending
            select new XElement("Category",
                new XElement("CategoryName", item.CategoryName),
                new XElement("Amount", item.OperationCount)
            )
        );
        xmlTaskC.Save(TaskC_path);

    }
}