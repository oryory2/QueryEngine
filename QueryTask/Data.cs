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
            this.Users = new List<User>();
            this.Orders = new List<Order>();
        }

        public void addUser(User u)
        {
            this.Users.Add(u);
            Console.WriteLine("User: '" + u.getName() + "' has been added to the DataBase");
        }

        public void addOrder(Order o)
        {
            this.Orders.Add(o);
            Console.WriteLine("Order: '" + o.getName() + "' has been added to the DataBase");
        }

        public List<User> getUsers()
        {
            return this.Users;
        }

        public List<Order> getOrders()
        {
            return this.Orders;
        }
    }
}
