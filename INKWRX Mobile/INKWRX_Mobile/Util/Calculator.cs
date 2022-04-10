using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Util
{
    public static class Calculator
    {
        public static double Calculate(string expression)
        {
            int lastBracket = expression.LastIndexOf('(');
            while (lastBracket != -1)
            {
                int nextClose = expression.IndexOf(')', lastBracket);
                string subsection = expression.Substring(lastBracket + 1, nextClose - lastBracket - 1);
                expression = expression.Replace("(" + subsection + ")", ProcessMultiplicationDivision(subsection).ToString());
                lastBracket = expression.LastIndexOf('(');
            };

            return ProcessMultiplicationDivision(expression);
        }

        private static double ProcessMultiplicationDivision(string expression)
        {
            int indexMultiply = expression.IndexOf('*');
            int indexDivide = expression.IndexOf('/');

            while (indexMultiply != -1 || indexDivide != -1)
            {
                if (indexMultiply == -1)
                {
                    double before = GetBefore(expression, indexDivide);
                    double after = GetAfter(expression, indexDivide);
                    double divide;
                    if (after == 0)
                    {
                        divide = 0;
                    }
                    else
                    {
                        divide = before / after;
                    }
                    expression = expression.Replace(before + "/" + after, divide.ToString());
                }
                else if (indexDivide == -1)
                {
                    double before = GetBefore(expression, indexMultiply);
                    double after = GetAfter(expression, indexMultiply);
                    double multiply = before * after;
                    expression = expression.Replace(before + "*" + after, multiply.ToString());
                }
                else if (indexMultiply < indexDivide)
                {
                    double before = GetBefore(expression, indexMultiply);
                    double after = GetAfter(expression, indexMultiply);
                    double multiply = before * after;
                    expression = expression.Replace(before + "*" + after, multiply.ToString());
                }
                else
                {
                    double before = GetBefore(expression, indexDivide);
                    double after = GetAfter(expression, indexDivide);
                    double divide;
                    if (after == 0)
                    {
                        divide = 0;
                    }
                    else
                    {
                        divide = before / after;
                    }
                    expression = expression.Replace(before + "/" + after, divide.ToString());
                }

                indexMultiply = expression.IndexOf('*');
                indexDivide = expression.IndexOf('/');
            };

            return ProcessAdditionSubtraction(expression);
        }

        private static double GetBefore(string expression, int index)
        {
            string beforeMultiple = expression.Substring(0, index);
            int beforeIndex = GetBeforeIndex(beforeMultiple);
            if (beforeIndex < 1)
            {
                //return double.Parse(beforeMultiple, CultureInfo.InvariantCulture);
                return double.Parse(beforeMultiple);
            }
            else
            {
                string stringBefore = beforeMultiple.Substring(beforeIndex + 1);
                return double.Parse(stringBefore, CultureInfo.InvariantCulture);
            }
        }

        private static double GetAfter(string expression, int index)
        {
            int afterIndex = GetAfterIndex(expression.Substring(index + 1));
            string stringAfter;
            if (afterIndex == -1)
            {
                stringAfter = expression.Substring(index + 1);
            }
            else
            {
                stringAfter = expression.Substring(index + 1, afterIndex);
            }
            return double.Parse(stringAfter, CultureInfo.InvariantCulture);
        }

        private static int GetAfterIndex(string expression)
        {
            int indexMultiply = expression.IndexOf('*');
            int indexDivide = expression.IndexOf('/');
            int indexAddition = expression.IndexOf('+');
            int indexSubtraction = expression.IndexOf('-', 1);

            if (indexMultiply == -1 && indexDivide == -1)
            {
                if (indexAddition == -1 && indexSubtraction == -1)
                {
                    return -1;
                }
                return (new List<int> { indexAddition, indexSubtraction }).OrderBy(x => x).First(x => x > -1);
            }
            else
            {
                return (new List<int> { indexMultiply, indexDivide }).OrderBy(x => x).First(x => x > -1);
            }
        }
        private static int GetBeforeIndex(string expression)
        {
            int indexMultiply = expression.LastIndexOf('*');
            int indexDivide = expression.LastIndexOf('/');
            int indexAddition = expression.LastIndexOf('+');
            int indexSubtraction = expression.LastIndexOf('-');

            if (indexMultiply == -1 && indexDivide == -1)
            {
                if (indexAddition == -1 && indexSubtraction == -1)
                {
                    return -1;
                }
                return (new List<int> { indexAddition, indexSubtraction }).OrderBy(x => x).Last(x => x > -1);
            }
            else
            {
                return (new List<int> { indexMultiply, indexDivide }).OrderBy(x => x).Last(x => x > -1); 
            }
        }

        private static double ProcessAdditionSubtraction(string expression)
        {
            expression.Replace("+-", "-");
            int indexAddition = expression.IndexOf('+');
            int indexSubtraction = expression.IndexOf('-', 1);
            if (indexSubtraction == 0)
            {
                return indexSubtraction = -1;
            }

            while (indexAddition != -1 || indexSubtraction != -1)
            {
                if (indexAddition == -1)
                {
                    double before = GetBefore(expression, indexSubtraction);
                    double after = GetAfter(expression, indexSubtraction);
                    double result = before - after;
                    expression = expression.Replace(before + "-" + after, result.ToString());
                }
                else if (indexSubtraction == -1)
                {
                    double before = GetBefore(expression, indexAddition);
                    double after = GetAfter(expression, indexAddition);
                    double result = before + after;
                    expression = expression.Replace(before + "+" + after, result.ToString());
                }
                else if (indexAddition < indexSubtraction)
                {
                    double before = GetBefore(expression, indexAddition);
                    double after = GetAfter(expression, indexAddition);
                    double result = before + after;
                    expression = expression.Replace(before + "+" + after, result.ToString());
                }
                else
                {
                    double before = GetBefore(expression, indexSubtraction);
                    double after = GetAfter(expression, indexSubtraction);
                    double result = before - after;
                    expression = expression.Replace(before + "-" + after, result.ToString());
                }
                expression.Replace("+-", "-");

                indexAddition = expression.IndexOf('+');
                indexSubtraction = expression.IndexOf('-', 1);
                if (indexSubtraction == 0)
                {
                    return indexSubtraction = -1;
                }
            }

            return double.Parse(expression, CultureInfo.InvariantCulture);
        }
    }
}
