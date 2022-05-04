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

        public QueryEngine(Data d)
        {
            this.data = d;
        }


        public List<List<String>> handleQuery(String query) // Function for handling a new Query
        {

            List<List<String>> defualtL = new List<List<string>>();

            // Input check
            if (query == null)
            {
                Console.WriteLine("Wrong input");
            }

            // Clean downLines
            query = query.Replace("\n", " ");

            // Clean extra spaces
            while (query.Contains("  "))
            {
                query = query.Replace("  ", " ");
            }

            // Parse the Query
            String from = "";
            String where = "";
            String select = "";


            int whereIdx = query.IndexOf("where"); // Index of "where" in the Query
            int selectIdx = query.IndexOf("select"); // Index of "select" in the Query

            // Slicing the from/where/select parts
            from = query.Substring(5, whereIdx - (5 + 1));
            where = query.Substring(whereIdx + (5 + 1), selectIdx - (whereIdx + (5 + 1) + 1));
            select = query.Substring(selectIdx + 7, query.Length - (selectIdx + 7));


            List<List<String>> whereWords = this.ParseWhere(where); // Creating a List of list<String>, each one represent a boolean expression


            switch (from) // Getting a List of the Candidates Objects (Orders/Users) that meet the Query criteria, and getting their wanted fields
            {
                case "Orders":
                    List<Order> orders = getCandidatesOrders(whereWords, this.data.getOrders());
                    return getWantedFieldsOrders(orders, select);

                case "Users":
                    List<User> Users = getCandidatesUsers(whereWords, this.data.getUsers());
                    return getWantedFieldsUsers(Users, select);

                default:
                    Console.WriteLine("Wrong 'from' input - there is no '" + from + "' Objects in the DataBase");
                    return defualtL;
            }
        }


        public List<List<String>> getWantedFieldsUsers(List<User> users, String select) // Function for creating the Query Answer
        {
            List<List<String>> defualtL = new List<List<string>>();
            List<List<String>> l = new List<List<string>>();

            if (users.Count == 0) // Check if there is no Candidate Users
                return defualtL;
            else
            {
                string[] words = select.Split(',');
                foreach(User u in users) // Iterate on each User, and getting all the needed fields
                {
                    List<String> currUserList = new List<string>();
                    foreach(String s in words)
                    {
                        String fixedS = s;
                        if (s[0] == ' ')
                            fixedS = s.Substring(1);

                        currUserList = u.getFields(fixedS, currUserList); // Gets the User needed field

                        if(currUserList.Count == 0) // Check if there is a problem with the "select" section
                        {
                            Console.WriteLine("Wrong 'select' input - User not include the field '" + fixedS + "'");
                            return defualtL;
                        }
                    }
                    l.Add(currUserList);
                }
                return l;
            }
        }
        public List<User> getCandidatesUsers(List<List<String>> where, List<User> users)
        {
            List<User> defualtL = new List<User>();
            List<User> candidates = new List<User>();

            foreach(User u in users) // Iterate on each User in the DataBase
            {
                bool flag = false;
                String oper = "";
                int countLists = 0;
                
                foreach (List<String> list in where) // Iterate on each List - expression/s in the query
                {
                    countLists += 1;
                    if (countLists % 2 == 0) // Check if the list contain only operator
                    {
                        oper = list[0];
                        continue;
                    }

                    for (int i = 0; i < list.Count; i += 4) // i + 4 for each iteration (expression (3) + operator (1) = 4)
                    {
                        int boolInt = u.booleanExpressionCheck(list[i], list[i + 1], list[i + 2]); // Check a single boolean expression (field, oper, val)

                        if(boolInt == 0) // false
                            flag = this.updateBoolFlag(flag, i, oper, false, countLists);

                        else if (boolInt == 1) // true
                            flag = this.updateBoolFlag(flag, i, oper, true, countLists);

                        else if (boolInt == 2) // Wrong input in the "where" section
                        {
                            Console.WriteLine("Wrong 'where' input - User not include the field '" + list[i] + "'");
                            return defualtL;
                        }

                        if(list.Count > i + 4) // Check if the current list is including more expressions (List of parenthesis expression)
                            oper = list[i + 3];
                        continue;
                    }
                }
                if (flag) // Check if the User is Qualify the Query criteria
                    candidates.Add(u);
                flag = false;
            }
            return candidates;
        }

        public List<List<String>> getWantedFieldsOrders(List<Order> orders, String select) // Function for creating the Query Answer
        {
            List<List<String>> defualtL = new List<List<string>>();
            List<List<String>> l = new List<List<string>>();

            if (orders.Count == 0) // Check if there is no Candidate Orders
                return l;
            else
            {
                string[] words = select.Split(',');
                foreach (Order o in orders) // Iterate on each Order, and getting all the needed fields
                {
                    List<String> currOrderList = new List<string>();
                    foreach (String s in words)
                    {
                        String fixedS = s;
                        if (s[0] == ' ')
                            fixedS = s.Substring(1);

                        currOrderList = o.getFields(fixedS, currOrderList); // Gets the Order needed field

                        if (currOrderList.Count == 0) // Check if there is a problem with the "select" section
                        {
                            Console.WriteLine("Wrong 'select' input - Order not include the field '" + fixedS + "'");
                            return defualtL;
                        }
                    }
                    l.Add(currOrderList);
                }
                return l;
            }
        }


        public List<Order> getCandidatesOrders(List<List<String>> where, List<Order> orders)
        {
            List<Order> candidates = new List<Order>();

            foreach (Order o in orders) // Iterate on each Order in the DataBase
            {
                List<Order> defualtL = new List<Order>();
                bool flag = false;
                String oper = "";
                int countLists = 0;

                foreach (List<String> list in where) // Iterate on each List - expression/s in the query
                {
                    countLists += 1;
                    if (countLists % 2 == 0) // Check if the list contain only operator
                    {
                        oper = list[0];
                        continue;
                    }

                    for (int i = 0; i < list.Count; i += 4) // i + 4 for each iteration (expression (3) + operator (1) = 4)
                    {
                        int boolInt = o.booleanExpressionCheck(list[i], list[i + 1], list[i + 2]); // Check a single boolean expression (field, oper, val)

                        if (boolInt == 0) // false
                            flag = this.updateBoolFlag(flag, i, oper, false, countLists);

                        else if (boolInt == 1) // true
                            flag = this.updateBoolFlag(flag, i, oper, true, countLists);

                        else if (boolInt == 2) // Wrong input in the "where" section
                        {
                            Console.WriteLine("Wrong 'where' input - User not include the field '" + list[i] + "'");
                            return defualtL;
                        }
                        if (list.Count > i + 4) // Check if the current list is including more expressions (List of parenthesis expression)
                            oper = list[i + 3];
                        continue;
                    }
                }
                if (flag) // Check if the Order is Qualify the Query criteria
                    candidates.Add(o);
                flag = false;
            }
            return candidates;
        }

        


        public bool updateBoolFlag(bool flag, int i, String oper, bool isTrue, int countLists) // Function for updating the boolean value that determine the qualify of an Object on the Query criteria
        {
            if (isTrue)
            {
                if (i == 0 && countLists == 1)
                    flag = true;
                else
                {
                    if (oper.Equals("AND"))
                        flag = flag && true;
                    else
                        flag = flag || true;
                }
            }
            else
            {
                if (i == 0 && countLists == 1)
                    flag = false;
                else
                {
                    if (oper.Equals("AND"))
                        flag = flag && false;
                    else
                        flag = flag || false;
                }
            }
            return flag;
        }

        public List<List<String>> ParseWhere(String where) // Function for parsing the "where" section to List<List<String>>, each List contain boolean expression
        {
            List<List<String>> words = new List<List<String>>();
            bool first = false;
            bool second = false;
            bool isPara = false;
            bool isStr = false;
            bool addedOper = false;
            int fixedIndex = 0;
            String currWord = "";

            for (int i = 0; i < where.Length; i++)
            {
                if (i + fixedIndex >= where.Length)
                {
                    if(!currWord.Equals(""))
                    {
                        words.Add(currWord.Split(' ').ToList());
                        currWord = "";

                    }
                    break;
                }
                if (!first && !addedOper && words.Count != 0)
                {
                    String oper = "";
                    int currIndex = 0;

                    while (true)
                    {
                        if (where[i + currIndex + fixedIndex] == ' ')
                        {
                            break;
                        }
                        else
                        {
                            oper += where[i + currIndex + fixedIndex];
                            currIndex += 1;
                        }
                    }
                    List<String> l = new List<string>();
                    l.Add(oper);
                    words.Add(l);
                    addedOper = true;
                    fixedIndex += currIndex;
                    continue;
                }
                if (!isPara)
                {
                    if (where[i + fixedIndex] != ' ' && where[i + fixedIndex] != '(' && where[i + fixedIndex] != '\'' && where[i + fixedIndex] != '"')
                    {
                        currWord += where[i + fixedIndex];
                    }
                    else if (where[i + fixedIndex] == '\'' || where[i + fixedIndex] == '"')
                    {
                        if (isStr)
                        {
                            isStr = false;
                        }
                        else
                        {
                            isStr = true;
                            currWord += '.';
                        }
                    }
                    else if (where[i + fixedIndex] == ' ')
                    {
                        if (isStr)
                        {
                            currWord += '-';
                        }
                        else if (first && second)
                        {
                            words.Add(currWord.Split(' ').ToList());
                            currWord = "";
                            first = false;
                            second = false;
                            addedOper = false;

                        }
                        else if (first)
                        {
                            currWord += where[i + fixedIndex];
                            second = true;
                        }
                        else
                        {
                            if (currWord.Equals(""))
                            {
                                continue;
                            }
                            currWord += where[i + fixedIndex];
                            first = true;
                        }
                    }
                    else if (where[i + fixedIndex] == '(')

                    {
                        isPara = true;
                        first = false;
                        second = false;
                    }

                    else
                    {
                        continue;
                    }
                }
                else
                {
                    if (where[i + fixedIndex] == '\'' || where[i + fixedIndex] == '"')
                    {
                        if (isStr)
                        {
                            isStr = false;
                        }
                        else
                        {
                            isStr = true;
                            currWord += '.';
                        }
                    }
                    else if (where[i + fixedIndex] != ')')
                    {
                        if (where[i + fixedIndex] == ' ')
                        {
                            if (isStr)
                            {
                                currWord += '-';
                                continue;
                            }
                        }
                        currWord += where[i + fixedIndex];
                    }

                    else if (where[i + fixedIndex] == ')')
                    {
                        words.Add(currWord.Split(' ').ToList());
                        currWord = "";
                        isPara = false;
                        addedOper = false;
                        fixedIndex += 1;
                    }
                }
            }
            if (!currWord.Equals(""))
            {
                words.Add(currWord.Split(' ').ToList());
                currWord = "";
            }

            return this.fixStr(words);
        }


        public List<List<String>> fixStr(List<List<String>> words) // Function for fixing the String inputs that came with ' '/" " in the Query
        {
            foreach(List<String> list in words)
            {
                for(int i = 0; i < list.Count; i++)
                {
                    if(list[i][0] == '.')
                    {
                        list[i] = list[i].Substring(1).Replace('-', ' ');
                    }
                }
            }
            return words;
        }
    }
}
