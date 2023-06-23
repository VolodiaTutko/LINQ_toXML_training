using System.Threading.Tasks;
using System.Xml.Linq;

internal class Program23_05
{

    static void Main(string[] args)
    {

        string employee_path = "C:\\Users\\HP\\source\\repos\\Tutko_23.05\\employee.xml";
        var xmlEmployee = XElement.Load(employee_path);

        string position_path = "C:\\Users\\HP\\source\\repos\\Tutko_23.05\\position.xml";
        var xmlPosition = XElement.Load(position_path);

        string project_path = "C:\\Users\\HP\\source\\repos\\Tutko_23.05\\project.xml";
        var xmlProject = XElement.Load(project_path);

        string report_path = "C:\\Users\\HP\\source\\repos\\Tutko_23.05\\report.xml";
        var xmlReport = XElement.Load(report_path);

        string TaskA_path = "C:\\Users\\HP\\source\\repos\\Tutko_23.05\\taskA.xml";
        string TaskB_path = "C:\\Users\\HP\\source\\repos\\Tutko_23.05\\taskB.xml";
        string TaskC_path = "C:\\Users\\HP\\source\\repos\\Tutko_23.05\\taskC.xml";
        string TaskD_path = "C:\\Users\\HP\\source\\repos\\Tutko_23.05\\taskD.xml";



        var employees = (from emp in xmlEmployee.Elements("employee")
                         select new
                         {
                             Employee_id = (uint)emp.Element("emp_id"),
                             Surname = (string)emp.Element("surname"),
                             Name = (string)emp.Element("name"),
                             Department = (string)emp.Element("department"),
                             Position_id = (uint)emp.Element("position_id")
                         }).ToList();


        var positions = (from pos in xmlPosition.Elements("position")
                         select new
                         {
                             Position_id = (uint)pos.Element("position_id"),
                             Position_title = (string)pos.Element("position_title"),
                             Pay_per_hour = (uint)pos.Element("pay_per_hour")

                         }).ToList();



        var projects = (from pro in xmlProject.Elements("project")
                        select new
                        {
                            Project_id = (string)pro.Element("projectID"),
                            Project_title = (string)pro.Element("project_title")

                        }).ToList();



        var reports = (from rep in xmlReport.Elements("report")
                       select new
                       {
                           Data = (DateTime)rep.Element("Date"),
                           Employee_id = (uint)rep.Element("emp_id"),
                           Project_id = (string)rep.Element("projectID"),
                           Total_hours = (uint)rep.Element("total_hours"),
                           Describe = (string)rep.Element("describe_work")
                       }).ToList();


        //(а) xml-файл, де звiти систематизованi за схемою <назва проєкту, перелiк прiзвищ (з iнiцiалами) працiвникiв разом iз сумарною кiлькiстю годин,вiдпрацьованих кожним з них>;
        // вмiст впорядкувати у лексико-графiчному порядку за назвою проєкту i прiзвищем працiвникiв;
        var taskA = from report in reports
                    join employee in employees on report.Employee_id equals employee.Employee_id
                    join project in projects on report.Project_id equals project.Project_id
                    orderby project.Project_title, employee.Surname
                    group report by new
                    {
                        project.Project_title,

                    } into g
                    select new
                    {
                        Project_title = g.Key.Project_title,
                        Employees = g.GroupBy(e => e.Employee_id).Select(g =>
                            new
                            {
                                Employee = employees.FirstOrDefault(s => s.Employee_id == g.Key),
                                Total_hours = g.Sum(r => r.Total_hours),
                            })
                    };











        var XTaskA = new XElement("TaskA",
                from item in taskA
                select new XElement("Project",
                new XElement("ProjectName", item.Project_title),
                from w in item.Employees
                select new XElement("Worker",
                new XElement("Name", w.Employee.Name),
                new XElement("Surname", w.Employee.Surname),
                new XElement("WorkedHours", w.Total_hours)
                )));

        XTaskA.Save(TaskA_path);


        //б) xml-файл, описаний у попередньому завданнi, але подати крiм вiдпрацьованих годин ще й за-роблену суму грошей; 

        var taskB = from report in reports
                    join employee in employees on report.Employee_id equals employee.Employee_id
                    join project in projects on report.Project_id equals project.Project_id
                    join position in positions on employee.Position_id equals position.Position_id
                    orderby project.Project_title, employee.Surname
                    group new { report, employee, project, position } by new
                    {
                        project.Project_title
                    } into g
                    select new
                    {
                        Project_title = g.Key.Project_title,
                        Employees = g.GroupBy(e => e.employee.Employee_id).Select(g =>
                            new
                            {
                                Employee = employees.FirstOrDefault(s => s.Employee_id == g.Key),
                                Total_Hours = (uint)g.Sum(r => r.report.Total_hours),
                                Total_Salary = (uint)g.Sum(r => r.report.Total_hours) * (uint)g.First().position.Pay_per_hour
                            })
                    };

        var XTaskB = new XElement("TaskB",
            from item in taskB
            select new XElement("Project",
                new XElement("ProjectName", item.Project_title),
                from w in item.Employees
                select new XElement("Worker",
                    new XElement("Name", w.Employee.Name),
                    new XElement("Surname", w.Employee.Surname),
                    new XElement("WorkedHours", w.Total_Hours),
                    new XElement("Salary", w.Total_Salary)
                )
            )
        );

        XTaskB.Save(TaskB_path);




        // c xml-файл, в якому для кожного проєкту(заданого iдентифiкатором)
        // вказати сумарний час ро-боти над ним працiвниками вiдповiдних посад;
        //вмiст впорядкувати у лексико-графiчному порядку за iдентифiкатором проєкту;


        var taskC = from report in reports
                    join employee in employees on report.Employee_id equals employee.Employee_id
                    join project in projects on report.Project_id equals project.Project_id
                    join position in positions on employee.Position_id equals position.Position_id
                    orderby project.Project_id
                    group new { report, employee, project, position } by new
                    {
                        project.Project_id
                    } into g
                    select new
                    {
                        ProjectID = g.Key.Project_id,
                        Positions_sum_hours = g.GroupBy(e => e.position.Position_id).Select(g =>
                            new
                            {
                                Position = positions.FirstOrDefault(s => s.Position_id == g.Key),
                                Total_Hours = (uint)g.Sum(r => r.report.Total_hours),
                                })
                            };

        var XTaskC = new XElement("TaskC",
            from item in taskC
            select new XElement("Project",
                new XElement("ProjectID", item.ProjectID),
                from w in item.Positions_sum_hours
                select new XElement("Positon",
                new XElement("PositionTitle", w.Position.Position_title),
                new XElement("SumHours", w.Total_Hours ))

                )
             );

        XTaskC.Save(TaskC_path);




        // D  xml-файл, де для кожного проєкту(заданого iдентифiкатором) вказати сумарний час роботи над ним i освоєну суму грошей;
        // цi результати впорядкувати за сумарним часом у спадному порядку.


        var subtask = from report in reports
                    join employee in employees on report.Employee_id equals employee.Employee_id
                    join project in projects on report.Project_id equals project.Project_id
                    join position in positions on employee.Position_id equals position.Position_id
                    orderby project.Project_title, employee.Surname
                    group new { report, employee, project, position } by new
                    {
                        project.Project_id
                    } into g
                    select new
                    {
                        ProjectID = g.Key.Project_id,
                        Employees = g.GroupBy(e => e.employee.Employee_id).Select(g =>
                            new
                            {
                                Employee = employees.FirstOrDefault(s => s.Employee_id == g.Key),
                                Total_Hours = (uint)g.Sum(r => r.report.Total_hours),
                                Total_Salary = (uint)g.Sum(r => r.report.Total_hours) * (uint)g.First().position.Pay_per_hour
                            })
                    };
        var taskD = from report in reports
                    join employee in employees on report.Employee_id equals employee.Employee_id
                    join project in projects on report.Project_id equals project.Project_id
                    join position in positions on employee.Position_id equals position.Position_id
                    orderby project.Project_id
                    group new { report, employee, project, position } by new
                    {
                        project.Project_id
                    } into g
                    select new
                    {
                        ProjectID = g.Key.Project_id,
                        SumHoures = (uint)g.Sum(r => r.report.Total_hours),
                          };

       




                var XTaskD = new XElement("TaskD",
                    from item in taskD
                    orderby item.SumHoures descending
                    select new XElement("Project",
                        new XElement("ProjectID", item.ProjectID),
                        new XElement("SumHours", item.SumHoures),
                    from w in subtask
                    where w.ProjectID == item.ProjectID
                    select new XElement("SumMoney",w.Employees.Sum(r => r.Total_Salary))

                        )
                     );

                XTaskD.Save(TaskD_path);







    }
}







       