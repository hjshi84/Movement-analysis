using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SharpGLWinformsApplication1
{
    class DrawXYZ
    {
        public Bitmap bmx, bmy, bmz;
        static private Pointf[] data;
        private int number;
        private double maxY, minY, maxX, minX,maxZ,minZ;
        private double Z, Y, X;

        public DrawXYZ(Pointf[] data_)
        {
            data = data_;
            number = data.Length;
            maxX = data[0].getX();
            maxY = data[0].getY();
            minX = data[0].getX();
            minY = data[0].getY();
            maxZ = data[0].getZ();
            minZ = data[0].getZ();
            foreach (Pointf i in data)
            {
                if (i.getX() > maxX) maxX = i.getX();
                else if (i.getX() < minX) minX = i.getX();
                if (i.getY() > maxY) maxY = i.getY();
                else if (i.getY() < minY) minY = i.getY();
                if (i.getZ() > maxZ) maxZ = i.getZ();
                else if (i.getZ() < minZ) minZ = i.getZ();
            }
            X = ((maxX - minX)==0? 1:(maxX-minX)) / 300f;
            Y = ((maxY - minY)==0? 1:(maxY-minY)) / 300f;
            Z = ((maxZ - minZ)==0? 1:(maxZ-minZ)) / 300f;
            paint();
        }

        public void paint()
        {
            bmx = new Bitmap(600, 400);
            Graphics gph = Graphics.FromImage(bmx);
            gph.Clear(Color.White);
            gph.DrawLine(Pens.Black, new Point(20, 380), new Point(550, 380));
            gph.DrawPolygon(Pens.Black, new Point[3] { new Point(550, 377), new Point(550, 383), new Point(553, 380) });
            gph.FillPolygon(new SolidBrush(Color.Black), new Point[3] { new Point(550, 377), new Point(550, 383), new Point(553, 380) });
            gph.DrawString("T", new Font("宋体", 14), Brushes.Black, new PointF(560, 370));

            gph.DrawLine(Pens.Black, new Point(20, 380), new Point(20, 20));
            gph.DrawPolygon(Pens.Black, new Point[3] { new Point(17, 20), new Point(23, 20), new Point(20, 17) });
            gph.FillPolygon(new SolidBrush(Color.Black), new Point[3] { new Point(17, 20), new Point(23, 20), new Point(20, 17) });
            gph.DrawString("X", new Font("宋体", 14), Brushes.Black, new PointF(3, 10));

            int position = 1;
             for (int i = 1; i < data.Length; i++)
            {
                gph.DrawLine(Pens.Red, new Point((int)position / (number / 500) + 20, 380-(int)((data[i].getX() - minX) / X + 20)), new Point((int)(position-1)/ (number / 500) + 20,380- (int)((data[i-1].getX() - minX) / X + 20)));
                //bmx.SetPixel((int)position/(number/500)+20,(int)((i.getX()-minX)/X+20), Color.Red);
                position++;
            }
             
             bmy = new Bitmap(600, 400);
             gph = Graphics.FromImage(bmy);
             gph.Clear(Color.White);
             gph.DrawLine(Pens.Black, new Point(20, 380), new Point(550, 380));
             gph.DrawPolygon(Pens.Black, new Point[3] { new Point(550, 377), new Point(550, 383), new Point(553, 380) });
             gph.FillPolygon(new SolidBrush(Color.Black), new Point[3] { new Point(550, 377), new Point(550, 383), new Point(553, 380) });
             gph.DrawString("T", new Font("宋体", 14), Brushes.Black, new PointF(560, 370));

             gph.DrawLine(Pens.Black, new Point(20, 380), new Point(20, 20));
             gph.DrawPolygon(Pens.Black, new Point[3] { new Point(17, 20), new Point(23, 20), new Point(20, 17) });
             gph.FillPolygon(new SolidBrush(Color.Black), new Point[3] { new Point(17, 20), new Point(23, 20), new Point(20, 17) });
             gph.DrawString("Y", new Font("宋体", 14), Brushes.Black, new PointF(3, 10));

             position = 1;
             for (int i = 1; i < data.Length; i++)
             {
                 gph.DrawLine(Pens.Green, new Point((int)position / (number / 500) + 20, 380-(int)((data[i].getY() - minY) / Y + 20)), new Point((int)(position - 1) / (number / 500) + 20,380- (int)((data[i - 1].getY() - minY) / Y + 20)));
                 //bmx.SetPixel((int)position/(number/500)+20,(int)((i.getX()-minX)/X+20), Color.Red);
                 position++;
             }

             bmz = new Bitmap(600, 400);
             gph = Graphics.FromImage(bmz);
             gph.Clear(Color.White);
             gph.DrawLine(Pens.Black, new Point(20, 380), new Point(550, 380));
             gph.DrawPolygon(Pens.Black, new Point[3] { new Point(550, 377), new Point(550, 383), new Point(553, 380) });
             gph.FillPolygon(new SolidBrush(Color.Black), new Point[3] { new Point(550, 377), new Point(550, 383), new Point(553, 380) });
             gph.DrawString("T", new Font("宋体", 14), Brushes.Black, new PointF(560, 370));

             gph.DrawLine(Pens.Black, new Point(20, 380), new Point(20, 20));
             gph.DrawPolygon(Pens.Black, new Point[3] { new Point(17, 20), new Point(23, 20), new Point(20, 17) });
             gph.FillPolygon(new SolidBrush(Color.Black), new Point[3] { new Point(17, 20), new Point(23, 20), new Point(20, 17) });
             gph.DrawString("Z", new Font("宋体", 14), Brushes.Black, new PointF(3, 10));

             position = 1;
             for (int i = 1; i < data.Length; i++)
             {
                 gph.DrawLine(Pens.Blue, new Point((int)position / (number / 500) + 20, 380-(int)((data[i].getZ() - minZ) / Z + 20)), new Point((int)(position - 1) / (number / 500) + 20, 380-(int)((data[i - 1].getZ() - minZ) / Z + 20)));
                 //bmx.SetPixel((int)position/(number/500)+20,(int)((i.getX()-minX)/X+20), Color.Red);
                 position++;
             }

        }

        public void free()
        {
            bmx.Dispose();
            bmy.Dispose();
            bmz.Dispose();
            data = null;
        }


    }
}
