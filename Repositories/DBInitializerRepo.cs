using ASPNETCore_DB.Data;
using ASPNETCore_DB.Interfaces;
using ASPNETCore_DB.Models;
using System;
using System.Linq;

namespace ASPNETCore_DB.Repositories
{
    public class DBInitializerRepo : IDBInitializer
    {
        public void Initialize(SQLiteDBContext context)
        {
            context.Database.EnsureCreated();
            if (context.Students.Any())
            {
                return;
            }

            var students = new Student[]
            {
                new Student{StudentNumber="2021000001",FirstName="Alexander",Surname = "May",
                EnrollmentDate=DateTime.Parse("2021-02-03"),Photo="DefaultPic.png",Email="DefaultEmail@gmail.com"},
                new Student{StudentNumber="2012000002",
                FirstName="Meredith",Surname="Alonso",EnrollmentDate=DateTime.Parse("2021-02-01"),Photo="DefaultPic.png",Email="DefaultEmail@gmail.com"},
                new Student{StudentNumber="2021000003",
                FirstName="Arturo",Surname="Anand",EnrollmentDate=DateTime.Parse("2021-02-04"),Photo="DefaultPic.png",Email="DefaultEmail@gmail.com"}
            };
            foreach (Student s in students)
            {
                context.Students.Add(s);
            }

            var consumers = new Consumer[]
            {
    new Consumer{ConsumerId="90909",Name="tshego",
    Email="tshego@example.com",Photo="DefaultPic.png",Address="66 kerk st",Phone="0795547786",RegistrationDate=DateTime.Parse("2021-02-03")},
    new Consumer{ConsumerId="90909",
    Name="Misper",Email="misper@example.com",Photo="DefaultPic.png",Address="45 Oaktree village",Phone="0795547786",RegistrationDate=DateTime.Parse("2021-02-01")},
    new Consumer{ConsumerId="77777",
    Name="nadio",Email="nadio@example.com",Photo="DefaultPic.png",Address="789 parkhof willows",Phone="0795547786",RegistrationDate=DateTime.Parse("2021-02-04")}
            };
            foreach (Consumer c in consumers)
            {
                context.Consumers.Add(c);
            }
            context.SaveChanges();

            context.SaveChanges();
        }
    }
}