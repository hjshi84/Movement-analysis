using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using System.Threading;
using SharpGL;

namespace SharpGLWinformsApplication1
{
    public struct walkcycle
    {
        public int start;
        public int end;
        public bool startok, endok;
    }
    /// <summary>
    /// The main form class.
    /// </summary>
    public partial class SharpGLForm : Form
    {
        private Pointf[][] com,comprevious;
        private double maxY, minY, maxX, minX,maxZ,minZ;
        private PointF _oldPosition, trangle;
        private float rotation=0;//旋转视角
        private System.Timers.Timer time1;
        private int[] length;

        private static DrawXYZ dwxyz;

        private bool[] select3d;
        private int selectnumber=0;
        private int intervial=1;

        private walkcycle wc;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>
        public SharpGLForm(params data3Dmat[] arg)
        {
            Form1.frmopen = true;
            com = new Pointf[arg.Length][];
            comprevious = new Pointf[arg.Length][];
            length=new int[arg.Length];
            select3d = new bool[arg.Length];
            for (int m = 0; m < arg.Length; m++)
            {
                com[m] = new Pointf[arg[m].data.Length];
                comprevious[m] = new Pointf[arg[m].data.Length];
                maxY = arg[m].data[0].getY();
                minX = arg[m].data[0].getX();
                minY = arg[m].data[0].getY();
                maxZ = arg[m].data[0].getZ();
                minZ = arg[m].data[0].getZ();
                foreach (Pointf i in arg[m].data)
                {
                    if (i.getX() > maxX) maxX = i.getX();
                    else if (i.getX() < minX) minX = i.getX();
                    if (i.getY() > maxY) maxY = i.getY();
                    else if (i.getY() < minY) minY = i.getY();
                    if (i.getZ() > maxZ) maxZ = i.getZ();
                    else if (i.getZ() < minZ) minZ = i.getZ();
                }
                for (int i = 0; i < arg[selectnumber].data.Length; i++)
                {
                    com[m][i] = new Pointf();
                    comprevious[m][i] = new Pointf();
                    comprevious[m][i].setX(arg[selectnumber].data[i].getX());
                    comprevious[m][i].setY(arg[selectnumber].data[i].getY());
                    comprevious[m][i].setZ(arg[selectnumber].data[i].getZ());
                    com[m][i].setX(arg[m].data[i].getX() - (maxX + minX) / 2);
                    com[m][i].setY(arg[m].data[i].getY() - (maxY + minY) / 2);
                    com[m][i].setZ(arg[m].data[i].getZ() - (maxZ + minZ) / 2);
                }
                length[m] = arg[m].data.Length;
            }
            
            InitializeComponent();
           


            foreach (data3Dmat i in arg)
            {
                checkedListBox1.Items.Add(i.name);                
            }
            for (int i = 0; i < select3d.Length; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
                select3d[i] = true;
            }
           
            checkedListBox1.ItemCheck += apple;

            dwxyz = new DrawXYZ(com[selectnumber]);
            pictureBox1.Image = dwxyz.bmx;
            pictureBox2.Image = dwxyz.bmy;
            pictureBox3.Image = dwxyz.bmz;
            
            time1 = new System.Timers.Timer(); 
            time1.Elapsed += play3D;
            time1.Interval = 10;
            time1.AutoReset = true;
            time1.Enabled = false;
        }


        public void play3D(object source, System.Timers.ElapsedEventArgs e)
        {
            
            if (trackBar1.InvokeRequired)
            {
                trackBar1.Invoke(new Action(() =>
                {
                    intervial = trackBar2.Value;
                }));
            }
            else
            {
                intervial = trackBar2.Value;
            }

            if (trackBar1.InvokeRequired)
            {
                trackBar1.Invoke(new Action(() =>
                {
                    if (trackBar1.Value+intervial> com[selectnumber].Length)
                    {
                        trackBar1.Value = 1;
                    }
                    else
                    {
                        trackBar1.Value = trackBar1.Value + intervial;
                    }
                }));
            }
            else
            {
                if (trackBar1.Value >= com[selectnumber].Length)
                {
                    trackBar1.Value = 1;
                }
                else
                {
                    trackBar1.Value = trackBar1.Value + 1;
                }
            }


        }


        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, PaintEventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Load the identity matrix.
            gl.LoadIdentity();
            gl.Rotate(trangle.X, 0, 1, 0);
            gl.Rotate(trangle.Y, 1, 0, 0);
            gl.LineWidth(2);
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f,0.0f,0.0f);
            gl.Vertex(0.005f, 0.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 0.005f, 0.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f, 0.005f);
            gl.End();
            gl.Begin(OpenGL.GL_TRIANGLES);
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.005f, 0.0005f, 0.0f);
            gl.Vertex(0.005f, -0.0005f, 0.0f);
            gl.Vertex(0.0055f, 0.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(0.0005f, 0.005f, 0.0f);
            gl.Vertex(-0.0005f, 0.005f, 0.0f);
            gl.Vertex(0.0f, 0.0055f, 0.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(0.0f, 0.0005f, 0.005f);
            gl.Vertex(0.0f, -0.0005f, 0.005f);
            gl.Vertex(0.0f, 0.0f, 0.0055f);
            gl.End();

            textBox1.Text = trangle.Y.ToString();
            textBox2.Text = trangle.X.ToString();
            textBox3.Text = string.Format("{0:F6}", comprevious[selectnumber][trackBar1.Value - 1].getX());
            textBox4.Text = string.Format("{0:F6}", comprevious[selectnumber][trackBar1.Value - 1].getY());
            textBox5.Text = string.Format("{0:F6}", comprevious[selectnumber][trackBar1.Value - 1].getZ());
            label3.Text = "当前为第  " + trackBar1.Value + "  个数据，共有  " + com[selectnumber].Length + "  个数据";

            int se = 0;

            foreach(Pointf[] i in com)
            {
                
                if (select3d[se] == true)
                {
                    
                    gl.LoadIdentity();
                    //  Rotate around the Y axis.
                    //gl.Translate((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2);
                    gl.Translate(0, 0, rotation / 10000);

                    gl.Rotate(trangle.X, 0, 1, 0);
                    gl.Rotate(trangle.Y, 1, 0, 0);



                    //gl.Translate(-(maxX + minX) / 2, -(maxY + minY) / 2, -(maxZ + minZ) / 2);
                    //测试用
                    //gl.Translate(com[0].getX()+(double)trackBar1.Value/2000, com[0].getY(), com[0].getZ());
                    //gl.Translate(-(maxX + minX)/2, -(maxY + minY)/2, -(maxZ + minZ)/2);           
                    //gl.Translate((maxX + minX)/2, (maxY + minY)/2, (maxZ + minZ)/2);

                    gl.Translate(i[trackBar1.Value - 1].getX(), i[trackBar1.Value - 1].getY(), i[trackBar1.Value - 1].getZ());

                    //  Draw a coloured pyramid.
                    gl.Begin(OpenGL.GL_TRIANGLES);
                    gl.Color(1.0f, 0.0f, 0.0f);
                    gl.Vertex(0.0f, 0.0003f, 0.0f);
                    gl.Color(0.0f, 1.0f, 0.0f);
                    gl.Vertex(-0.0003f, -0.0003f, 0.0003f);
                    gl.Color(0.0f, 0.0f, 1.0f);
                    gl.Vertex(0.0003f, -0.0003f, 0.0003f);
                    gl.Color(1.0f, 0.0f, 0.0f);
                    gl.Vertex(0.0f, 0.0003f, 0.0f);
                    gl.Color(0.0f, 0.0f, 1.0f);
                    gl.Vertex(0.0003f, -0.0003f, 0.0003f);
                    gl.Color(0.0f, 1.0f, 0.0f);
                    gl.Vertex(0.0003f, -0.0003f, -0.0003f);
                    gl.Color(1.0f, 0.0f, 0.0f);
                    gl.Vertex(0.0f, 0.0003f, 0.0f);
                    gl.Color(0.0f, 1.0f, 0.0f);
                    gl.Vertex(0.0003f, -0.0003f, -0.0003f);
                    gl.Color(0.0f, 0.0f, 1.0f);
                    gl.Vertex(-0.0003f, -0.0003f, -0.0003f);
                    gl.Color(1.0f, 0.0f, 0.0f);
                    gl.Vertex(0.0f, 0.0003f, 0.0f);
                    gl.Color(0.0f, 0.0f, 1.0f);
                    gl.Vertex(-0.0003f, -0.0003f, -0.0003f);
                    gl.Color(0.0f, 1.0f, 0.0f);
                    gl.Vertex(-0.0003f, -0.0003f, 0.0003f);
                    gl.End();
                }
                se++;
            }

            se = 0;

            dwxyz.free();
            dwxyz = new DrawXYZ(com[selectnumber]);
            Graphics gph = Graphics.FromImage(dwxyz.bmx);
            gph.DrawLine(Pens.Black, new Point((int)trackBar1.Value / (length[selectnumber] / 500) + 20, 20), new Point((int)trackBar1.Value / (length[selectnumber] / 500) + 20, 380));
            gph = Graphics.FromImage(dwxyz.bmy);
            gph.DrawLine(Pens.Black, new Point((int)trackBar1.Value / (length[selectnumber] / 500) + 20, 20), new Point((int)trackBar1.Value / (length[selectnumber] / 500) + 20, 380));
            gph = Graphics.FromImage(dwxyz.bmz);
            gph.DrawLine(Pens.Black, new Point((int)trackBar1.Value / (length[selectnumber] / 500) + 20, 20), new Point((int)trackBar1.Value / (length[selectnumber] / 500) + 20, 380));
            pictureBox1.Image = dwxyz.bmx;
            pictureBox2.Image = dwxyz.bmy;
            pictureBox3.Image = dwxyz.bmz;
        }



        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);

            trackBar1.SetRange(1, com[selectnumber].Length);
            trackBar1.TickFrequency = 100;

            
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, EventArgs e)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            
            //  Load the identity.
            gl.LoadIdentity();
            //gl.Ortho(minX, maxX, minY, maxY, minZ - (maxZ - minZ) / 2, maxZ + (maxZ - minZ) / 2);
            gl.Perspective(45.0f, (double)Width / (double)Height, 0.001, 1);
            gl.LookAt(0, 0, 0.03, 0,0,0, 0, 1, 0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, new float[4] { 1, 1, 1, 1 });
            
            //  Use the 'look at' helper function to position and aim the camera.
            //gl.LookAt(0.1, 0, 0, middle.getX(),middle.getY(),middle.getZ(), 0, 0, 1);
            //gl.LookAt(1, 0, 0,middle.getX(), middle.getY(),middle.getZ(), 0, 0, 1);
            //  Set the modelview matrix.

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        /// <summary>
        /// The current rotation.
        /// </summary>

        private void openGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                trangle.X += Cursor.Position.X - _oldPosition.X;
                trangle.Y += Cursor.Position.Y - _oldPosition.Y;
                _oldPosition = Cursor.Position;
            }
            else if (e.Button == MouseButtons.Right)
            {
                rotation += -Cursor.Position.Y + _oldPosition.Y + Cursor.Position.X - _oldPosition.X;
                    
                _oldPosition = Cursor.Position;
            }
        }

        private void openGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            _oldPosition = Cursor.Position;
        }

        private void openGLControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (time1.Enabled == false)
                    time1.Enabled = true;
                else
                    time1.Enabled = false;
            }

        }

        private void SharpGLForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (time1.Enabled == true)
            {
                e.Cancel = true;
                MessageBox.Show("请按 空格键 暂停动画后，再关闭窗口","注意");
                return;
            }
            Form1.frmopen = false;
            dwxyz.free();
        }

        public void apple(object sender, ItemCheckEventArgs e)
        {
            select3d[int.Parse(e.Index.ToString())] = e.NewValue.ToString().Equals("Checked")? true:false;
        }

        private void checkedListBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Right)
            {
                selectnumber = int.Parse(checkedListBox1.SelectedIndex.ToString()) == -1 ? selectnumber : int.Parse(checkedListBox1.SelectedIndex.ToString());
                dwxyz.free();
                dwxyz = new DrawXYZ(com[selectnumber]);
                pictureBox1.Image = dwxyz.bmx;
                pictureBox2.Image = dwxyz.bmy;
                pictureBox3.Image = dwxyz.bmz;
            }
        }

        private void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = e.Location;
                //  p.X = e.Location.X + this.Location.X + 5;
                //  p.Y = e.Location.Y + this.Location.Y + 30;

                contextMenuStrip1.Show(this.trackBar1, p);


            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void StartPlace_Click(object sender, EventArgs e)
        {

            wc.start = trackBar1.Value;
            wc.startok = true;
            StartPlace.Text = "重设起始桢（" + wc.start + ")";
        }

        private void EndPosition_Click(object sender, EventArgs e)
        {
            wc.end = trackBar1.Value;
            wc.endok = true;
            EndPosition.Text = "重设结束桢（" + wc.end + ")";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string savepict = saveFileDialog1.FileName.ToString();
                    savepict = savepict.Substring(0, savepict.LastIndexOf("."));
                    Image img = pictureBox1.Image;
                    img.Save(savepict + "X.jpg");
                    img = pictureBox2.Image;
                    img.Save(savepict + "Y.jpg");
                    img = pictureBox3.Image;
                    img.Save(savepict + "Z.jpg");
                    Bitmap bm1 = new Bitmap(600,1200);
                    Graphics g = Graphics.FromImage(bm1);
                    g.DrawImage(pictureBox1.Image, 0, 0);
                    g.DrawImage(pictureBox2.Image, 0, 400);
                    g.DrawImage(pictureBox3.Image, 0, 800);
                    img = bm1;
                    img.Save(savepict + "ALL.jpg");
                }
            }
            catch
            {
                MessageBox.Show("保存文件失败！");
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            double pos = (pictureBox2.PointToClient(MousePosition).X / (double)this.pictureBox2.Width) * 600 ;
            Point X = new Point(pictureBox2.PointToClient(MousePosition).X, groupBox2.PointToClient(MousePosition).Y);
            if (pos > (double)(520) )
                pos = 540;
            if (pos < (double)(20)) 
                pos = 20;
            trackBar1.Value = (int)( (double)((pos-20)/520.0)*trackBar1.Maximum);
            
           
        }










    }
}
