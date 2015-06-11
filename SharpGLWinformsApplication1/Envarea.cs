using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGLWinformsApplication1
{
    class Envarea
    {
        private int num = 0;
        private int envnum = 0;
        private Pointf[] Points;
        private Pointf[] EnvPoints;
        private Pointf[] TmpPoints;
        private double Area;

        public Envarea(Pointf[] arg, int number)
        {
            Points = new Pointf[number];
            EnvPoints = new Pointf[number];
            TmpPoints = new Pointf[number];
            num = number;
            for (int i = 0; i < num; ++i)
                Points[i] = arg[i];
            Area = 0;

        }

        public double cross(Pointf org, Pointf dst1, Pointf dst2, int arg)//叉积
        {
            switch (arg)
            {
                case 2:
                    return (dst1.getX() - org.getX()) * (dst2.getY() - org.getY()) - (dst1.getY() - org.getY()) * (dst2.getX() - org.getX());
                //case 3:
                //    return new Pointf((dst1.getY() - org.getY()) * (dst2.getZ() - org.getZ()) - (dst1.getZ() - org.getZ()) * (dst2.getY() - org.getY())
                //        , ((dst1.getX() - org.getX()) * (dst1.getZ() - org.getZ()) - (dst2.getZ() - org.getZ()) * (dst1.getX() - org.getX()))
                //        , ((dst1.getX() - org.getX()) * (dst2.getY() - org.getY()) - (dst1.getY() - org.getY()) * (dst2.getX() - org.getX())));
                //｛A,B,C}*｛m,n,p｝=(bp-cn)i+(mc-pa)j+(an-bm)k
            }
            return 0;
        }

        public int sgn(double x)//判断符号，注意极小值的情况
        {
            return (Math.Abs(x) < 1e-12 ? 0 : (x > 0 ? 1 : -1));
        }

        public double getarea()
        {
            envnum = 0;
            Pointf Max = Points[0];
            Pointf Min = Points[0];
            for (int i = 0; i < num; ++i)
            {
                if (Points[i].getX() > Max.getX())
                    Max = Points[i];
                if (Points[i].getX() < Min.getX())
                    Min = Points[i];
            }
            EnvPoints[envnum++] = Min;
            int index1 = 0;
            for (int i = 0; i < num; ++i)
            {
                double result = cross(Min, Max, Points[i], 2);
                if (sgn(result) > 0)//叉乘。左侧。
                {
                    TmpPoints[index1++] = Points[i];
                }
            }
            Hull(0, index1, Min, Max);
            EnvPoints[envnum++] = Max;

            //收集下部。
            index1 = 0;
            for (int i = 0; i < num; ++i)    // i=0?
            {
                double result = cross(Min, Max, Points[i], 2);
                if (sgn(result) < 0)//叉乘。左侧。
                {
                    //PointF temp = Points[i];
                    //Points[i] = Points[index1];
                    //Points[index1++] = temp;
                    TmpPoints[index1++] = Points[i];
                }
            }
            Hull(0, index1, Max, Min);

            //至此。应该得到了envelopnum，envedot[10000]。全部的值。

            //计算包络面积
            Area = 0;
            for (int l = 1; l < envnum - 1; l++)
                Area += Math.Abs(cross(EnvPoints[0], EnvPoints[l], EnvPoints[l + 1], 2)) / 2;
            return Area;
        }

        private void Hull(int i, int j, Pointf a, Pointf b)  //算法
        {
            //快速凸包算法，详见：http://www.cnblogs.com/Booble/archive/2011/03/10/1980089.html。
            //小技巧：CROSS可以用来直接做距离的比较。

            //1 void 快速凸包(P:点集 , S:向量 /*S.p,S.q:点)*/ )
            //{ 2 　　/* P 在 S 左侧*/ 
            //  3 　　选取 P 中距离 S 最远的 点 Y ; 
            //  4 　　向量 A <- { S.p , Y } ; 向量 B <- { Y , S.q } ; 
            //  5 　　点集 Q <- 在 P 中 且在 A 左侧的点 ; 
            //  6 　　点集 R <- 在 P 中 且在 B 左侧的点 ; /* 划分 */ 
            //  7 　　快速凸包 ( Q , A ) ; /* 分治 */ 
            //  8 　　输出 (点 Y) ; /* 按中序输出 保证顺序*/ 
            //  9 　　快速凸包 ( P , B ) ; /* 分治 */
            //  10 }

            //何时停止：没点？
            if (j == i)
            {
                return;
            }

            Pointf Farest = new Pointf();
            double dis = 0;
            for (int k = i; k < j; k++)
            {//FIND THE MAX DISTANCE
                if (cross(a, b, TmpPoints[k], 2) > dis)
                {
                    dis = cross(a, b, TmpPoints[k], 2);
                    Farest = TmpPoints[k];
                }
            }
            int index1 = i;
            //doPOINT temp[10000];//这种千万不能写，因为在递归的过程中会占用大量的内存空间。
            //划分两边点。确定区域。
            for (int k = i; k < j; k++)
            {//左边拖一起。
                double pos = cross(a, Farest, TmpPoints[k], 2);

                if (sgn(pos) > 0)
                {
                    Pointf temp = TmpPoints[k];
                    TmpPoints[k] = TmpPoints[index1];
                    TmpPoints[index1++] = temp;
                }
            }

            //左HULL。
            Hull(i, index1, a, Farest);
            //输出。
            EnvPoints[envnum++] = Farest;
            int index2 = index1;
            index1 = i;
            for (int k = i; k < j; k++)
            {//右边拖一起。
                double pos = cross(Farest, b, TmpPoints[k], 2);

                if (sgn(pos) > 0)
                {
                    Pointf temp = TmpPoints[k];
                    TmpPoints[k] = TmpPoints[index1];
                    TmpPoints[index1++] = temp;
                }
            }
            //右HULL。
            Hull(i, index1, Farest, b);
        }
    }
}
