using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGPProject
{
    abstract class Shape
    {
        // This is the base class for Shapes in the application. It should allow an array or LL
        // to be created containing different kinds of shapes.
        public Shape()   // constructor
        {

        }

        public abstract void Draw(Graphics g, Pen blackPen, int shapeCount);

        public abstract void Rotate(int angle);

        public abstract bool Contains(Point point);

        public Point RotatePoint(Point point, Point center, double angle)
        {
            int newX = (int)(center.X + (point.X - center.X) * Math.Cos(angle) - (point.Y - center.Y) * Math.Sin(angle));
            int newY = (int)(center.Y + (point.X - center.X) * Math.Sin(angle) + (point.Y - center.Y) * Math.Cos(angle));
            return new Point(newX, newY);
        }
    }

    class Square : Shape
    {
        //This class contains the specific details for a square defined in terms of opposite corners
        Point keyPt, oppPt;      // these points identify opposite corners of the square

        public Square(Point keyPt, Point oppPt)   // constructor
        {
            this.keyPt = keyPt;
            this.oppPt = oppPt;
        }

        public override void Rotate(int angle)
        {
            // Get the center of the square
            int centerX = (keyPt.X + oppPt.X) / 2;
            int centerY = (keyPt.Y + oppPt.Y) / 2;

            // Convert angle to radians
            double radians = angle * (Math.PI / 180);

            // Calculate new positions for each corner after rotation
            Point newKeyPt = RotatePoint(keyPt, new Point(centerX, centerY), radians);
            Point newOppPt = RotatePoint(oppPt, new Point(centerX, centerY), radians);

            // Update the square's corner positions
            keyPt = newKeyPt;
            oppPt = newOppPt;
        }

        // You will need a different draw method for each kind of shape. Note the square is drawn
        // from first principles. All other shapes should similarly be drawn from first principles. 
        // Ideally no C# standard library class or method should be used to create, draw or transform a shape
        // and instead should utilse user-developed code.
        public override void Draw(Graphics g, Pen blackPen, int shapeCount)
        {
            // This method draws the square by calculating the positions of the other 2 corners
            double xDiff, yDiff, xMid, yMid;   // range and mid points of x & y  

            // calculate ranges and mid points
            xDiff = oppPt.X - keyPt.X;
            yDiff = oppPt.Y - keyPt.Y;
            xMid = (oppPt.X + keyPt.X) / 2;
            yMid = (oppPt.Y + keyPt.Y) / 2;

            // draw square
            g.DrawLine(blackPen, (int)keyPt.X, (int)keyPt.Y, (int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2));
            g.DrawLine(blackPen, (int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2), (int)oppPt.X, (int)oppPt.Y);
            g.DrawLine(blackPen, (int)oppPt.X, (int)oppPt.Y, (int)(xMid - yDiff / 2), (int)(yMid + xDiff / 2));
            g.DrawLine(blackPen, (int)(xMid - yDiff / 2), (int)(yMid + xDiff / 2), (int)keyPt.X, (int)keyPt.Y);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            g.DrawString((shapeCount + 1).ToString(), new Font("Arial", 12), Brushes.Black, (float)xMid, (float)yMid, stringFormat);
        }

        public override bool Contains(Point point)
        {
            // Check if the point is within the square
            return point.X >= keyPt.X && point.X <= oppPt.X && point.Y >= keyPt.Y && point.Y <= oppPt.Y;
        }
        public void MoveShape(int x, int y)
        {
            keyPt.X += x;
            keyPt.Y += y;
            oppPt.X += x;
            oppPt.Y += y;
        }
    }

    class Triangle : Shape
    {
        //This class contains the specific details for a triangle defined in terms of its 3 corners
        Point corner1, corner2, corner3;      // these points identify the 3 corners of the triangle

        public Triangle(Point corner1, Point corner2, Point corner3)   // constructor
        {
            this.corner1 = corner1;
            this.corner2 = corner2;
            this.corner3 = corner3;
        }
        public override void Rotate(int angle)
        {
            // Get the centroid of the triangle
            int xMid = (corner1.X + corner2.X + corner3.X) / 3;
            int yMid = (corner1.Y + corner2.Y + corner3.Y) / 3;

            // Convert angle to radians
            double radians = angle * (Math.PI / 180);

            // Calculate new positions for each corner after rotation
            Point newCorner1 = RotatePoint(corner1, new Point(xMid, yMid), radians);
            Point newCorner2 = RotatePoint(corner2, new Point(xMid, yMid), radians);
            Point newCorner3 = RotatePoint(corner3, new Point(xMid, yMid), radians);

            // Update the triangle's corner positions
            corner1 = newCorner1;
            corner2 = newCorner2;
            corner3 = newCorner3;
        }

        public override void Draw(Graphics g, Pen blackPen, int shapeCount)
        {
            // This method draws the triangle by calculating the positions of the other 2 corners
            g.DrawLine(blackPen, corner1, corner2);
            g.DrawLine(blackPen, corner2, corner3);
            g.DrawLine(blackPen, corner3, corner1);

            // calculate centroid
            int xMid = (corner1.X + corner2.X + corner3.X) / 3;
            int yMid = (corner1.Y + corner2.Y + corner3.Y) / 3;

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            g.DrawString((shapeCount + 1).ToString(), new Font("Arial", 12), Brushes.Black, (float)xMid, (float)yMid, stringFormat);
        }

        public override bool Contains(Point point)
        {
            // Check if the point is within the triangle
            int area = Math.Abs((corner1.X * (corner2.Y - corner3.Y) + corner2.X * (corner3.Y - corner1.Y) + corner3.X * (corner1.Y - corner2.Y)) / 2);
            int area1 = Math.Abs((corner1.X * (corner2.Y - point.Y) + corner2.X * (point.Y - corner1.Y) + point.X * (corner1.Y - corner2.Y)) / 2);
            int area2 = Math.Abs((corner1.X * (point.Y - corner3.Y) + point.X * (corner3.Y - corner1.Y) + corner3.X * (corner1.Y - point.Y)) / 2);
            int area3 = Math.Abs((point.X * (corner2.Y - corner3.Y) + corner2.X * (corner3.Y - point.Y) + corner3.X * (point.Y - corner2.Y)) / 2);

            return area == area1 + area2 + area3;
        }

        public void MoveShape(int x, int y)
        {
            corner1.X += x;
            corner1.Y += y;
            corner2.X += x;
            corner2.Y += y;
            corner3.X += x;
            corner3.Y += y;
        }
    }

    class Circle : Shape
    {
        //This class contains the specific details for a circle defined in terms of its centre and radius
        Point point1, point2;   // these points identify the 2 opposite sides of the circle

        public Circle(Point point1, Point point2)   // constructor
        {
            this.point1 = point1;
            this.point2 = point2;
        }
        public override void Draw(Graphics g, Pen blackPen, int shapeCount)
        {
            int diameter = (int)Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));

            int centerX = (point1.X + point2.X) / 2;
            int centerY = (point1.Y + point2.Y) / 2;

            g.DrawEllipse(blackPen, centerX - diameter / 2, centerY - diameter / 2, diameter, diameter);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            g.DrawString((shapeCount + 1).ToString(), new Font("Arial", 12), Brushes.Black, (float)centerX, (float)centerY, stringFormat);
        }
        public override void Rotate(int angle)
        {
            // Rotating a circle is a bit silly, so we'll just leave it as is
        }

        public override bool Contains(Point point)
        {
            // Check if the point is within the circle
            int radius = (int)Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
            return Math.Pow(point.X - point1.X, 2) + Math.Pow(point.Y - point1.Y, 2) <= Math.Pow(radius, 2);
        }
        public void MoveShape(int x, int y)
        {
            point1.X += x;
            point1.Y += y;
            point2.X += x;
            point2.Y += y;
        }
    }
}
