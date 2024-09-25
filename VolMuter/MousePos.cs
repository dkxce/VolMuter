//
// C#
// VolMuter.MousePos
// v 0.1, 25.09.2024
// https://github.com/dkxce/VolMuter
// en,ru,1251,utf-8
//

using System.Drawing;
using System.Runtime.InteropServices;

namespace VolMuter
{
    public static class MousePos
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
            public static implicit operator Point(POINT point) => new Point(point.X, point.Y);
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            bool success = GetCursorPos(out lpPoint);
            if (success) return lpPoint;
            return Point.Empty;
        }

    }

}
