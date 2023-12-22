using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace xstrat
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        public static string RemoveAllWhitespace(this string str)
        {
            if (str == null) return null;
            return string.Concat(str.Where(c => !char.IsWhiteSpace(c)));
        }
        public static bool IsDigitsOnly(this string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        public static bool ToBool(this string input)
        {
            return (input.Trim() == "1");
        }
    }
    public static class DateTimeExtensions
    {
        public static DateTime SetTime(this DateTime dateTime, int hour, int minute, int second)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, second, dateTime.Kind);
        }
    }
    public static class ListExtensions
    {
        // Extension method to make the list empty if it's null
        public static List<T> EmptyIfNull<T>(this List<T> list)
        {
            return list ?? new List<T>();
        }
    }

    public static class FrameworkElementExtensions
    {
        public static double GetCenterX(this FrameworkElement element)
        {
            if(element is Path)
            {
                return (element as Path).GetDistanceFromCanvasLeft() + (element as Path).ActualWidth / 2.0;
            }

            var pos = element.TranslatePoint(new Point(0,0), element.Parent as UIElement);
            return pos.X + element.ActualWidth / 2.0;
        }
        public static double GetCenterY(this FrameworkElement element)
        {
            if(element is Path)
            {
                return (element as Path).GetDistanceFromCanvasTop() + (element as Path).ActualHeight / 2.0;
            }

            var pos = element.TranslatePoint(new Point(0, 0), element.Parent as UIElement);
            return pos.Y + element.ActualHeight / 2.0;
        }
        public static double GetLeft(this FrameworkElement element)
        {
            if(element is Path)
            {
                return (element as Path).GetDistanceFromCanvasLeft();
            }
            var pos = element.TranslatePoint(new Point(0, 0), element.Parent as UIElement);
            return pos.X;
        }
        public static double GetTop(this FrameworkElement element)
        {
            if(element is Path)
            {
                return (element as Path).GetDistanceFromCanvasTop();
            }
            var pos = element.TranslatePoint(new Point(0, 0), element.Parent as UIElement);
            return pos.Y;
        }
    }
    public static class PathExtensions
    {
        public static double GetDistanceFromCanvasLeft(this Path arrowPath)
        {
            // Ensure the arrow is not null
            if (arrowPath == null)
            {
                throw new ArgumentNullException("ArrowPath must not be null.");
            }

            // Get the bounding box of the arrow
            Geometry geometry = arrowPath.Data;
            if (geometry == null)
            {
                throw new InvalidOperationException("ArrowPath does not have valid geometry data.");
            }

            // Assume the stroke thickness is known or retrieve it from the arrowPath
            double penThickness = 1.0; // Adjust as needed based on your actual arrow stroke thickness
            Pen drawingPen = new Pen(arrowPath.Stroke, penThickness);
            Rect arrowBounds = geometry.GetRenderBounds(drawingPen);

            // Get the left position of the arrow within its parent (likely a Canvas)
            double arrowLeft = arrowBounds.Left;

            // Canvas.GetLeft returns the offset of the left edge of an element from the left side of its parent Canvas.
            // If the arrow is not directly on the canvas (e.g., nested in another container), this value will be necessary.
            double canvasLeftOffset = Canvas.GetLeft(arrowPath);

            // If canvasLeftOffset is NaN, the arrowPath may not have an explicit set value. Assuming 0 for such cases.
            if (double.IsNaN(canvasLeftOffset))
            {
                canvasLeftOffset = 0;
            }

            // Calculate the distance from the arrow's left edge to the canvas's left border
            double distanceFromCanvasLeft = arrowLeft + canvasLeftOffset;

            return distanceFromCanvasLeft;
        }

        public static double GetDistanceFromCanvasTop(this Path arrowPath)
        {
            // Ensure the arrow is not null
            if (arrowPath == null)
            {
                throw new ArgumentNullException("ArrowPath must not be null.");
            }

            // Get the bounding box of the arrow
            Geometry geometry = arrowPath.Data;
            if (geometry == null)
            {
                throw new InvalidOperationException("ArrowPath does not have valid geometry data.");
            }

            // Assume the stroke thickness is known or retrieve it from the arrowPath
            double penThickness = 1.0; // Adjust as needed based on your actual arrow stroke thickness
            Pen drawingPen = new Pen(arrowPath.Stroke, penThickness);
            Rect arrowBounds = geometry.GetRenderBounds(drawingPen);

            // Get the top position of the arrow within its parent (likely a Canvas)
            double arrowTop = arrowBounds.Top;

            // Canvas.GetTop returns the offset of the top edge of an element from the top side of its parent Canvas.
            // If the arrow is not directly on the canvas (e.g., nested in another container), this value will be necessary.
            double canvasTopOffset = Canvas.GetTop(arrowPath);

            // If canvasTopOffset is NaN, the arrowPath may not have an explicit set value. Assuming 0 for such cases.
            if (double.IsNaN(canvasTopOffset))
            {
                canvasTopOffset = 0;
            }

            // Calculate the distance from the arrow's top edge to the canvas's top border
            double distanceFromCanvasTop = arrowTop + canvasTopOffset;

            return distanceFromCanvasTop;
        }
    }

}
