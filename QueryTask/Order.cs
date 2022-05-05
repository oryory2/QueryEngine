using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTask
{
    class Order
    {
        public string fullName;
        public int id;


        public Order(string fullName, int id)
        {
            this.fullName = fullName;
            this.id = id;
        }

        public string GetName()
        {
            return fullName;
        }

        public int GetId()
        {
            return id;
        }


        public int BooleanExpressionCheck(string field, string oper, string val) // Function for cheking a boolean expression (0 - false, 1 - true, 2 - wrong input)
        {
            switch (field)
            {
                case ("FullName"):
                    return CheckName(val);
                case ("Id"):
                    return CheckId(val);
                default:
                    return 2;
            }
        }
        private int CheckName(string val) // 0 - false, 1 - true
        {
            if (GetName().Equals(val))
                return 1; 
            else
                return 0;
        }

        private int CheckId(string val) // 0 - false, 1 - true
        {
            if (GetId() == Int32.Parse(val))
                return 1;
            else
                return 0;
        }

        public List<string> GetFields(string field, List<string> currUserList) // Function for getting the needed fields by the "select" section
        {
            List<string> defualtL = new List<string>();

            switch (field)
            {
                case ("FullName"):
                    currUserList.Add(GetName());
                    break;
                case ("Id"):
                    currUserList.Add(GetId().ToString());
                    break;
                default:
                    return defualtL;
            }
            return currUserList;
        }
    }
}
