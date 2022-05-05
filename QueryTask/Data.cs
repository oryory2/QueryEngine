using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTask
{
    class Data
    {
        public List<User> Users;
        public List<Order> Orders;

        public Data()
        {
            Users = new List<User>();
            Orders = new List<Order>();
        }

        public void AddUser(User u)
        {
            Users.Add(u);
            Console.WriteLine("User: '" + u.GetName() + "' has been added to the DataBase");
        }

        public void AddOrder(Order o)
        {
            Orders.Add(o);
            Console.WriteLine("Order: '" + o.GetName() + "' has been added to the DataBase");
        }

        public List<Object> GetUsers()
        {
            return Users.Cast<object>().ToList(); ;
        }

        public List<Object> GetOrders()
        {
            return Orders.Cast<object>().ToList(); ;
        }
    }
}
