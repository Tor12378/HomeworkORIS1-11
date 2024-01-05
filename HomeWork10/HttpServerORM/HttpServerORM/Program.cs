using HttpServerORM;
using HttpServerORM.Models;
        
Console.WriteLine(PhrasesToStudents.Ex1("Тимур"));
Console.WriteLine();
Console.WriteLine(PhrasesToStudents.Ex2(new Student2{Address = "Ул.Тукая"}));
Console.WriteLine();
Console.WriteLine(PhrasesToStudents.Ex3(new Student3()));
Console.WriteLine();

var table = new Table();
table.Students.Add(new StudentForTable{Fio = "Тупаев Тимур" , Grade = 30});
table.Students.Add(new StudentForTable{Fio = "Козлов Руслан" , Grade = 45});


Console.WriteLine(PhrasesToStudents.Ex4(table));