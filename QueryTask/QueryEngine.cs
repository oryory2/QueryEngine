using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTask
{
    class QueryEngine
    {

        private Data data; // DataBase 
        private Parser parser; // Parser

        public QueryEngine(Data d)
        {
            data = d;
            parser = new Parser();
        }

        public List<List<string>> HandleQuery(string query) // Function for handling a new Query
        {

            List<List<string>> defualtL = new();

            if (query == null) // Input check
            {
                Console.WriteLine("Wrong input");
                return defualtL;
            }

            var sectionsTupple = parser.Parse(query); // tupple of (from, where, select) sections

            return AnswerQuery(defualtL, sectionsTupple.Item1, sectionsTupple.Item2, sectionsTupple.Item3); // Get the candidates Objects and return their wanted fields
        }

        private List<List<string>> AnswerQuery(List<List<string>> defualtL, string from, List<List<string>> whereWords, string select)
        {
            switch (from) // Getting a List of the Candidates Objects (Orders/Users) that meet the Query criteria, and getting their wanted fields
            {
                case "Orders":
                    List<Object> orders = GetCandidates(whereWords, data.GetOrders(), 0);
                    return GetWantedFields(orders, select, 0);
                case "Users":
                    List<Object> Users = GetCandidates(whereWords, data.GetUsers(), 1);
                    return GetWantedFields(Users, select, 1);
                default:
                    Console.WriteLine("Wrong 'from' input - there is no '" + from + "' Objects in the DataBase");
                    return defualtL;
            }
        }

        public List<Object> GetCandidates(List<List<string>> where, List<Object> objects, int type) // Function for getting the Candidate Objects for the Query (type: 0 = Order, 1 = User)
        {
            List<Object> defualtL = new();
            List<Object> candidates = new();

            foreach (Object curr in objects) // Iterate on each User/Order in the DataBase
            {
                bool flag = true;
                string oper = "";
                int countLists = 0;

                foreach (List<string> list in where) // Iterate on each List - expression/s in the query
                {
                    countLists += 1;
                    if (countLists % 2 == 0) // Check if the list contain only operator
                    {
                        oper = list[0];
                        continue;
                    }

                    for (int i = 0; i < list.Count; i += 4) // i + 4 for each iteration (expression (3) + operator (1) = 4)
                    {
                        int boolInt = -1;
                        if (type == 0) // Object = Order
                            boolInt = ((Order)curr).BooleanExpressionCheck(list[i], list[i + 1], list[i + 2]); // Check a single boolean expression (field, oper, val)
                        else if (type == 1) // Object = User
                            boolInt = ((User)curr).BooleanExpressionCheck(list[i], list[i + 1], list[i + 2]); // Check a single boolean expression (field, oper, val)

                        if(oper != "" && (oper.ToLower() != "and" && oper.ToLower() != "or"))
                        {
                            Console.WriteLine("Wrong 'where' input - '" + oper + "' is not a valid Operator");
                            return defualtL;
                        }

                        if (boolInt == 0) // false
                            flag = UpdateBoolFlag(flag, i, oper, false, countLists);
                        else if (boolInt == 1) // true
                            flag = UpdateBoolFlag(flag, i, oper, true, countLists);
                        else if (boolInt == 2) // Wrong input in the "where" section
                        {
                            if(type == 0)
                                Console.WriteLine("Wrong 'where' input - Order not include the field '" + list[i] + "'");
                            else if(type == 1)
                                Console.WriteLine("Wrong 'where' input - User not include the field '" + list[i] + "'");
                            return defualtL;
                        }
                        if (list.Count > i + 4) // Check if the current list is including more expressions (parentheses expression)
                            oper = list[i + 3];
                        continue;
                    }
                }
                if (flag) // Check if the Object is Qualify the Query criteria
                    candidates.Add(curr);
            }
            return candidates;
        }

        public bool UpdateBoolFlag(bool flag, int i, string oper, bool isTrue, int countLists) // Function for updating the boolean value that determine the qualify of an Object on the Query criteria
        {
            if (i == 0 && countLists == 1)
                flag = isTrue;
            else
            {
                if (oper.ToLower().Equals("and"))
                    flag = flag && isTrue;
                else if (oper.ToLower().Equals("or"))
                    flag = flag || isTrue;
            }
            return flag;
        }

        public List<List<string>> GetWantedFields(List<Object> candidates, string select, int type) // Function for creating the Query Answer (type: 0 = Order, 1 = User)
        {
            List<List<string>> defualtL = new();
            List<List<string>> result = new();

            if (candidates.Count == 0) // Check if there is no Candidate Objects
                return defualtL;
            else
            {
                string[] words = select.Split(',');
                foreach (Object curr in candidates) // Iterate on each Object, and getting all the needed fields
                {
                    List<string> currObjList = new();
                    foreach (string s in words)
                    {
                        string field = s;
                        if (s[0] == ' ')
                            field = s[1..];

                        if (type == 0) // Object = Order
                            currObjList = ((Order)curr).GetField(field, currObjList); // Gets the Order needed field

                        else if (type == 1) // Object = User
                            currObjList = ((User)curr).GetField(field, currObjList); // Gets the User needed field

                        if (currObjList.Count == 0) // Check if there is a problem with the "select" section
                        {
                            if (type == 0)
                                Console.WriteLine("Wrong 'select' input - Order not include the field '" + field + "'");
                            else if (type == 1)
                                Console.WriteLine("Wrong 'select' input - User not include the field '" + field + "'");
                            return defualtL;
                        }
                    }
                    result.Add(currObjList);
                }
                return result;
            }
        }
    }
}
