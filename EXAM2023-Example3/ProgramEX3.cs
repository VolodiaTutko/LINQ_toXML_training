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

        string student_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example3\\Student.xml";
        var xmlStudent = XElement.Load(student_path);

        string group_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example3\\Group.xml";
        var xmlGroup = XElement.Load(group_path);

        string teachers_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example3\\Teacher.xml";
        var xmlTeacher = XElement.Load(teachers_path);

        string record_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example3\\Record.xml";
        var xmlRecord = XElement.Load(record_path);





        string TaskA_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example3\\TaskA.xml";
        string TaskB_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example3\\TaskB.xml";
        string TaskC_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example3\\TaskC.xml";
        string TaskD_path = "C:\\Users\\HP\\source\\repos\\EXAM2023-Example3\\TaskD.xml";



        var students = (from a in xmlStudent.Elements("Student")
                     select new
                     {
                         StudentID = (uint)a.Element("StudentID"),
                         StudentName = (string)a.Element("StudentName"),
                         StudentSurname = (string)a.Element("StudentSurname")
                        
                     }).ToList();

        var groups= (from a in xmlGroup.Elements("Group")
                        select new
                        {
                            GroupID = (uint)a.Element("GroupID"),
                            GroupName = (string)a.Element("GroupName")                           

                        }).ToList();

        var teachers = (from a in xmlTeacher.Elements("Teacher")
                        select new
                        {
                            TeacherID = (uint)a.Element("TeacherID"),
                            TeacherName = (string)a.Element("TeacherName"),
                            TeacherSurname = (string)a.Element("TeacherSurname")

                        }).ToList();


        var records = (from a in xmlRecord.Elements("Record")
                        select new
                        {
                            StudentID = (uint)a.Element("StudentID"),
                            GroupID = (uint)a.Element("GroupID"),
                            TeacherID = (uint)a.Element("TeacherID"),
                            Result = (uint)a.Element("Result")

                        }).ToList();

        Console.WriteLine(records.Count());

        // A) 
        var taskA = from rec in records
                    join stud in students on rec.StudentID equals stud.StudentID
                    join gr in groups on rec.GroupID equals gr.GroupID
                    join teach in teachers on rec.TeacherID equals teach.TeacherID
                    select new
                    {
                        Student = stud.StudentSurname + " " + stud.StudentName.FirstOrDefault() + ".",
                        Group = gr.GroupName,
                        Teacher = teach.TeacherSurname + " " + teach.TeacherName.FirstOrDefault() + ".",
                        Result = rec.Result
                    };

        var XtaskA = new XElement("TaskA",
          from item in taskA   
          orderby item.Student
          select new XElement("Student",
              new XElement("StudentSurname", item.Student),
               new XElement("Group", item.Group),
                new XElement("Teacher", item.Teacher),
                  new XElement("Result", item.Result)
              )
          );

        XtaskA.Save(TaskA_path);

        //B) 

        var XtaskB = new XElement("TaskB",
          from item in taskA
          group item by  new { item.Group } into g
          select new XElement("Group",
              new XElement("GroupName", g.Key.Group),
              from gr in g
              select new XElement("Student",
                new XElement("StudentSurname", gr.Student),
                  new XElement("Result", gr.Result)
              ))
          );

        XtaskB.Save(TaskB_path);


        // C) 
        var XtaskC = new XElement("TaskC",
          from item in taskA
          group item by new { item.Group } into g
          select new XElement("Group",
              new XElement("GroupName", g.Key.Group),
              new XElement("SumResult", g.Sum(item => item.Result)/g.Count()               
              ))
          );

        XtaskC.Save(TaskC_path);

        // D) 


        var taskD = from item in taskA   
                    group item by new {item.Teacher} into g
                    select new
                    {
                       Teacher = g.Key.Teacher,
                       Groups = from gr in g
                               group gr by new {gr.Group} into h
                               let successfulTests = h.Count(x => x.Result > 51)
                               let totalTests = h.Count()
                               let successRate = totalTests > 0 ? (double)successfulTests / totalTests * 100 : 0
                               orderby h.Key
                                select new
                                {
                                    Group = h.Key,
                                    SuccessRate = successRate
                                }


                    };


              var XtaskD = new XElement("TaskD",
              from item in taskD
              let minSuccessRate = item.Groups.Min(g => g.SuccessRate != null ? g.SuccessRate : double.MaxValue)
              select new XElement("Teacher",
                  new XElement("TeacherSurname", item.Teacher),
                  from groupItem in item.Groups
                  where (groupItem.SuccessRate != null ? groupItem.SuccessRate : double.MaxValue) == minSuccessRate
                  select new XElement("Group", groupItem.Group,
                      new XElement("Percent", groupItem.SuccessRate)
      )
  )
);




    }
}