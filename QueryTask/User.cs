using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTask
{
    class User
    {
        public string email;
        public string fullName;
        public int age;

        public User(String fullName, String email, int age)
        {
            this.fullName = fullName;
            this.email = email;
            this.age = age;
        }

        public String getName()
        {
            return this.fullName;
        }

        public String getMail()
        {
            return this.email;
        }

        public int getAge()
        {
            return this.age;
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

                case ("Email"):
                    if (this.getMail().Equals(val))
                        return 1;
                    else
                        return 0;

                case ("Age"):
                    if (oper == "=")
                    {
                        if (this.getAge() == Int32.Parse(val))
                            return 1;
                        else
                            return 0;
                    }
                    else if (oper == ">")
                    {
                        if (this.getAge() > Int32.Parse(val))
                            return 1;
                        else
                            return 0;
                    }
                    else if (oper == "<")
                    {
                        if (this.getAge() < Int32.Parse(val))
                            return 1;
                        else
                            return 0;
                    }
                    else if (oper == "<=")
                    {
                        if (this.getAge() <= Int32.Parse(val))
                            return 1;
                        else
                            return 0;
                    }
                    else if (oper == ">=")
                    {
                        if (this.getAge() >= Int32.Parse(val))
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
                case ("Email"):
                    currUserList.Add(this.getMail());
                    break;
                case ("Age"):
                    currUserList.Add(this.getAge().ToString());
                    break;
                default:
                    return defualtL;
            }
            return currUserList;
        }
    }
}
