using System;
using System.Collections.Generic;

namespace QueryTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Data d = new Data();
            User u1 = new User("John Dh", "oryory2@gmail.com", 31);
            User u2 = new User("John Doe", "oryory2@gmail.com", 2);
            User u3 = new User("John Doe", "oryory2@gmail.com", 29);
            Order o1 = new Order("A", 123);
            Order o2 = new Order("B", 123);

            d.addUser(u1);
            d.addUser(u2);
            d.addUser(u3);

            d.addOrder(o1);
            d.addOrder(o2);

            QueryEngine q = new QueryEngine(d);

            List<List<String>> queryAns1 = q.handleQuery("from Users where Age = 40 Or (Age < 30 AND Email = 'oryory2@gmail.com' AND Age = 29) AND FullName = 'John Doe' select Email, Age, FullName");
            List<List<String>> queryAns2 = q.handleQuery("from Orders where (FullName    =    \n    'A' OR FullName = 'C') AND (Id = 123 Or Id = 124) select FullName");
        }
    }
}
