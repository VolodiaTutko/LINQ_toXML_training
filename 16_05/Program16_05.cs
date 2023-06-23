using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace _16_05pr
{
    internal class Program16_05
    {

        static void Main(string[] args)
        {

            string Students = "C:\\Users\\HP\\source\\repos\\Tutko_16_05\\students.xml";
            var xmlStudents = XElement.Load(Students);

            string Teachers = "C:\\Users\\HP\\source\\repos\\Tutko_16_05\\teachers.xml";
            var xmlTeachers = XElement.Load(Teachers);

            string ResultProgramming = "C:\\Users\\HP\\source\\repos\\Tutko_16_05\\ResultProgramming.xml";
            var xmlResultProgramming = XElement.Load(ResultProgramming);

            string ResultMath = "C:\\Users\\HP\\source\\repos\\Tutko_16_05\\ResultMath.xml";
            var xmlResultMath = XElement.Load(ResultMath);

            string ResultEnglish = "C:\\Users\\HP\\source\\repos\\Tutko_16_05\\ResultEnglish.xml";
            var xmlResultEnglish = XElement.Load(ResultEnglish);

            string taskA_path = "C:\\Users\\HP\\source\\repos\\Tutko_16_05\\TaskA.xml";
            string taskB_path = "C:\\Users\\HP\\source\\repos\\Tutko_16_05\\TaskB.xml";
            string taskC_path = "C:\\Users\\HP\\source\\repos\\Tutko_16_05\\TaskC.xml";
            string taskD_path = "C:\\Users\\HP\\source\\repos\\Tutko_16_05\\TaskD.xml";


            var students = (from st in xmlStudents.Elements("Student")
                            select new
                            {
                                StudentID = (uint)st.Element("StudentID"),
                                StudentName = (string)st.Element("StudentName"),
                                StudentSurname = (string)st.Element("StudentSurname"),
                                Group = (string)st.Element("GroupName")
                            }).ToList();

            var teachers = (from t in xmlTeachers.Elements("Teacher")
                            select new
                            {
                                TeacherID = (uint)t.Element("TeacherID"),
                                TeacherName = (string)t.Element("TeacherName"),
                                TeacherSurname = (string)t.Element("TeacherSurname"),
                            }).ToList();


            var math = (from m in xmlResultMath.Elements("Discipline")
                        from s in m.Elements("ResultStudents").Elements("ResultStudent")
                        select new
                        {
                            DisciplineName = (string)m.Element("DisciplineName"),
                            TeacherID = (uint)m.Element("TeacherID"),
                            StudentID = (uint)s.Element("StudentID"),
                            Mark = (uint)s.Element("Mark")
                        }).ToList();


            var english = (from m in xmlResultEnglish.Elements("Discipline")
                           from s in m.Elements("ResultStudents").Elements("ResultStudent")
                           select new
                           {
                               DisciplineName = (string)m.Element("DisciplineName"),
                               TeacherID = (uint)m.Element("TeacherID"),
                               StudentID = (uint)s.Element("StudentID"),
                               Mark = (uint)s.Element("Mark")
                           }).ToList();


            var programming = (from m in xmlResultProgramming.Elements("Discipline")
                               from s in m.Elements("ResultStudents").Elements("ResultStudent")
                               select new
                               {
                                   DisciplineName = (string)m.Element("DisciplineName"),
                                   TeacherID = (uint)m.Element("TeacherID"),
                                   StudentID = (uint)s.Element("StudentID"),
                                   Mark = (uint)s.Element("Mark")
                               }).ToList();


            var all_results = math.Concat(programming).Concat(english);



            // A)  xml-файл, де результати систематизованi за схемою <назва дисциплiни, прiзвище та iнiцiали викладача, назва групи,
            // перелiк результатiв у виглядi пар <прiзвище та iнiцiали студента,кiлькiсть балiв>;
            // вмiст впорядкувати у лексико-графiчному порядку за назвою дисциплiни,назвою групи i прiзвищем студента;

            var taskA = from result in all_results
                        join teacher in teachers on result.TeacherID equals teacher.TeacherID
                        join student in students on result.StudentID equals student.StudentID
                        group new { result, teacher, student } by new
                        {
                            result.DisciplineName,
                            teacher.TeacherName,
                            teacher.TeacherSurname,
                            student.Group
                        } into g
                        select new
                        {
                            DisciplineName = g.Key.DisciplineName,
                            Teacher = g.Key.TeacherName + " " + g.Key.TeacherSurname,
                            Group = g.Key.Group,
                            Student = g.Select(item => new
                            {
                                StudentName = item.student.StudentSurname + " " + item.student.StudentName,
                                Mark = item.result.Mark
                            })
                        };



            var XTaskA = new XElement("TaskA",
                from item in taskA
                orderby item.DisciplineName, item.Group
                group item by new
                {
                    item.DisciplineName,
                    item.Teacher
                } into g

                select new XElement("Discipline",
                    new XElement("DisciplineName", g.Key.DisciplineName),
                    new XElement("Teacher", g.Key.Teacher),
                    from grp in g
                    orderby grp.Group
                    select new XElement("Group",
                        new XElement("GroupName", grp.Group),
                        from student in grp.Student
                        orderby student.StudentName
                        select new XElement("Student",
                            new XElement("StudentName", student.StudentName),
                            new XElement("Mark", student.Mark)
                        )
                    )
                )
            );

            XTaskA.Save(taskA_path);


            // B) xml-файл, де результати систематизованi за схемою < назва групи, перелiк результатiв у виглядi <прiзвище та iнiцiали студента>
            // та пари <назва дисциплiни, кiлькiсть балiв>;
            // вмiствпорядкувати у лексико-графiчному порядку за назвою групи i прiзвищем студента;


            var taskB = from result in all_results
                        join teacher in teachers on result.TeacherID equals teacher.TeacherID
                        join student in students on result.StudentID equals student.StudentID
                        group new { result, teacher, student } by new
                        {
                            student.Group,
                            student.StudentSurname,
                            student.StudentName,
                            result.DisciplineName
                        } into g
                        select new
                        {
                            Group = g.Key.Group,
                            Student = g.Key.StudentSurname + " " + g.Key.StudentName,
                            Discipline = g.Key.DisciplineName,
                            Mark = g.First().result.Mark
                        };

            var XTaskB = new XElement("TaskB",
                from item in taskB
                orderby item.Group
                group item by new
                {
                    item.Group
                } into g
                select new XElement("Groups",
                    new XElement("GroupName", g.Key.Group),
                    from gr in g
                    orderby gr.Student
                    group gr by new
                    {
                        gr.Student
                    } into sg
                    select new XElement("Students",
                        new XElement("Student", sg.Key.Student,
                            from s in sg
                            group s by new
                            {
                                s.Discipline
                            } into ds
                            select new XElement("Disciplines",
                                new XElement("DisciplineName", ds.Key.Discipline),
                                new XElement("Mark", ds.First().Mark)
                            )
                        )
                    )
                )
            );

            XTaskB.Save(taskB_path);



            // С) xml-файл, описаний у попередньому завданнi, але без врахування студентiв з незадовiльнимибалами (меншими 51);

            var taskC = from result in all_results
                        join teacher in teachers on result.TeacherID equals teacher.TeacherID
                        join student in students on result.StudentID equals student.StudentID
                        group new { result, teacher, student } by new
                        {
                            student.Group,
                            student.StudentSurname,
                            student.StudentName,
                            result.DisciplineName
                        } into g
                        select new
                        {
                            Group = g.Key.Group,
                            Student = g.Key.StudentSurname + " " + g.Key.StudentName,
                            Discipline = g.Key.DisciplineName,
                            Mark = g.First().result.Mark
                        };

            var XTaskC = new XElement("TaskC",
                from item in taskC
                orderby item.Group
                group item by new
                {
                    item.Group
                } into g
                select new XElement("Groups",
                    new XElement("GroupName", g.Key.Group),
                    from gr in g
                    orderby gr.Student
                    group gr by new
                    {
                        gr.Student
                    } into sg
                    where sg.All(s => s.Mark > 51)
                    select new XElement("Students",
                        new XElement("Student", sg.Key.Student,
                            from s in sg
                            group s by new
                            {
                                s.Discipline
                            } into ds
                            select new XElement("Disciplines",
                                new XElement("DisciplineName", ds.Key.Discipline),
                                new XElement("Mark", ds.First().Mark)
                            )
                        )
                    )
                )
            );

            XTaskC.Save(taskC_path);



            // D) xml-файл, в якому подано рейтинг студентiв за сумарною кiлькiстю балiв з усiх дисциплiн
            // без врахування студентiв з незадовiльними балами.

            var taskD = from result in all_results
                                 join teacher in teachers on result.TeacherID equals teacher.TeacherID
                                 join student in students on result.StudentID equals student.StudentID
                                 group new { result, teacher, student } by new
                                 {
                                     student.StudentID,
                                     student.StudentSurname,
                                     student.StudentName
                                 } into g
                                 where g.All(item => item.result.Mark >= 51)
                                 let totalMarks = g.Sum(item => item.result.Mark)
                                 orderby totalMarks descending
                                 select new
                                 {
                                     StudentID = g.Key.StudentID,
                                     Student = g.Key.StudentSurname + " " + g.Key.StudentName,
                                     TotalMarks = totalMarks
                                 };

            var XtaskD = new XElement("StudentRanking",
                from item in taskD
                select new XElement("Student",
                    new XElement("StudentID", item.StudentID),
                    new XElement("FullName", item.Student),
                    new XElement("TotalMarks", item.TotalMarks)
                )
            );

            XtaskD.Save(taskD_path);

        }

    }
}
