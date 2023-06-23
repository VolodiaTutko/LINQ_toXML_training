using FileStream_and_Linq.File_Stream_AND_Linq;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FileStream_and_Linq
{

    namespace File_Stream_AND_Linq
    {


        class Receipt
        {
            public DateTime Date { get; set; }
            public uint Computer_id { get; set; }
            public uint Os_Id
            {
                get;
                set;
            }
            public uint Amount { get; set; }



            public Receipt(DateTime date = new DateTime(), uint computer_id = 0, uint os_Id = 0, uint amount = 0)
            {
                Date = date;
                Computer_id = computer_id;
                Os_Id = os_Id;
                Amount = amount;
            }

            public string ToString()
            {
                return $"DateTime: {Date}, Computerid: {Computer_id}, Os_id: {Os_Id}, Amount: {Amount}";
            }
            public List<Receipt> ReadReceiptFromFile(string path)
            {
                var receipts = new List<Receipt>();

                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var parts = line.Split(',');
                        var date = DateTime.Parse(parts[0]);
                        var computer_id = uint.Parse(parts[1]);
                        var os_id = uint.Parse(parts[2]);
                        var amount = uint.Parse(parts[3]);


                        var receipt = new Receipt(date, computer_id, os_id, amount);
                        receipts.Add(receipt);
                    }
                }

                return receipts;
            }


        }
    }

    class OS
    { 
            
              
            public OS(uint os_Id = 0, string name = "", uint price = 0)
            {
                Os_Id = os_Id;
                Name = name;
                Price = price;
            }

            public uint Os_Id { get; set; }
            public string Name { get; set; }

            public uint price;

            public uint Price
        {
            get { return price; }
            set
            {
                if (Os_Id != 0)
                {
                    price = value;
                }
                else
                {
                    price = 0;
                }
            }
        }
            public string ToString()
            {
                return $"Os_id: {Os_Id}, Name: {Name}, Price: {Price}";
            }

            public List<OS> ReadOSFromFile(string path)
            {
                var oses = new List<OS>();

                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var parts = line.Split(',');
                        var os_id  = uint.Parse(parts[0]);
                        var name = parts[1];
                        var price = uint.Parse(parts[2]);
                       

                        var os = new OS(os_id, name, price);
                        oses.Add(os);
                    }
                }

                return oses;
            }
        
    }

      class Computer
        {

            public uint Computer_Id { get; set; }
            public string Marka { get; set; }
            public uint Price { get; set; }


            public Computer(uint computer_Id = 0, string marka = "", uint price = 0)
            {
                Computer_Id = computer_Id;
                Marka = marka;
                Price = price;
                
            }
            

            public string ToString()
            {
                return $"Computer_id: {Computer_Id}, Marka: {Marka}, Price: {Price}";
            }

            public List<Computer> ReadComputerFromFile(string path)
            {
                var computers = new List<Computer>();

                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var parts = line.Split(',');
                        var computer_id = uint.Parse(parts[0]);
                        var marka = parts[1];
                        var price = uint.Parse(parts[2]);


                        var computer = new Computer(computer_id, marka, price);
                        computers.Add(computer);
                    }
                }

                return computers;
            }

        }


       
    
    class Program_18_04
    {

        static void Main(string[] args)
        {
           // var DEFOLTED_OS = new OS(0, "", 0);

            string pathtoOScsv = @"C:\Users\HP\source\repos\ProgramingLab18-04\OS.csv";
            string pathtoComputercsv = @"C:\Users\HP\source\repos\ProgramingLab18-04\Computer.csv";

            string pathtoReceipt1csv = @"C:\Users\HP\source\repos\ProgramingLab18-04\Receipt1.csv";
            string pathtoReceipt2csv = @"C:\Users\HP\source\repos\ProgramingLab18-04\Receipt2.csv";
            string pathtoReceipt3csv = @"C:\Users\HP\source\repos\ProgramingLab18-04\Receipt3.csv";


            

            var os = new OS();
            var computer = new Computer();
            var receipt = new Receipt();
            
           
            var os_list = os.ReadOSFromFile(pathtoOScsv);
            var computer_list = computer.ReadComputerFromFile(pathtoComputercsv);

            var receipts1 = receipt.ReadReceiptFromFile(pathtoReceipt1csv);
            var receipts2 = receipt.ReadReceiptFromFile(pathtoReceipt2csv);
            var receipts3 = receipt.ReadReceiptFromFile(pathtoReceipt3csv);

            var all_receipts = receipts1.Concat(receipts2).Concat(receipts3);


            //foreach (var i in receipts3)
            //{
            //    Console.WriteLine(i.Os_Id);
            //}


            // A) сумарну вартiсть проданої за весь час комп’ютерної технiки;

            var taskA = from rec in all_receipts
                        join com in computer_list on rec.Computer_id equals com.Computer_Id
                        join Os in os_list on rec.Os_Id equals Os.Os_Id
                        group new { rec, com, Os } by new { rec.Computer_id } into g
                        from gr in g

                        select new
                        {
                            SumComp = g.Sum(item => item.rec.Amount * item.com.Price),
                            SumOs = g.Sum(item => item.rec.Amount * item.Os.Price)
                            
                        };
                        


            var sum = taskA.Sum(item => item.SumComp)+ taskA.Sum(item => item.SumOs);

            Console.WriteLine($"TASK A:");
            Console.WriteLine($"ALL SUM OF MONEY : {sum}\n");


            // B) кiлькiсть проданих комп’ютерiв кожної марки;

            var taskB = (from rec in all_receipts
                         join com in computer_list on rec.Computer_id equals com.Computer_Id
                         join Os in os_list on rec.Os_Id equals Os.Os_Id
                         group new { rec, com, Os } by new { com.Marka } into g
                         from gr in g

                         select new
                         {
                             ComputerMarka = g.Key.Marka,
                             Amount = g.Sum(item => item.rec.Amount)
                         }).Distinct();
            Console.WriteLine($"TASK B:");
            foreach (var i in taskB )
            {
                Console.WriteLine($"Marka: {i.ComputerMarka}, Amount: {i.Amount}");
            }
            Console.WriteLine($"\n");



            // C)  вартiсть комп’ютерiв, проданих кожного дня;

            var taskC = (from rec in all_receipts
                         join com in computer_list on rec.Computer_id equals com.Computer_Id
                         join Os in os_list on rec.Os_Id equals Os.Os_Id
                         group new { rec, com, Os } by new { rec.Date } into g
                         from gr in g

                         select new
                         {
                             Date = g.Key.Date,
                             Money = g.Sum(item => item.rec.Amount * item.com.Price)+ g.Sum(item => item.rec.Amount * item.Os.Price)
                             
                         });

           

            Console.WriteLine($"TASK C:");
            foreach (var i in taskC)
            {
                Console.WriteLine($"Date: {i.Date}, Sum of money: {i.Money}");
            }
            Console.WriteLine($"\n");


            // D для кожної операцiйної системи сумарну вартiсть проданих комп’ютерiв з цiєю системою.

            var taskD = (from rec in all_receipts
                         join com in computer_list on rec.Computer_id equals com.Computer_Id
                         join Os in os_list on rec.Os_Id equals Os.Os_Id
                         group new { rec, com, Os } by new {Os.Name } into g
                         from gr in g

                         select new
                         {
                             OS = g.Key.Name,
                             Money = g.Sum(item => item.rec.Amount * item.com.Price) + g.Sum(item => item.rec.Amount * item.Os.Price)

                         });


            Console.WriteLine($"TASK D:");
            foreach (var i in taskD)
            {
                Console.WriteLine($"OS: {i.OS}, Sum of money: {i.Money}");
            }
            Console.WriteLine($"\n");


        }
    }
}