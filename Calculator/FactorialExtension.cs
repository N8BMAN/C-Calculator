using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public static class FactorialExtension
    {
        public static double Factorial(this Type Math, int num)
        {
            double val = num;
            for (int i = num-1; i > 0; i--)
                val *= i;
            return val;
        }

        public static double Gamma(this Type Math, double num)
        {
            if (num == System.Math.Floor(num))
                return Double.Parse((new Parser('(' + num.ToString() + "-1)!").Calculate()));
            string func = "(X^(d-1))*(e^-X)";
            return typeof(Math).Integrate(0, 1000, func.Replace("d", num.ToString()), 1, Double.MaxValue);
        }

        //Uses simpson's 1/3 rule to approximate definte integrals recursively
        public static double Integrate(this Type Math, double a, double b, string f, double n, double prev)
        {
            double h = (b - a) / n;
            double x = a;
            double sum = EvaluateVariableExpression(f, "X", x);
            for(int i = 1; i < n-2; i+=2)
            {
                x += h;
                sum += 4 * EvaluateVariableExpression(f, "X", x);
                x += h;
                sum += 2 * EvaluateVariableExpression(f, "X", x);
            }
            x += h;
            sum += 4 * EvaluateVariableExpression(f, "X", x);
            sum += EvaluateVariableExpression(f, "X", b);

            double result = (b - a) * sum / (3 * n);

            if (((System.Math.Abs(result - prev)) / result) * 100 < 0.0005) //fucking percent error
                return result;// System.Math.Round(result, 2);  //round to 2 decimals because approximation
            else
                return typeof(Math).Integrate(a, b, f, n * 2, result);
        }

        public static double EvaluateVariableExpression(string expression, string variable, double value)
        {
            return double.Parse(new Parser(expression.Replace(variable, value.ToString())).Calculate());
        }
    }
}
