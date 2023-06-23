using System.Net;
using System.Xml.Linq;

internal class Program30_05
{

    static void Main(string[] args)
    {

        string book_path = "C:\\Users\\HP\\source\\repos\\Tutko_30_05\\Book.xml";
        var xmlBook = XElement.Load(book_path);

        string genre_path = "C:\\Users\\HP\\source\\repos\\Tutko_30_05\\Genre.xml";
        var xmlGenre = XElement.Load(genre_path);

        string publisher_path = "C:\\Users\\HP\\source\\repos\\Tutko_30_05\\Publisher.xml";
        var xmlPublisher = XElement.Load(publisher_path);

        string result_path = "C:\\Users\\HP\\source\\repos\\Tutko_30_05\\Result.xml";
        var xmlResult = XElement.Load(result_path);


        string TaskA_path = "C:\\Users\\HP\\source\\repos\\Tutko_30_05\\TaskA.xml";
        string TaskB_path = "C:\\Users\\HP\\source\\repos\\Tutko_30_05\\TaskB.xml";
        string TaskC_path = "C:\\Users\\HP\\source\\repos\\Tutko_30_05\\TaskC.xml";
        string TaskD_path = "C:\\Users\\HP\\source\\repos\\Tutko_30_05\\TaskD.xml";



        var books = (from book in xmlBook.Elements("Book")
                         select new
                         {
                             BookID = (uint)book.Element("BookID"),
                             BookTilte = (string)book.Element("BookTitle"),
                             Author = (string)book.Element("Author")                            
                         }).ToList();


        var genres = (from g in xmlGenre.Elements("Genre")
                         select new
                         {
                             GenreID = (uint)g.Element("GenreID"),
                             GenreTitle = (string)g.Element("GenreTitle")
                            }).ToList();



        var publishers = (from p in xmlPublisher.Elements("Publisher")
                        select new
                        {
                            PublisherID = (string)p.Element("PublisherID"),
                            PublisherTitle= (string)p.Element("PublisherTitle")

                        }).ToList();



        var results = (from res in xmlResult.Elements("Result")
                       select new
                       {
                           
                           BookID = (uint)res.Element("BookID"),
                           GenreID = (uint)res.Element("GenreID"),
                           PublisherID = (string)res.Element("PublisherID"),
                           Data = (DateTime)res.Element("Date")

                       }).ToList();


       

        //A xml-файл, де iнформацiя про книги подана в такому виглядi:<прiзвище та iнiцiали автора, перелiк назв його книг
        //з вiдповiдними повними назвами видав-ництв>;вмiст впорядкувати у лексико-графiчному порядку за прiзвищем та назвою книги;

        var taskA = from result in results
                    join book in books on result.BookID equals book.BookID
                    join publisher in publishers on result.PublisherID equals publisher.PublisherID
                    orderby book.Author, book.BookTilte
                    group new { result, book, publisher } by new
                    {
                        book.Author
                    } into g
                    select new
                    {
                        Author = g.Key.Author,
                        Books = g.Select(item => new
                        {
                            BookTitle = item.book.BookTilte,
                            PublisherName = item.publisher.PublisherTitle
                        })
                    };

        var XtaskA = new XElement("TaskA",
            from item in taskA
            select new XElement("Books",
                new XElement("Author", item.Author),
                from w in item.Books
                select new XElement("Book",
                    new XElement("BookTitle", w.BookTitle),
                    new XElement("PublisherName", w.PublisherName)
                )
            )
        );

        XtaskA.Save(TaskA_path);

        // B xml-файл, описаний у попередньому завданнi, але для автора подати лише одну книжку – яка надiйшла останньою;
        //вмiст впорядкувати за зазначеною датою, починаючи вiд найближчої;


        var taskB = from result in results
                    join book in books on result.BookID equals book.BookID
                    join publisher in publishers on result.PublisherID equals publisher.PublisherID
                    orderby result.Data
                    group new { result, book, publisher } by new
                    {
                        book.Author
                    } into g
                    select new
                    {
                        Author = g.Key.Author,
                        Books = g.OrderByDescending(e => e.result.Data).Select(item => new
                        {
                            BookTitle = item.book.BookTilte,
                            PublisherName = item.publisher.PublisherTitle
                        }).ToList()
                    };


        var XtaskB = new XElement("TaskB",
            from item in taskB
              select new XElement("Books",
                new XElement("Author", item.Author),
                new XElement("Book",
                    new XElement("BookTitle", item.Books[0].BookTitle),
                    new XElement("PublisherName", item.Books[0].PublisherName)
                )
            )
        );

        XtaskB.Save(TaskB_path);


        //С xml-файл, в якому для кожного видавництва (заданого повною назвою) вказати перелiк авторiв отриманих книг;
        //вмiст впорядкувати у лексико-графiчному порядку за повною назвою видавництва, а також за прiзвищем;


        var taskC = from result in results
                    join book in books on result.BookID equals book.BookID
                    join publisher in publishers on result.PublisherID equals publisher.PublisherID
                    orderby publisher.PublisherTitle, book.Author
                    group new { result, book, publisher } by new
                    {
                        publisher.PublisherTitle
                    } into g
                    select new
                    {
                        PublisherName = g.Key.PublisherTitle,
                        Authors = g.Select(item => new
                        {
                            Author = item.book.Author

                        }).Distinct()
                    };

        var XtaskC = new XElement("TaskC",
           from item in taskC
           select new XElement("Publisher",
              new XElement("PublisherTitle", item.PublisherName),
              from w in item.Authors
              select new XElement("Author", w.Author
             
                  
              )
          )
       );

        XtaskC.Save(TaskC_path);




        // D xml-файл з перелiком назв жанрiв з найбiльшою кiлькiстю книг;
        // цi результати впорядкуватиза назвою жанру у лексико-графiчному порядку.

        var maxCount = (from result in results
                        join book in books on result.BookID equals book.BookID
                        join genre in genres on result.GenreID equals genre.GenreID
                        group new { result, book, genre } by new
                        {
                            genre.GenreTitle
                        } into g
                        orderby g.Count() descending
                        select g.Count()).FirstOrDefault();


        var taskD = (from result in results
                     join book in books on result.BookID equals book.BookID
                     join genre in genres on result.GenreID equals genre.GenreID
                     orderby genre.GenreTitle
                     group new { result, book, genre } by new
                     {
                         genre.GenreTitle
                     } into g
                     where g.Count() == maxCount
                     select new
                     {
                         GenreTitle = g.Key.GenreTitle,
                         Count = g.Count()
                     });

        var XtaskD = new XElement("TaskD",
            from item in taskD
            select new XElement("Genre",
                new XElement("GenreTitle", item.GenreTitle),
                new XElement("Count", item.Count)
            )
        );

        XtaskD.Save(TaskD_path);


    }
}