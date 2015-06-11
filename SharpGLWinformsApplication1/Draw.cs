using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SharpGLWinformsApplication1
{
    class Draw
    {
        private Bitmap drawmap;
        static private Pointf[] com;
        static private Pointf[] cop;
        private int number;
        private double maxY, minY, maxX, minX;
        private double Y, X;

        public Draw(Pointf[] com_, Pointf[] cop_)
        {
            com = com_;
            cop = cop_;
            number = com.Length;
            maxX = com[0].getX();
            maxY = com[0].getY();
            minX = com[0].getX();
            minY = com[0].getY();
            foreach (Pointf i in com)
            {
                if (i.getX() > maxX) maxX = i.getX();
                else if (i.getX() < minX) minX = i.getX();
                if (i.getY() > maxY) maxY = i.getY();
                else if (i.getY() < minY) minY = i.getY();
            }
            foreach (Pointf i in cop)
            {
                if (i.getX() > maxX) maxX = i.getX();
                else if (i.getX() < minX) minX = i.getX();
                if (i.getY() > maxY) maxY = i.getY();
                else if (i.getY() < minY) minY = i.getY();
            }
            X = (maxX - minX) / 450f;
            Y = (maxY - minY) / 300f;
        }

        public Bitmap paint(bool COMCHECK, bool COPCHECK)
        {
            //450,300
            drawmap = new Bitmap(600, 400);
            Graphics gph = Graphics.FromImage(drawmap);
            gph.Clear(Color.White);
            gph.DrawLine(Pens.Black, new Point(20, 20), new Point(500, 20));
            gph.DrawPolygon(Pens.Black, new Point[3] { new Point(500, 17), new Point(500, 23), new Point(503, 20) });
            gph.FillPolygon(new SolidBrush(Color.Black), new Point[3] { new Point(500, 17), new Point(500, 23), new Point(503, 20) });
            gph.DrawString("X", new Font("宋体", 12), Brushes.Black, new PointF(510, 15));

            gph.DrawLine(Pens.Black, new Point(470, 18), new Point(470, 22));
            gph.DrawString(maxX.ToString(), new Font("宋体", 12), Brushes.Black, new PointF(450, 3));
            gph.DrawString(minX.ToString(), new Font("宋体", 12), Brushes.Black, new PointF(20, 3));

            gph.DrawLine(Pens.Black, new Point(20, 20), new Point(20, 350));
            gph.DrawPolygon(Pens.Black, new Point[3] { new Point(17, 350), new Point(23, 350), new Point(20, 353) });
            gph.FillPolygon(new SolidBrush(Color.Black), new Point[3] { new Point(17, 350), new Point(23, 350), new Point(20, 353) });
            gph.DrawString("COM/COP点图", new Font("宋体", 14), Brushes.Black, new PointF(220, 360));
            gph.DrawString("Y", new Font("宋体", 12), Brushes.Black, new PointF(15, 360));

            gph.DrawLine(Pens.Black, new Point(18, 320), new Point(22, 320));
            gph.DrawString(minY.ToString(), new Font("宋体", 12), Brushes.Black, new PointF(20, 23));
            gph.DrawString(maxY.ToString(), new Font("宋体", 12), Brushes.Black, new PointF(20, 320));

            int position = 0;
            int C = number / 510 + 1;

            if (COMCHECK)
            {

                foreach (Pointf i in com)
                {
                    
                    drawmap.SetPixel(20 + (int)((i.getX() - minX) / X), 20 + (int)((i.getY() - minY) / Y), Color.FromArgb((int)(position / C) > 255 ? 255 : (int)position / C, (int)(position / C) < 255 ? 0 : (int)position / C - 255, 255));
                    position++;
                }
            }

            if (COPCHECK)
            {
                position = 0;
                for (int i = 1; i < cop.Length; i++)
                {
                    //gph.DrawLine(new Pen(Color.FromArgb(255, (int)(position / C) > 255 ? 255 : (int)position / C, (int)(position / C) < 255 ? 0 : (int)position / C - 255, 255)), new PointF(20 + (int)((cop[i].getX() - minX) / X), 20 + (int)((cop[i].getY() - minY) / Y)), new PointF(20 + (int)((cop[i - 1].getX() - minX) / X), 20 + (int)((cop[i - 1].getY() - minY) / Y)));
                    gph.DrawLine(Pens.Gray, new PointF(20 + (int)((cop[i].getX() - minX) / X), 20 + (int)((cop[i].getY() - minY) / Y)), new PointF(20 + (int)((cop[i - 1].getX() - minX) / X), 20 + (int)((cop[i - 1].getY() - minY) / Y)));
                    position++;
                }
            }

            /*            foreach (Pointf i in cop)
            {
                
                drawmap.SetPixel(20 + (int)((i.getX() - minX) / X), 20 + (int)((i.getY() - minY) / Y), Color.FromArgb((int)(position / C) > 255 ? 255 : (int)position / C, (int)(position / C) < 255 ? 0 : (int)position / C - 255, 255));
                position++;
            }*/

            return drawmap;
        }


        public void free()
        {
            drawmap.Dispose();
            com = null;
            cop = null;
        }

    }
}
