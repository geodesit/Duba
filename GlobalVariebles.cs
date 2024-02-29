using DubaProject.Objects;

namespace DubaProject
{
    public static class GlobalVariebles
    {
        /// <summary>
        /// Global Variebles class
        /// </summary>
        public static string? pathToMobileMapPackage { get; set; }
        public static Point? PUPoint { get; set; }
        public static Point? UserPoint { get; set; }
        public static List<Point> UserPoints { get; set; } = new List<Point>();
        public static List<Point>? DubaPoints { get; set; } = new List<Point>();
        public static Point? dubaPoint { get; set; }
        public static bool PUclicked { get; set; }
        public static bool PUentered { get; set; }
        public static bool running { get; set; } = false;
        public static bool connected { get; set; } = true;
        public static bool UserPointClicked { get; set; } = false;

    }
}
