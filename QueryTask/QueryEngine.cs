using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTask
{
    class QueryEngine
    {
        private const int fromLen = 5;
        private const int whereLen = 5;
        private const int selectLen = 7;
        private const int spaceLen = 1;

        private Data data; // DataBase 

        public QueryEngine(Data d)
        {
            data = d;
        }

        public List<List<string>> HandleQuery(string query) // Function for handling a new Query
        {

            List<List<string>> defualtL = new List<List<string>>();

            if (query == null) // Input check
            {
                Console.WriteLine("Wrong input");
                return defualtL;
            }

            query = CleanDownLines(query);

            query = CleanSpaces(query);

            int whereIdx = query.IndexOf("where"); // Index of "where" in the Query
            int selectIdx = query.IndexOf("select"); // Index of "select" in the Query

            // Slicing the from/where/select parts
            string from = query.Substring(fromLen, whereIdx - (fromLen + spaceLen));
            string where = query.Substring(whereIdx + (whereLen + spaceLen), selectIdx - (whereIdx + (whereLen + spaceLen) + spaceLen));
            string select = query.Substring(selectIdx + selectLen, query.Length - (selectIdx + selectLen));

            List<List<string>> whereWords = ParseWhere(where); // Creating a List of list<string>, each one represent a boolean expression

            return AnswerQuery(defualtL, from, select, whereWords); // Get candidates Objects and return their wanted fields
        }

        private List<List<string>> AnswerQuery(List<List<string>> defualtL, string from, string select, List<List<string>> whereWords)
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

        private static string CleanDownLines(string query) // Function for removing DownLines
        {
            query = query.Replace("\n", " ");
            return query;
        }

        private static string CleanSpaces(string query) // Function for removing extra spaces
        {
            while (query.Contains("  "))
            {
                query = query.Replace("  ", " ");
            }
            return query;
        }

        public List<List<string>> GetWantedFields(List<Object> candidates, string select, int type) // Function for creating the Query Answer (type: 0 = Order, 1 = User)
        {
            List<List<string>> defualtL = new List<List<string>>();
            List<List<string>> l = new List<List<string>>();

            if (candidates.Count == 0) // Check if there is no Candidate Objects
                return defualtL;
            else
            {
                string[] words = select.Split(',');
                foreach (Object curr in candidates) // Iterate on each Object, and getting all the needed fields
                {
                    List<string> currObjList = new List<string>();
                    foreach (string s in words)
                    {
                        string fixedS = s;
                        if (s[0] == ' ')
                            fixedS = s[1..];

                        if (type == 0) // Object = Order
                            currObjList = ((Order)curr).GetFields(fixedS, currObjList); // Gets the Order needed field

                        else if (type == 1) // Object = User
                            currObjList = ((User)curr).GetFields(fixedS, currObjList); // Gets the User needed field

                        if (currObjList.Count == 0) // Check if there is a problem with the "select" section
                        {
                            if (type == 0)
                                Console.WriteLine("Wrong 'select' input - Order not include the field '" + fixedS + "'");
                            else if (type == 1)
                                Console.WriteLine("Wrong 'select' input - User not include the field '" + fixedS + "'");
                            return defualtL;
                        }
                    }
                    l.Add(currObjList);
                }
                return l;
            }
        }
        public List<Object> GetCandidates(List<List<string>> where, List<Object> objects, int type) // Function for getting the Candidate Objects for the Query (type: 0 = Order, 1 = User)
        {
            List<Object> defualtL = new List<Object>();
            List<Object> candidates = new List<Object>();

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
                if (flag) // Check if curr is Qualify the Query criteria
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

        public List<List<string>> ParseWhere(string where) // Function for parsing the "where" section to List<List<string>>, each List contain boolean expression
        {
            List<List<string>> words = new List<List<string>>();
            bool first = false;
            bool second = false;
            bool isPara = false;
            bool isStr = false;
            bool addedOper = false;
            int fixedIndex = 0;
            string currWord = "";

            for (int i = 0; i < where.Length; i++)
            {
                if (i + fixedIndex >= where.Length) // Check if we finished the parsing
                {
                    if (!currWord.Equals(""))
                    {
                        words.Add(currWord.Split(' ').ToList());
                        currWord = "";

                    }
                    break;
                }
                if (!first && !addedOper && words.Count != 0) // Check if we parsing the second (or more) expression in a parentheses expression
                {
                    string oper = "";
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
                    List<string> operList = new List<string>();
                    operList.Add(oper);
                    words.Add(operList);
                    addedOper = true;
                    fixedIndex += currIndex;
                    continue;
                }
                if (!isPara) // Check if we are not parsing a parentheses expression 
                {
                    if (where[i + fixedIndex] != ' ' && where[i + fixedIndex] != '(' && where[i + fixedIndex] != '\'' && where[i + fixedIndex] != '"')
                    {
                        currWord += where[i + fixedIndex];
                    }
                    else if (where[i + fixedIndex] == '\'' || where[i + fixedIndex] == '"') // Check if we starting/finishing parsing a string value (name, city, etc..) 
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
                        if (isStr) // Check if the ' ' is inside a string value (name, city, etc..)
                        {
                            currWord += '-';
                        }
                        else if (first && second) // Check if we finished to parse the first + second operand
                        {
                            words.Add(currWord.Split(' ').ToList());
                            currWord = "";
                            first = false;
                            second = false;
                            addedOper = false;
                        }
                        else if (first) // Check if we finished to parse the first operand
                        {
                            currWord += where[i + fixedIndex];
                            second = true;
                        }
                        else
                        {
                            if (currWord.Equals("")) // Check if the ' ' is between expressions
                            {
                                continue;
                            }
                            currWord += where[i + fixedIndex]; // The ' ' is after the first operand
                            first = true;
                        }
                    }
                    else if (where[i + fixedIndex] == '(') // Check if the we starting parsing a parentheses expression
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
                else // we are parsing a parentheses expression 
                {
                    if (where[i + fixedIndex] == '\'' || where[i + fixedIndex] == '"') // Check if we starting/finishing parsing a string value (name, city, etc..) 
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
                            if (isStr) // Check if the ' ' is inside a string value (name, city, etc..)
                            {
                                currWord += '-';
                                continue;
                            }
                        }
                        currWord += where[i + fixedIndex];
                    }
                    else if (where[i + fixedIndex] == ')') // we are finishing parsing a parentheses expression 
                    {
                        words.Add(currWord.Split(' ').ToList());
                        currWord = "";
                        isPara = false;
                        addedOper = false;
                        fixedIndex += 1;
                    }
                }
            }
            if (!currWord.Equals("")) // Adding the boolean expression to the result list
            {
                words.Add(currWord.Split(' ').ToList());
                currWord = "";
            }
            return FixStr(words);
        }


        public List<List<string>> FixStr(List<List<string>> words) // Function for fixing the string inputs that came with ' '/" " in the Query
        {
            foreach (List<string> list in words)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i][0] == '.')
                    {
                        list[i] = list[i].Substring(1).Replace('-', ' ');
                    }
                }
            }
            return words;
        }
    }
}
