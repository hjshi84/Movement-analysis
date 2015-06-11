using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpGLWinformsApplication1
{
    class EvaluateExpression
    {
        private Pointf[] para;
        public EvaluateExpression(Pointf[] arg)
        {
            para=new Pointf[arg.Length];
            for (int i = 0; i < arg.Length; i++)
            {
                para[i] = new Pointf(0, 0, 0);
                para[i] += arg[i];
            }
        }
        public EvaluateExpression()
        {
            para =new Pointf[1];
            para[0] =new Pointf(1, 2, 3);

        }
        /// <summary>
        /// 根据表3.1，判断两符号的优先关系
        /// </summary>
        /// <param name="Q1">操作栈栈顶运算符</param>
        /// <param name="Q2">当前从表达式读取到的运算符</param>
        /// <returns>返回Q1和Q2两个运算符之间的优先关系</returns>
        private string Precede(string Q1, string Q2)  
        {
           
            string f = string.Empty;
            switch (Q2)
            {
                case "+":
                case "-":
                    if (Q1 == "(" || Q1 == "#")
                        f = "<";
                    else
                        f = ">";
                    break;
 
                case "*":
                case "/":
                    if (Q1 == "*" || Q1 == "/" || Q1 == ")")
                        f = ">";
                    else
                        f = "<";
                    break;
                case "(":
                    if (Q1 == ")")
                        throw new ArgumentOutOfRangeException("表达式错误！");
                    else
                        f = "<";
                    break;
                case ")":
                    switch (Q1)
                    {
                        case "(": f = "="; break;
                        case "#": throw new ArgumentOutOfRangeException("表达式错误！");
                        default: f = ">"; break;
                    }
                    break;
                case "#":
                    switch (Q1)
                    {
                        case "#": f = "="; break;
                        case "(": throw new ArgumentOutOfRangeException("表达式错误！");
                        default: f = ">"; break;
                    }
                    break;
            }
            return f;
        }
 
        /// <summary>
        /// 判断c是否为操作符
        /// </summary>
        /// <param name="c">需要判断的运算符</param>
        /// <returns></returns>
        private bool IsKeyword(object c)  
        {
            if (c.GetType().Name == "Pointf") { return false; }
            switch (c.ToString())
            {
                case "+":
                case "-":
                case "*":
                case "/":
                case "(":
                case ")": return true;
                default: return false;
            }
        }
 
        /// <summary>
        /// 判断字符串是否为保留字
        /// </summary>
        /// <param name="input">需要判断的字符串</param>
        /// <returns></returns>
        private bool IsNumericOrArg(object input)        
        {
            bool flag = true;
            string pattern = (@"^(-|\+)?\d+(\.\d+)?|(A(\d)+|A(\d)+(\.)[XYZ])$");//正则表达式，|或，？ 0个及以上，+ 一个级以上
            Regex validate = new Regex(pattern);
            if (input.GetType().Name == "Pointf") { return flag; }
            if (!validate.IsMatch(input.ToString()))
            {
                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// 判断字符串是否为实数
        /// </summary>
        /// <param name="input">需要判断的字符串</param>
        /// <returns></returns>
        private bool IsNumeric(string input)
        {
            bool flag = true;
            string pattern = (@"^(-|\+)?\d+(\.\d+)?$");
            Regex validate = new Regex(pattern);
            if (!validate.IsMatch(input))
            {
                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// 判断字符串是否是带点的传入参数
        /// </summary>
        /// <param name="input">需要判断的字符串</param>
        /// <returns></returns>
        private bool IsArg(string input)
        {
            bool flag = false;
            string pattern = (@"^[A](\d)+\.[XYZ]$");
            Regex validate = new Regex(pattern);
            if (validate.IsMatch(input))
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 对传入的参数opnd1和opnd2进行oper四则运算
        /// </summary>
        /// <param name="opnd1">操作数1</param>
        /// <param name="oper">运算符</param>
        /// <param name="opnd2">操作数2</param>
        /// <returns>返回实数结果</returns>
        private static Pointf Operate(Pointf opnd1, string oper,Pointf opnd2)
        {
            Pointf result = new Pointf(0,0,0);
            switch (oper)
            {
                case "+": result = opnd1 + opnd2; break;
                case "-": result = opnd1 - opnd2; break;
                case "*": result = opnd1 * opnd2; break;
                case "/":
                    {
                        if (opnd2==new Pointf(0,0,0)) throw new ArgumentOutOfRangeException("除数为0!");
                        result = opnd1 / opnd2; break;
                    }
                default: throw new ArgumentOutOfRangeException(string.Format("操作符{0}错误！", oper));
            }
            return result;
        }
        private static double Operate(double opnd1, string oper, double opnd2)
        {
            double result = 0;
            switch (oper)
            {
                case "+": result = opnd1 + opnd2; break;
                case "-": result = opnd1 - opnd2; break;
                case "*": result = opnd1 * opnd2; break;
                case "/":
                    {
                        if (opnd2 == 0) throw new ArgumentOutOfRangeException("除数为0!");
                        result = opnd1 / opnd2; break;
                    }
                default: throw new ArgumentOutOfRangeException(string.Format("操作符{0}错误！", oper));
            }
            return result;
        }
        /// <summary>
        /// 检查传入的是否为运算符
        /// </summary>
        /// <param name="op">要检查的数组元素</param>
        /// <returns></returns>
        private static bool IsOperator(object op)
        {
            bool flag = false;
            if (op.GetType().Name == "Pointf") { return flag; }
            switch (op.ToString())
            {
                case "+": flag = true; break;
                case "-": flag = true; break;
                case "*": flag = true; break;
                case "/": flag = true; break;
            }
            return flag;
        }
 
        /// <summary>
        /// 对表达式的每一部分进行检查是否符合表达式的规范
        /// </summary>
        /// <param name="expr">表达式数组</param>
        /// <param name="idx">要检查的部分</param>
        /// <returns></returns>
        private bool CheckEveryExpr(ArrayList expr, int idx)
        {
            int len = expr.Count;
            if (len == 0) return false;
            if (idx == len - 1) return true;
            int p = 0;  //previous
            int n = 0;  //next
            if (idx == 0)  //表达式只能以数字或者(开头
            {
                if (IsNumericOrArg(expr[idx]))  //数字 expr[n]可为[+,-,*,/,#]
                {
                    n = idx + 1;
                    if (n >= len) return false;
                    else
                    {
                        if (expr[n].ToString() == "#") return true;
                        else
                        {
                            if (IsOperator(expr[n])) return true;
                            else return false;
                        }
                    }
                }
                else if (expr[idx].ToString() == "(")  //( expr[n]可为[数字]
                {
                    n = idx + 1;
                    if (n >= len) return false;
                    else
                    {
                        if (IsNumericOrArg(expr[n])) return true;
                        else return false;
                    }
                }
                else return false;
            }
            else if (idx == len - 2)  //表达式只能以数字或者)结尾
            {
                if (IsNumericOrArg(expr[idx])) return true;
                else if (expr[idx].ToString() == ")") return true;
                else return false;
            }
            else  //表达式中间部分分成4种进行判断
            {
                n = idx + 1;
                p = idx - 1;
                if (IsNumericOrArg(expr[idx]))  //数字 expr[p]可为[(,+,-,*,/] expr[n]可为[),+,-,*,/]
                {
                    if (IsKeyword(expr[p]) && expr[p].ToString() != ")") return true;
                    else return false;
                }
                else if (IsKeyword(expr[idx]))  //操作符 +,-,*,/ 
                {
                    if(IsOperator(expr[idx]))  //+,-,*,/操作操作符 expr[p]可为[数字,)] expr[n]可为[数字,(]
                    {
                        if ((IsNumericOrArg(expr[p]) || expr[p].ToString() == ")")
                            && (IsNumericOrArg(expr[n]) || expr[n].ToString() == "(")) return true;
                        else return false;
                    }
                    else //操作符 ( )
                    {
                        if (expr[idx].ToString() == "(") //( expr[p]可为[+,-,*,/,(] expr[n]可为[数字,(]
                        {
                            if ((IsOperator(expr[p]) || expr[p].ToString() == "(") &&
                                (IsNumericOrArg(expr[n]) || expr[n].ToString() == "(")) return true;
                            else return false;
                        }
                        else if (expr[idx].ToString() == ")") //) expr[p]可为[数字,)] expr[n]可为[+,-,*,/,)] 
                        {
                            if ((IsNumericOrArg(expr[p]) || expr[p].ToString() == ")") && 
                                (IsOperator(expr[n]) || expr[n].ToString() == ")")) return true;
                            else return false;
                        }
                        else return false;
                    }
                }
                else return false;
            }
        }
 
        /// <summary>
        /// 把表达式拆分成字符串数组，用于后面的检查和求值
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>从左到右返回从下标0开始的字符串数组</returns>
        private ArrayList SplitExpression(string expression)
        {
            ArrayList exprs =new ArrayList();
            List<string> lstItem = new List<string>();
            expression = expression.Trim();
            int length = expression.Length;
            string item = string.Empty;
            string ch = string.Empty;
            while (length != 0)
            {
                ch = expression.Substring(expression.Length - length, 1);
                if (!IsKeyword(ch)) item += ch;
                else
                {
                    item = item.Trim();
                    if (item != string.Empty) lstItem.Add(item);
                    item = string.Empty;
                    lstItem.Add(ch);
                }
                length--;
            }
            item = item.Trim();
            if (item != string.Empty) lstItem.Add(item);
 
            for (int i = 0; i < lstItem.Count; i++) 
                exprs.Add(lstItem[i]);
            exprs.Add("#");
 
            return exprs;
        }
 
        /// <summary>
        /// 对表达式进行语法校验
        /// </summary>
        /// <param name="expression">要校验的表达式</param>
        /// <returns></returns>
        public bool CheckExpression(string expression)
        {
            bool flag = true;
            object op = null;
            object operand1 = null;
            object operand2 = null;
            Stack optr = new Stack();
            optr.Push("#");
            Stack opnd = new Stack();
 
            ArrayList expr = SplitExpression(expression);
            int idx = 0;
            while (idx < expr.Count && (expr[idx].ToString() != "#" || optr.Peek().ToString() != "#"))
            {
                if (!CheckEveryExpr(expr, idx)) return false;
                if (!IsKeyword(expr[idx]))
                {
                    if (!IsNumericOrArg(expr[idx]))
                    {
                        if (expr[idx].ToString() == "#")
                        {
                            if (optr.Peek().ToString() != "#")
                            {
                                if (opnd.Count < 2) return false;
                                op = optr.Pop();
                                operand1 = opnd.Pop();
                                operand2 = opnd.Pop();
                                if (IsOperator(op)) opnd.Push(operand1);
                                else return false;
                            }
                        }
                        else return false;
                    }
                    else
                    {
                        opnd.Push(expr[idx]);
                        idx++;
                    }
                }
                else
                {
                    switch (Precede(optr.Peek().ToString(), expr[idx].ToString()))
                    {
                        case "<":         //栈顶元素优先权低
                            optr.Push(expr[idx]);
                            idx++;
                            break;
                        case "=":       //脱括号并接收下一个字符
                            optr.Pop();
                            idx++;
                            break;
                        case ">":    //退栈并将运算结果入栈
                            if (opnd.Count < 2) return false;
                            op = optr.Pop().ToString();
                            operand1 = opnd.Pop().ToString();
                            operand2 = opnd.Pop().ToString();
                            if (IsOperator(op)) opnd.Push(operand1);
                            else return false;
                            break;
                    }
                }
            }
            if (opnd.Count != 1) flag = false;
            return flag;
        }
 
        /// <summary>
        /// 对表达式进行求值，求值之前会先进行语法校验
        /// </summary>
        /// <param name="expression">要求值的表达式</param>
        /// <returns>求值结果</returns>
        public object Calculate(string expression)
        {
            if (!CheckExpression(expression)) throw new ArgumentOutOfRangeException("表达式错误！");
 
            object op = null;
            object operand1 = null;
            object operand2 = null;
            Stack optr = new Stack();
            optr.Push("#");
            Stack opnd = new Stack();

            ArrayList expr = SplitExpression(expression);
            int idx = 0;
            
            while (idx < expr.Count && (expr[idx].ToString() != "#" || optr.Peek().ToString() != "#"))
            {
                if (IsNumericOrArg(expr[idx]))
                {
                    if ((!IsArg(expr[idx].ToString())) & (!IsNumeric(expr[idx].ToString())))
                    {
                        opnd.Push(para[Convert.ToInt16(expr[idx].ToString().Substring(1, expr[idx].ToString().Length - 1))]);
                    }
                    else if (IsArg(expr[idx].ToString()))
                    {
                        switch (expr[idx].ToString().Substring(expr[idx].ToString().IndexOf('.') + 1))
                        {
                            case "X":
                                opnd.Push(para[Convert.ToInt16(expr[idx].ToString().Substring(1, expr[idx].ToString().Length - 3))].getX());
                                break;
                            case "Y":
                                opnd.Push(para[Convert.ToInt16(expr[idx].ToString().Substring(1, expr[idx].ToString().Length - 3))].getY());
                                break;
                            case "Z":
                                opnd.Push(para[Convert.ToInt16(expr[idx].ToString().Substring(1, expr[idx].ToString().Length - 3))].getZ());
                                break;
                        }
                    }
                    else opnd.Push(expr[idx]);                 
                   idx++;
                    
                }
                else
                {
                    switch (Precede(optr.Peek().ToString(), expr[idx].ToString()))
                    {
                        case "<":         //栈顶元素优先权低
                            optr.Push(expr[idx]);
                            idx++;
                            break;
                        case "=":       //脱括号并接收下一个字符
                            optr.Pop();
                            idx++;
                            break;
                        case ">":    //退栈并将运算结果入栈
                            if (opnd.Count < 2) return new Pointf(0,0,0);
                            op = optr.Pop();
                            
                            operand2 = opnd.Pop();
                            operand1 = opnd.Pop();

                            

                            if (operand1.GetType() == operand2.GetType())
                            {
                                string a=operand1.GetType().ToString();
                                switch (operand1.GetType().ToString())
                                {
                                    case "SharpGLWinformsApplication1.Pointf":
                                        opnd.Push(Operate(new Pointf(operand1), op.ToString(), new Pointf(operand2)));
                                        break;
                                    default:
                                        opnd.Push(Operate(Convert.ToDouble(operand1), op.ToString(), Convert.ToDouble(operand2)));
                                        break;
                                }
                            }
                            else 
                            {
                                opnd.Push(Operate(new Pointf(operand1), op.ToString(), new Pointf(operand2)));
                            }
                            break;
                    }
                }
            }
 
            return opnd.Peek();
        }
        
    }
}

    
