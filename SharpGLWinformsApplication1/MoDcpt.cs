using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;

namespace SharpGLWinformsApplication1
{
    public partial class MoDcpt : Form
    {
        Pointf[][] para;
        string[] paraname;
 
        public MoDcpt(params data3Dmat[] arg)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            para = new Pointf[arg.Length][];
            paraname = new string[arg.Length];
            for (int i = 0; i < arg.Length; i++)
            {
                para[i] = new Pointf[arg[i].data.Length];
                int number = 0;
                foreach (Pointf m in arg[i].data)
                {
                    para[i][number] = new Pointf(0, 0, 0);
                    para[i][number] += arg[i].data[number];
                    number++;
                }
                paraname[i] = arg[i].name;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int tempst = textBox1.SelectionStart;

            if (textBox1.SelectionStart + textBox1.SelectionLength < textBox1.Text.Length)
            { textBox1.Text = textBox1.Text.Substring(0, textBox1.SelectionStart) + button2.Text
               + textBox1.Text.Substring(textBox1.SelectionStart + textBox1.SelectionLength, textBox1.Text.Length - textBox1.SelectionStart-textBox1.SelectionLength); }
            else if (textBox1.SelectionStart + textBox1.SelectionLength == textBox1.Text.Length)
            { textBox1.Text += button2.Text; }
            textBox1.Focus();
            textBox1.Select(tempst + 3, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = 0;

            int num = textBox1.Text.Split(';').Length;
            string text = Regex.Replace(textBox1.Text, @"\s|\r|\n|\t", "");
            //置换输入框中的符号为计算用符号，如COM-->A0

            int ContrCount = 0;
            foreach (Control cl in this.groupBox1.Controls)
            {               
                if (cl.GetType().ToString() == "System.Windows.Forms.Button")
                {
                    text=text.Replace(cl.Text, "A" + ContrCount);
                    ContrCount++;
                }
            }
            
            foreach (string expr in text.Substring(0,text.Length-1).Split(';'))
            {
                if (CheckPointfOrNot(expr.Substring(expr.IndexOf('=') + 1, expr.Length - 1 - expr.IndexOf('=')) + ";"))
                {
                    progressBar1.Maximum += para[0].Length;
                }
                else if ((CheckPOINTOrNot(expr.Substring(expr.IndexOf('=') + 1, expr.Length - 1 - expr.IndexOf('=')) + ";")) & (!CheckPointfOrNot(expr.Substring(expr.IndexOf('=') + 1, expr.Length - 1 - expr.IndexOf('=')) + ";")))
                {
                    progressBar1.Maximum += para[0].Length;
                }
                else
                {
                    progressBar1.Maximum++;
                }
            }
            progressBar1.Maximum++;
            progressBar1.Minimum = 0;
           
            ThreadStart starter = delegate { dothing(text); };
            new Thread(starter).Start();

        }

        public void dothing(string text)
        {
            text = Regex.Replace(text, @"\s|\r|\n|\t", "");
            if (!Checkexp(text))
            {
                MessageBox.Show("表达式输入有误！");
                textBox1.Text = "";
                return;
            }
            string[] temp = text.Substring(0,text.Length-1).Split(';');
            
            int maxnum=0;
            
            DataSet ds = new DataSet();
            ds.Tables.Add();
            ds.Tables[0].Rows.Add();
            ds.Tables[0].Rows.Add();
            
            foreach (Pointf[] i in para)
            { 
                if (maxnum<i.Length) 
                    maxnum=i.Length;
            }
            for (int i = 0; i < maxnum; i++)
            {
                ds.Tables[0].Rows.Add();
            }
            
            int dataC=0, dataR = 0;//用于标记生成的数据表的行列
            
            foreach (string expr in temp)
            {
                if (CheckPointfOrNot(expr.Substring(expr.IndexOf('=') + 1, expr.Length - 1 - expr.IndexOf('='))+";"))
                {
                    ds.Tables[0].Columns.Add();
                    ds.Tables[0].Columns.Add();
                    ds.Tables[0].Columns.Add();
                    ds.Tables[0].Rows[dataR++][dataC] = expr.Substring(0, expr.IndexOf('='));
                    ds.Tables[0].Rows[dataR][dataC++] = "X";
                    ds.Tables[0].Rows[dataR][dataC++] = "Y";
                    ds.Tables[0].Rows[dataR][dataC++] = "Z";
                    dataC -= 3;
                    Pointf[] temppara = new Pointf[para.Length];
                    int numbers = 0;
                    foreach (Pointf i in para[0])
                    {
                        for (int m = 0; m < para.Length; m++)
                        {
                            temppara[m] = new Pointf();
                            temppara[m] += para[m][numbers];
                        }

                        EvaluateExpression ee = new EvaluateExpression(temppara);
                        Pointf tt = new Pointf(ee.Calculate(expr.Substring(expr.IndexOf('=') + 1, expr.Length - 1 - expr.IndexOf('='))));
                        ds.Tables[0].Rows[++dataR][dataC++] = tt.getX().ToString();
                        ds.Tables[0].Rows[dataR][dataC++] = tt.getY().ToString();
                        ds.Tables[0].Rows[dataR][dataC++] = tt.getZ().ToString();
                        dataC -= 3;
                        //ds.Tables[0].Rows.Add(tt.getX().ToString(), tt.getY().ToString(), tt.getZ().ToString());
                        tt = null;
                        
                        progressBar1.PerformStep();
                        numbers++;
                    }
                    temppara = null;
                    dataC += 3;
                    dataR = 0;
                }
                else if ((CheckPOINTOrNot(expr.Substring(expr.IndexOf('=') + 1, expr.Length - 1 - expr.IndexOf('=')) + ";")) & (!CheckPointfOrNot(expr.Substring(expr.IndexOf('=') + 1, expr.Length - 1 - expr.IndexOf('=')) + ";")))
                {
                    ds.Tables[0].Columns.Add();
                    ds.Tables[0].Rows.Add();
                    ds.Tables[0].Rows[dataR++][dataC] = expr.Substring(0, expr.IndexOf('='));
                    ds.Tables[0].Rows.Add();
                    ds.Tables[0].Rows[dataR++][dataC] = "Value";

                    Pointf[] temppara = new Pointf[para.Length];
                    int numbers = 0;
                    foreach (Pointf i in para[0])
                    {
                        for (int m = 0; m < para.Length; m++)
                        {
                            temppara[m] = new Pointf();
                            temppara[m] += para[m][numbers];
                        }
                        
                        EvaluateExpression ee = new EvaluateExpression(temppara);
                        ds.Tables[0].Rows[dataR++][dataC] = ee.Calculate(expr.Substring(expr.IndexOf('=') + 1, expr.Length - 1 - expr.IndexOf('='))).ToString();
                        numbers++;
                        progressBar1.PerformStep();
                    }
                    dataC++;
                    dataR = 0;
                }
                else
                {
                    ds.Tables[0].Columns.Add();
                    ds.Tables[0].Rows.Add();
                    ds.Tables[0].Rows[dataR++][dataC] = expr.Substring(0, expr.IndexOf('='));
                    ds.Tables[0].Rows.Add();
                    ds.Tables[0].Rows[dataR++][dataC] = "Value";
                    EvaluateExpression ee = new EvaluateExpression();
                    ds.Tables[0].Rows[dataR++][dataC] = ee.Calculate(expr.Substring(expr.IndexOf('=') + 1, expr.Length - 1 - expr.IndexOf('='))).ToString();
                    dataR = 0;
                    dataC++;
                    progressBar1.PerformStep();
                }

            }
            dataR = 0;
            dataC = 0;
            gSendGridInfoToExcel(ds, @"E:\111");
            progressBar1.PerformStep();
        }

        //public void dothing(string text)//需要重写！！输出结果，只支持整体运算不支持单个运算//这一段要重写
        //{
        //    string[] expr = text.split(';');
        //    foreach (string temp in expr)
        //    {
        //        temp.substring(0, temp.length - 1);
        //    }

        //    dataset ds = new dataset();
        //    ds.tables.add();
        //    for (int i = 0; i < group; i++)
        //    {
        //        ds.tables[0].columns.add("x");
        //        ds.tables[0].columns.add("y");
        //        ds.tables[0].columns.add("z");
        //        ds.tables[0].rows.add("x", "y", "z");
        //    }

        //    if (checkpointfornot(textbox1.text))
        //    {
        //        pointf[] temppara = new pointf[para.length];

        //        int numbers = 0;
        //        foreach (pointf i in para[0])
        //        {
        //            for (int m = 0; m < para.length; m++)
        //            {
        //                temppara[m] = new pointf();
        //                temppara[m] += para[m][numbers];
        //            }

        //            evaluateexpression ee = new evaluateexpression(temppara);
        //            ds.tables[0].rows.add(ee.calculate(textbox1.text).getx().tostring(), ee.calculate(textbox1.text).gety().tostring(), ee.calculate(textbox1.text).getz().tostring());
        //            progressbar1.performstep();
        //            numbers++;
        //        }
        //        numbers = 0;
        //        gsendgridinfotoexcel(ds, @"f:\111");
        //        progressbar1.performstep();
        //    }
        //    else
        //    {
        //        evaluateexpression ee = new evaluateexpression();
        //        string s = ee.calculate(textbox1.text).getx().tostring();
        //        messagebox.show(s.tostring(), "results");
        //    }
        //}

        private void button3_Click(object sender, EventArgs e)//文本框操作
        {
            int tempst=textBox1.SelectionStart;
            //int templen=textBox1.SelectionLength;
            if (textBox1.SelectionStart + textBox1.SelectionLength < textBox1.Text.Length)
            {
                textBox1.Text = textBox1.Text.Substring(0, textBox1.SelectionStart) + button3.Text
                 + textBox1.Text.Substring(textBox1.SelectionStart + textBox1.SelectionLength, textBox1.Text.Length - textBox1.SelectionStart - textBox1.SelectionLength);
            }
            else if (textBox1.SelectionStart + textBox1.SelectionLength == textBox1.Text.Length)
            { textBox1.Text = textBox1.Text.Substring(0, textBox1.SelectionStart) + button3.Text; }
            textBox1.Focus();
            textBox1.Select(tempst +3, 0);
        }

        public static void gSendGridInfoToExcel(DataSet ds, string excelpath) //利用ds输出为excel
        {

            if (ds.Tables.Count <= 0)
            {
                return;
            }
            int count = ds.Tables[0].Rows.Count;//获取数据表中DataRow行总数
            int column = ds.Tables[0].Columns.Count;//获取数据表中列总数
            Microsoft.Office.Interop.Excel.Application excelapp = new Microsoft.Office.Interop.Excel.Application();
            //Make Excel Application Visible
            excelapp.Visible = false;
            //写入特定文件
            //Excel.Workbook wb = excelapp.Workbooks.Open(string filename,
            //    Type.Missing,Type.Missing,Type.Missing,Type.Missing,
            //    Type.Missing,Type.Missing,Type.Missing,Type.Missing,
            //    Type.Missing,Type.Missing,Type.Missing,Type.Missing,
            //    Type.Missing,Type.Missing);
            Microsoft.Office.Interop.Excel.Workbook wb = excelapp.Application.Workbooks.Add(true);
            Microsoft.Office.Interop.Excel.Worksheet sheets = (Microsoft.Office.Interop.Excel.Worksheet)wb.Sheets[1];
            sheets.Name = excelpath.Substring(excelpath.LastIndexOf(@"\") + 1); ;
            for (int x = 1; x <= count; x++)
            {
                for (int y = 1; y <= column; y++)
                {
                    //excelapp.Cells[x, y] = this.archiverdbDataSet.channel.Rows[x- 1].ItemArray[y - 1];
                    excelapp.Cells[x, y] = ds.Tables[0].Rows[x - 1].ItemArray[y - 1];
                }
            }


            try
            {
                wb.SaveAs(excelpath + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8/*Type.Missing*/, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                wb.Saved = true;
                excelapp.UserControl = false;


            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }
            finally
            {
                
                excelapp.Quit();
                excelapp = null;
                ds = null;
                GC.Collect();//垃圾回收 
            }

            return;
        }

        /// <summary>
        /// 对表达式进行检查，是否包含三维坐标格式以方便输出
        /// </summary>
        /// <param name="expression">要求的表达式</param>
        /// <returns>检查结果</returns>
        public bool CheckPointfOrNot(string expression)
        {
            bool flag = false;
            string pattern=(@"A\d+[;+*/\-]");
            if (Regex.IsMatch(expression, pattern))
            {
                flag = true;
            }
            return flag;
        }
        public bool CheckPOINTOrNot(string expression)
        {
            bool flag = false;
            string pattern = (@"A\d+\.");
            if (Regex.IsMatch(expression, pattern))
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 对表达式进行检查，是否符合表达式的输入要求
        /// </summary>
        /// <param name="expression">要求的表达式</param>
        /// <returns>检查结果</returns>
        public bool Checkexp(string expression)
        {
            bool flag = true;
           //string pattern = (@"^(-|\+)?\d+(\.\d+)?$");
            string pattern = (@"^([A-Za-z0-9\.]+=([A-Za-z0-9+*/\-\.]+)[;])+$");
            Regex validate = new Regex(pattern);
            if (!validate.IsMatch(expression))
            {
                flag = false;
            }
            return flag;
        }
            
    }
}
