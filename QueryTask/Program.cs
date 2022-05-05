using System;
using System.Collections.Generic;

namespace QueryTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Data d = new Data();
            User u1 = new User("x", "oryory2@gmailg.com", 31);
            User u2 = new User("John Doe", "oryory2@gmail.com", 30);
            User u3 = new User("John Doe", "oryory2@gmail.com", 29);
            Order o1 = new Order("A", 123);
            Order o2 = new Order("B", 123);


            d.AddUser(u1);
            d.AddUser(u2);
            d.AddUser(u3);

            d.AddOrder(o1);
            d.AddOrder(o2);

            QueryEngine q = new QueryEngine(d);

            List<List<String>> queryAns1 = q.HandleQuery("from Users where Age = 40      oR (Age <= 30 AND Email = 'oryory2@gmail.com' AND Age >= 29) AND FullName = 'John Doe' select Email, Age, FullName");
            List<List<String>> queryAns2 = q.HandleQuery("from Orders\nwhere (FullName\n    = 'A' oR FullName =\n        'C')       and (Id = 123 Or Id = 124) select FullName, Id");

        }
    }
}
