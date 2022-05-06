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


        public User(string fullName, string email, int age)
        {
            this.fullName = fullName;
            this.email = email;
            this.age = age;
        }

        public string GetName()
        {
            return fullName;
        }

        public string GetMail()
        {
            return email;
        }

        public int GetAge()
        {
            return age;
        }

        public int BooleanExpressionCheck(string field, string oper, string val) // Function for cheking a boolean expression (0 - false, 1 - true, 2 - wrong input)
        {
            switch (field)
            {
                case ("FullName"):
                    return CheckName(val);
                case ("Email"):
                    return CheckMail(val);
                case ("Age"):
                    return CheckAge(val, oper);
                default:
                    return 2; // wrong input field
            }
        }

        private int CheckName(string val) // 0 - false, 1 - true
        {
            if (GetName().Equals(val))
                return 1;
            else
                return 0;
        }

        private int CheckMail(string val) // 0 - false, 1 - true
        {
            if (GetMail().Equals(val))
                return 1;
            else
                return 0;
        }

        private int CheckAge(string val, string oper) // 0 - false, 1 - true, 2 - wrong input
        {
            if (oper == "=")
            {
                if (GetAge() == Int32.Parse(val))
                    return 1;
                else
                    return 0;
            }
            else if (oper == ">")
            {
                if (GetAge() > Int32.Parse(val))
                    return 1;
                else
                    return 0;
            }
            else if (oper == "<")
            {
                if (GetAge() < Int32.Parse(val))
                    return 1;
                else
                    return 0;
            }
            else if (oper == "<=")
            {
                if (GetAge() <= Int32.Parse(val))
                    return 1;
                else
                    return 0;
            }
            else if (oper == ">=")
            {
                if (GetAge() >= Int32.Parse(val))
                    return 1;
                else
                    return 0;
            }
            else
                return 2;
        }

        public List<string> GetField(string field, List<string> currUserList) // Function for getting the needed fields by the "select" section
        {
            List<string> defualtL = new();

            switch (field)
            {
                case ("FullName"):
                    currUserList.Add(GetName());
                    break;
                case ("Email"):
                    currUserList.Add(GetMail());
                    break;
                case ("Age"):
                    currUserList.Add(GetAge().ToString());
                    break;
                default:
                    return defualtL;
            }
            return currUserList;
        }
    }
}
