using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGLWinformsApplication1
{
    public class Pointf
    {
        static Int16 hashcode=0;

        double x;
        double y;
        double z;
       
        public Pointf()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public Pointf(double x_, double y_, double z_)
        {
            x = x_;
            y = y_;
            z = z_;
        }

        public Pointf(object x_)
        {
            string a= x_.GetType().Name;
            switch (x_.GetType().Name)
            {
                case "Double":
                    x = Convert.ToDouble(x_);
                    y = Convert.ToDouble(x_);
                    z = Convert.ToDouble(x_);
                    break;
                case "String":
                    x = Convert.ToDouble(x_);
                    y = Convert.ToDouble(x_);
                    z = Convert.ToDouble(x_);
                    break;
                case "Pointf":
                    x = ((Pointf)x_).getX();
                    y = ((Pointf)x_).getY();
                    z = ((Pointf)x_).getZ();
                    break;
                default: x = y = z = 0;
                    break;
            }
        }

        public void setX(double x_)
        {
            x = x_;
        }

        public void setY(double y_)
        {
            y = y_;
        }

        public void setZ(double z_)
        {
            z = z_;
        }

        public double getX()
        {
            return x;
        }

        public double getY()
        {
            return y;
        }

        public double getZ()
        {
            return z;
        }

        public double length(Pointf arg0)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(this.getX() - arg0.getX()), 2) + Math.Pow(Math.Abs(this.getY() - arg0.getY()), 2) + Math.Pow(Math.Abs(this.getZ() - arg0.getZ()), 2));
        }

        public static Pointf operator +(Pointf arg0, Pointf arg1)
        {
            return new Pointf(arg0.x + arg1.x, arg0.y + arg1.y, arg0.z + arg1.z);
        }

        public static Pointf operator -(Pointf arg0, Pointf arg1)
        {
            return new Pointf(arg0.x - arg1.x, arg0.y - arg1.y, arg0.z - arg1.z);
        }

        public static Pointf operator *(Pointf arg0, Pointf arg1)
        {
            return new Pointf(arg0.x * arg1.x, arg0.y * arg1.y, arg0.z * arg1.z);
        }

        public static Pointf operator /(Pointf arg0, Pointf arg1)
        {
            return new Pointf(arg0.x / arg1.x, arg0.y / arg1.y, arg0.z / arg1.z);
        }


        public static bool operator ==(Pointf arg0, Pointf arg1)
        {
            if ((arg0.getX() == arg1.getX()) & (arg0.getY() == arg1.getY()) & (arg0.getZ() == arg1.getZ())) return true;
            else return false;
        }

        public static bool operator !=(Pointf arg0, Pointf arg1)
        {
            if ((arg0.getX() == arg1.getX()) & (arg0.getY() == arg1.getY()) & (arg0.getZ() == arg1.getZ())) return false;
            else return true;
        }

        public override int GetHashCode()
        {
            return hashcode++;
        }

        public override bool Equals(Object obj)
        {
            if (obj is Pointf)
            {
                if (this == (Pointf)obj)
                    return true;
                else return false;
            }
            else return false;
        }

        public static explicit operator Pointf(String s)
        {
            return new Pointf(Convert.ToDouble(s), Convert.ToDouble(s), Convert.ToDouble(s));
        }

        public override string ToString()
        {
            return string.Format("X={0},Y={1},Z={2}",x,y,z);
        }
    }
}
