using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTask
{
    class Parser
    {
        private const int fromLen = 5;
        private const int whereLen = 5;
        private const int selectLen = 7;
        private const int spaceLen = 1;

        public Parser()
        {

        }

        public (string, List<List<string>>, string) Parse(string query)
        {
            query = CleanDownLines(query);
            query = CleanSpaces(query);

            int whereIdx = query.IndexOf("where"); // Index of "where" in the Query
            int selectIdx = query.IndexOf("select"); // Index of "select" in the Query

            // Slicing the from/where/select sections
            string from = query.Substring(fromLen, whereIdx - (fromLen + spaceLen));
            string where = query.Substring(whereIdx + (whereLen + spaceLen), selectIdx - (whereIdx + (whereLen + spaceLen) + spaceLen));
            string select = query.Substring(selectIdx + selectLen, query.Length - (selectIdx + selectLen));

            List<List<string>> whereWords = ParseWhere(where); // Creating a List of list<string>, each one represent a boolean expression

            return (from, whereWords, select);
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

        public static List<List<string>> ParseWhere(string where) // Function for parsing the "where" section to List<List<string>>, each List contain boolean expression
        {
            List<List<string>> words = new();
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
                    List<string> operList = new();
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


        public static List<List<string>> FixStr(List<List<string>> words) // Function for fixing the string inputs that came with ' '/" " in the Query
        {
            foreach (List<string> list in words)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i][0] == '.')
                    {
                        list[i] = list[i][1..].Replace('-', ' ');
                    }
                }
            }
            return words;
        }
    }


 }



