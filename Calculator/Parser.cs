using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Asssume everything is a double until proven to not be a double */
namespace Calculator
{
    class Parser
    {
        public bool usingDegrees { get; set; }
        // String to parse
        private string toParse { get; }
        // Current index
        private int index = 0;

        // Constructor
        public Parser(string parseIn)
        {
            toParse = parseIn;
        }

        // Get the next token
        char getToken()
        {
            if (index == toParse.Length)
                return '\0';
            char ch = toParse[index];
            index++;
            return ch;
        }

        //peek at the next token
        char Peek()
        {
            if (index == toParse.Length)
                return '\0';
            return toParse[index];
        }

        //what was the last token?
        char PeekLast()
        {
            if (index < 2)
                return '\0';
            return toParse[index - 2];
        }

        //parentheses, numbers, and trig
        double Factor()
        {
            double val = 0;
            char ch = getToken();
            bool isNeg = false;

            if (ch == '-')
            {
                if (!(Char.IsDigit(Peek()) || Peek() == '('))
                    throw new ArgumentException("Syntax error!");
                isNeg = true;
                ch = getToken();
            }
            //parse sub expression
            if (ch == '(')
            {
                val = expression();
                ch = getToken();
                if (ch != ')')
                    throw new ArgumentException("Expected )");
            }

            else if (ch == 's' || ch == 'c' || ch == 't')
            {
                char func = ch;
                // since trig functions can be determined by the first letter,
                // just consume the next two letters
                getToken();
                getToken();
                ch = getToken();
                if (ch == '(')
                {
                    if (usingDegrees)
                    {
                        if (func == 's')
                            val = (float)Math.Sin(Math.PI * expression() / 180.0);
                        if (func == 'c')
                            val = (float)Math.Cos(Math.PI * expression() / 180.0);
                        if (func == 't')
                            val = (float)Math.Tan(Math.PI * expression() / 180.0);
                    }
                    else
                    {
                        if (func == 's')
                            val = (float)Math.Sin(expression());
                        if (func == 'c')
                            val = (float)Math.Cos(expression());
                        if (func == 't')
                            val = (float)Math.Tan(expression());
                    }
                    ch = getToken();
                    if (ch != ')')
                        throw new ArgumentException("Expected )");
                }
            }

            //Finds numbers and e
            else if (Char.IsDigit(ch) || ch == '-')
                GetDigits(out val, ch);
            else if (ch == 'e')
                val = Math.E;
            else if (ch == 'Γ' || ch == '∫')
                index--;
            else
                throw new ArgumentException("Syntax Error");
            if (isNeg) val = -(val);
            return val;
        }

        void GetDigits(out double val, char ch)
        {
            val = ch - '0';
            while (Char.IsDigit(Peek()))
                val = val * 10 + (getToken() - '0');
            //Hacky decimals
            if (Peek() == '.')
            {
                //eat the token
                ch = getToken();
                string decNum = val.ToString();
                decNum += ch;
                if (!Char.IsDigit(Peek()))
                    throw new ArgumentException("Expected a digit");
                //concatenate each digit to the string
                while (Char.IsDigit(Peek()))
                    decNum += getToken();
                if (Peek() == '.')
                    throw new ArgumentException("Multiple Decimals");
                val = Convert.ToDouble(decNum);
            }
        }

        // Exponents
        double Exponent()
        {
            double val = Factor();
            char ch = getToken();
            if (ch == '^')
            {
                double b = Exponent();
                val = Convert.ToSingle(Math.Pow(val, b));
            }
            else
                index--;
            return val;
        }

        double Factorial()
        {
            double val = Exponent();
            char ch = getToken();
            if (ch == '!')
            {
                if ((val % 1) == 0)
                {
                    int num = (int)val;
                    val = typeof(Math).Factorial(num);
                }
                else
                    throw new ArgumentException("Can't factorial decimals");
            }
            else if (ch == 'Γ')
            {
                double expr = Factor();
                val = typeof(Math).Gamma(expr);
            }
            else if (ch == '∫')
            {
                try
                {
                    if (getToken() == '(')
                    {
                        string f = toParse.Substring(index);

                        int expressionEndIndex = -1;
                        int parenCount = 1;
                        for (int i = 0; i < f.Length; i++)
                        {
                            if (f[i] == '(')
                                parenCount++;
                            else if (f[i] == ')')
                                parenCount--;
                            if (parenCount == 0)
                            {
                                expressionEndIndex = i;
                                break;
                            }
                        }
                        f = f.Substring(0, expressionEndIndex);
                        List<string> argList = f.Split(',').ToList();
                        for (int i = 0; i < 2; i++)
                        {
                            argList[i] = new Parser(argList[i]).Calculate();
                        }
                        val = typeof(Math).Integrate(Double.Parse(argList[0]), Double.Parse(argList[1]), argList[2], 1, Double.MaxValue);
                    }
                }
                catch (ArgumentException e)
                {
                    //System.Windows.MessageBox.Show(e.Message, "Error");
                    throw new ArgumentException("Integral machine broke");
                }
            }
            else
                index--;
            return val;
        }

        // Mutliplication and Division
        double Term()
        {
            double val = Factorial();
            char ch = getToken();
            if (ch == '*' || ch == '/')
            {
                double b = Term();
                if (ch == '*')
                    val *= b;
                else
                {
                    if (b == 0)
                        throw new ArgumentException("ERR: Division by 0");
                    val /= b;
                }
            }
            else
                index--;
            return val;
        }

        // Addition and Subtraction
        double expression()
        {
            double val = Term();
            char ch = getToken();
            if (ch == '+' || ch == '-')
            {
                double b = expression();
                if (ch == '+')
                    val += b;
                else
                    val -= b;
            }
            else
                index--;
            return val;
        }

        public string Calculate()
        {
            //return 0 if no input
            if (toParse.Length == 0)
                return "0";
            //figure out if it can be an integer
            double result = expression();
            if (result == Math.Ceiling(result))
                return Convert.ToInt64(result).ToString();
            return result.ToString();
        }
    }
}
