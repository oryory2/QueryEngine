using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTask
{
    class Order
    {
        public string name;
        public int id;


        public Order(String fullName, int id)
        {
            this.name = fullName;
            this.id = id;
        }

        public String getName()
        {
            return this.name;
        }

        public int getId()
        {
            return this.id;
        }


        public int booleanExpressionCheck(String field, String oper, String val) // Function for cheking a boolean expression (0 - false, 1 - true, 2 - wrong input)
        {
            switch (field)
            {
                case ("FullName"):
                    if (this.getName().Equals(val))
                        return 1;
                    else
                        return 0;

                case ("Id"):
                    if (oper == "=")
                    {
                        if (this.getId() == Int32.Parse(val))
                            return 1;
                        else
                            return 0;
                    }
                    break;
                default:
                    return 2; // wrong input field
            }
            return 2;
        }



        public List<String> getFields(String field, List<String> currUserList) // Function for getting the needed fields by the "select" section
        {
            List<String> defualtL = new List<String>();

            switch (field)
            {
                case ("FullName"):
                    currUserList.Add(this.getName());
                    break;
                case ("Id"):
                    currUserList.Add(this.getId().ToString());
                    break;
                default:
                    return defualtL;
            }
            return currUserList;
        }
    }
}
