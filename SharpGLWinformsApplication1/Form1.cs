using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SharpGLWinformsApplication1
{
    public struct data3Dmat
{
    public Pointf[] data;
    public string name;
}

    public partial class Form1 : Form
    {
        private Pointf[] com;
        private Pointf[] cop;
        private Pointf[] copycom;
        private int datanumber = 0;
        private static Draw graph;

        private void rectifydata(Pointf[] com_, Pointf[] cop_)//校正中心
        {
            com = com_;
            cop = cop_;

            copycom = com_;
            Pointf meancom = meandata(com_);
            Pointf meancop = meandata(cop_);


            if (com_.Length == cop_.Length)
            {
                Pointf[] a = new Pointf[com.Length];
                int number = 0;
                foreach (Pointf i in com)
                {
                    a[number] = i - (meancom - meancop);
                    number++;
                }
                com = a;
            }
        }

        public Pointf meandata(Pointf[] arg0)//平均值
        {
            Pointf total = new Pointf(0, 0, 0);
            int number = 0;
            foreach (Pointf i in arg0)
            {
                total = total + i;
                number++;
            }
            return new Pointf(total.getX() / number, total.getY() / arg0.Length, total.getZ() / arg0.Length);
        }

        public Pointf maxvalue(Pointf[] arg0)//最大值
        {
            Pointf maxvalue = new Pointf();
            foreach (Pointf i in arg0)
            {
                if (i.getX() > maxvalue.getX()) maxvalue.setX(i.getX());
                if (i.getY() > maxvalue.getY()) maxvalue.setY(i.getY());
                if (i.getZ() > maxvalue.getZ()) maxvalue.setZ(i.getZ());
            }
            return maxvalue;
        }

        public Pointf minvalue(Pointf[] arg0)//最小值
        {
            Pointf minvalue = new Pointf();
            foreach (Pointf i in arg0)
            {
                if (i.getX() < minvalue.getX()) minvalue.setX(i.getX());
                if (i.getY() < minvalue.getY()) minvalue.setY(i.getY());
                if (i.getZ() < minvalue.getZ()) minvalue.setZ(i.getZ());
            }
            return minvalue;
        }

        public double distance(Pointf[] arg0)//轨迹路径
        {
            double distance = 0;
            for (int i = 1; i < arg0.Length; i++)
            {
                distance = distance + Math.Abs(arg0[i].length(arg0[i - 1]));
            }
            return distance;
        }

        public Pointf msd(Pointf[] com_, Pointf[] cop_)//均方差
        {
            Pointf[] msd = new Pointf[cop_.Length];
            for (int i = 0; i < datanumber; i++)
            {
                msd[i] = new Pointf(0, 0, 0);
            }
            for (int i = 0; i < msd.Length; i++)
            {
                msd[i] = msd[i] + cop_[i] - com_[i];
            }
            Pointf meanval = meandata(msd);
            Pointf msdval = new Pointf();
            foreach (Pointf i in msd)
            {
                msdval.setX(msdval.getX() + Math.Pow((i.getX() - meanval.getX()), 2));
                msdval.setY(msdval.getY() + Math.Pow((i.getY() - meanval.getY()), 2));
                msdval.setZ(msdval.getZ() + Math.Pow((i.getZ() - meanval.getZ()), 2));
            }
            msdval.setX(Math.Sqrt(msdval.getX() / msd.Length));
            msdval.setY(Math.Sqrt(msdval.getY() / msd.Length));
            msdval.setZ(Math.Sqrt(msdval.getZ() / msd.Length));
            return msdval;
        }

        private Pointf[] swayangle(Pointf[] com_, Pointf[] cop_) //摆动角度
        {
            Pointf[] swangle = new Pointf[datanumber];

            for (int i = 0; i < datanumber; i++)
            {
                swangle[i] = new Pointf();
                swangle[i].setX((cop[i] - com[i]).getX() / com[i].getX());
                swangle[i].setY((cop[i] - com[i]).getY() / com[i].getY());
            }
            return swangle;
        }



        public Form1()
        {
            InitializeComponent();
        }


        private void readtxt(string filepath) 
        {
            string filename = filepath;
            filename = filename.Substring(filename.LastIndexOf("\\") + 1, filename.Length - filename.LastIndexOf("\\") - 5);


            //用EXCEL方式打开
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";" +
            "Extended Properties='Excel 8.0;HDR=NO;IMEX=1'";
            //OleDbDataAdapter oada = new OleDbDataAdapter("select * from [Sheet1$]", strConn);

            OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + filename + "$]", strConn);

            oada.Fill(dataSet1);
            dataGridView1.DataSource = dataSet1.Tables[0];
            //dataGridView1.ReadOnly = true;
            //整理项目数据
            string previous = dataGridView1[1, 0].Value.ToString();
            dataGridView1.EnableHeadersVisualStyles = false;
            for (int i = 2; i < dataGridView1.ColumnCount; i++)
            {
                if (previous.Equals(dataGridView1[i, 0].Value.ToString())) { }
                else
                {
                    previous = dataGridView1[i, 0].Value.ToString();
                    dataGridView1[i, 0].Style.BackColor = Color.Blue;
                }
            }
            foreach (DataGridViewColumn column in dataGridView1.Columns)//取消自动排序
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void dataimport(string filename) //对EXCEL数据处理得到所需结果，想要其他的方式重写此函数即可
        {
            //读入数据
            string filepath = dataGridView1[dataGridView1.CurrentCell.ColumnIndex, 0].Value.ToString();
            string copname = "CENTREOFMASS";
            string comname = "COFP";
            for (int i = dataGridView1.ColumnCount - 1; i >= 1; i--)//删除不需要的数据
            {
                if ((filepath.Equals(dataGridView1[i, 0].Value.ToString())) & ((copname.Equals(dataGridView1[i, 1].Value.ToString())) | (comname.Equals(dataGridView1[i, 2].Value.ToString())))) { }
                else
                {
                    dataGridView1.Columns.RemoveAt(i);
                    //dataGridView1.Columns[i].Visible = false;
                }

            }
            datanumber = 0;

            for (; dataGridView1[1, 4 + datanumber].Value.ToString().Trim().Length != 0; datanumber++)//计算COM,COP的个数
            {
                if (datanumber + 4 == dataGridView1.Rows.Count - 2) { datanumber++; break; }//计算最大行的值（在多组数据的情况下）
            }
            datanumber--;
            com = new Pointf[datanumber];//申请内存
            cop = new Pointf[datanumber];

            for (int i = 0; i < datanumber; i++)
            {
                com[i] = new Pointf(0, 0, 0);
                cop[i] = new Pointf(0, 0, 0);
            }

            for (int i = 1; i < dataGridView1.ColumnCount; i++)//读入com与cop数据
            {
                if (copname.Equals(dataGridView1[i, 1].Value.ToString()))
                {

                    switch (dataGridView1[i, 4].Value.ToString())
                    {
                        case "X":
                            for (int j = 0; j < datanumber; j++)
                            {
                                com[j].setX(double.Parse(dataGridView1[i, j + 5].Value.ToString()));
                            }
                            break;
                        case "Y":
                            for (int j = 0; j < datanumber; j++)
                            {
                                com[j].setY(double.Parse(dataGridView1[i, j + 5].Value.ToString()));
                            }
                            break;
                        case "Z":
                            for (int j = 0; j < datanumber; j++)
                            {
                                com[j].setZ(double.Parse(dataGridView1[i, j + 5].Value.ToString()));
                            }
                            break;
                    }

                }
                else if (comname.Equals(dataGridView1[i, 2].Value.ToString()))
                {
                    switch (dataGridView1[i, 4].Value.ToString())
                    {
                        case "X":
                            for (int j = 0; j < datanumber; j++)
                            {
                                cop[j].setX(double.Parse(dataGridView1[i, j + 5].Value.ToString()));
                            }
                            break;
                        case "Y":
                            for (int j = 0; j < datanumber; j++)
                            {
                                cop[j].setY(double.Parse(dataGridView1[i, j + 5].Value.ToString()));
                            }
                            break;
                        case "Z":
                            for (int j = 0; j < datanumber; j++)
                            {
                                cop[j].setZ(double.Parse(dataGridView1[i, j + 5].Value.ToString()));
                            }
                            break;
                    }
                }
            }
            rectifydata(com, cop);//调整中心
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            panel1.Enabled = false;
            button2.Enabled = false;
            button8.Enabled = false;
            dataSet1.Clear();
            dataGridView1.DataSource = null;
            OpenFileDialog opdialog = new OpenFileDialog();
            opdialog.Filter = "EXCEL2003文件|*.xls";
            opdialog.RestoreDirectory = true;
            if (opdialog.ShowDialog() == DialogResult.OK)
            {
                readtxt(opdialog.FileName);
            }
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex != 0)
            {
                dataimport(null);
                //画图??????
                graph = new Draw(com, cop);
                pictureBox1.Image = graph.paint(checkBox1.Checked, checkBox2.Checked);
                Console.Write("");
                Envarea e1 = new Envarea(cop, datanumber);
                panel1.Enabled = true;
                button8.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "COM:";
            label2.Text = "COP:";
            textBox1.Text = (distance(com) / datanumber).ToString();
            textBox2.Text = (distance(cop) / datanumber).ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = "COM:";
            label2.Text = "COP:";
            Envarea e1 = new Envarea(com, datanumber);
            textBox1.Text = e1.getarea().ToString();
            e1 = null;
            e1 = new Envarea(cop, datanumber);
            textBox2.Text = e1.getarea().ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label1.Text = "X:";
            label2.Text = "Y:";
            textBox1.Text = msd(com, cop).getX().ToString();
            textBox2.Text = msd(com, cop).getY().ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            label1.Text = "COM:";
            label2.Text = "COP:";
            textBox1.Text = ((maxvalue(com).getX() - minvalue(com).getX()) / (maxvalue(com).getY() - minvalue(com).getY())).ToString();
            textBox2.Text = ((maxvalue(cop).getX() - minvalue(cop).getX()) / (maxvalue(cop).getY() - minvalue(cop).getY())).ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            label1.Text = "X:";
            label2.Text = "Y:";
            textBox1.Text = maxvalue(swayangle(com, cop)).getX().ToString();
            textBox2.Text = maxvalue(swayangle(com, cop)).getY().ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            graph.free();
            graph = new Draw(com, cop);
            pictureBox1.Image = graph.paint(checkBox1.Checked, checkBox2.Checked);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            graph.free();
            graph = new Draw(com, cop);
            pictureBox1.Image = graph.paint(checkBox1.Checked, checkBox2.Checked);
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            if (!frmopen)
            {
                data3Dmat arg1 = new data3Dmat();
                arg1.data = com;
                arg1.name = "COM";
                data3Dmat arg2 = new data3Dmat();
                arg2.data = cop;
                arg2.name = "COP";
                Form frm = new SharpGLForm(arg1, arg2);
                frm.Show();
            }
            else
            {
                MessageBox.Show("你已经打开过窗口，请勿再开", "注意");
            }
        }

        public static bool frmopen = false;

        private void button9_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:

                    break;
                case MouseButtons.Right:

                    break;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            data3Dmat arg1 = new data3Dmat();
            arg1.data = com;
            arg1.name = "COM";
            data3Dmat arg2 = new data3Dmat();
            arg2.data = cop;
            arg2.name = "COP";
            MoDcpt m = new MoDcpt(arg1,arg2);
            m.Show();
        }


    }
}
