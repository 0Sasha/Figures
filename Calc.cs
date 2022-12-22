using System;
using System.Drawing;

namespace Figures
{
    // Класс Calc, как набор статических методов для тех, кто не хочет создавать экземпляры фигур
    public static class Calc
    {
        public static double GetAreaCircleByRadius(double radius)
        {
            if (double.IsNegative(radius) || double.IsNaN(radius))
                throw new ArgumentException("Радиус не может быть отрицательным или равным NaN", nameof(radius));

            return radius * radius * Math.PI;
        }

        public static double GetAreaTriangleBySides(double a, double b, double c)
        {
            if (a <= 0 || b <= 0 || c <= 0 || double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(c))
                throw new ArgumentException("Значение аргумента NaN или <= 0", nameof(a));

            if (a + b - c < double.Epsilon || a + c - b < double.Epsilon || b + c - a < double.Epsilon)
                throw new ArgumentException("Сумма длин двух сторон должна быть больше длины третьей стороны", nameof(a));

            double s = (a + b + c) / 2;
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }

        public static bool IsRightTriangle(double a, double b, double c)
        {
            if (a <= 0 || b <= 0 || c <= 0 || double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(c))
                throw new ArgumentException("Значение аргумента NaN или <= 0", nameof(a));

            if (a + b - c < double.Epsilon || a + c - b < double.Epsilon || b + c - a < double.Epsilon)
                throw new ArgumentException("Сумма длин двух сторон должна быть больше длины третьей стороны", nameof(a));

            if (a > b && a > c && Math.Abs(a * a - (b * b + c * c)) <= double.Epsilon ||
                b > a && b > c && Math.Abs(b * b - (a * a + c * c)) <= double.Epsilon ||
                c > a && c > b && Math.Abs(c * c - (a * a + b * b)) <= double.Epsilon) return true;
            return false;
        }

        public static double GetAreaEquilateralPolygon(int numberOfSides, double lengthOfSide)
        {
            if (numberOfSides < 3) throw new ArgumentException("Количество сторон не может быть меньше 3", nameof(numberOfSides));

            if (double.IsNegative(lengthOfSide) || double.IsNaN(lengthOfSide))
                throw new ArgumentException("Длина стороны не может быть отрицательной или равной NaN", nameof(lengthOfSide));

            return numberOfSides * (lengthOfSide * lengthOfSide) / (4 * Math.Tan(180 / numberOfSides * (Math.PI / 180)));
        }

        public static double GetAreaArbitraryFigure(params Point[] vertices)
        {
            if (vertices == null) throw new ArgumentNullException(nameof(vertices));

            if (vertices.Length == 0) throw new ArgumentException("Длина массива равна нулю", nameof(vertices));

            checked
            {
                long res = vertices[0].Y * vertices[^1].X - vertices[0].X * vertices[^1].Y;
                for (int i = 1; i < vertices.Length; i++)
                    res += vertices[i - 1].X * vertices[i].Y - vertices[i - 1].Y * vertices[i].X;
                return Math.Abs(res) / 2;
            }
        }
    }
}
