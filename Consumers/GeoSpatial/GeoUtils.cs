using System;
using System.Text;

namespace Odin.Consumers.GeoSpatial
{
    public static class QuadkeyUtils
    {
        const double MinLatitude = -85.05112878;
        const double MaxLatitude = 85.05112878;
        const double MinLongitude = -180;
        const double MaxLongitude = 180;
        const int DetailLevel = 5;

        public static string GetQuadKey(this Position value)
        {
            int pixelX, pixelY, tileX, tileY;
            LatLongToPixelXY(value.Latitude, value.Longitude, DetailLevel, out pixelX, out pixelY);
            PixelXYToTileXY(pixelX, pixelY, out tileX, out tileY);
            string qaudkey = TileXYToQuadKey(tileX, tileY, DetailLevel);
            return qaudkey;
        }

        private static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        public static uint MapSize(int levelOfDetail)
        {
            return (uint)256 << levelOfDetail;
        }

        public static void LatLongToPixelXY(double latitude, double longitude, int levelOfDetail, out int pixelX, out int pixelY)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            longitude = Clip(longitude, MinLongitude, MaxLongitude);

            double x = (longitude + 180) / 360;
            double sinLatitude = Math.Sin(latitude * Math.PI / 180);
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            uint mapSize = MapSize(levelOfDetail);
            pixelX = (int)Clip(x * mapSize + 0.5, 0, mapSize - 1);
            pixelY = (int)Clip(y * mapSize + 0.5, 0, mapSize - 1);
        }

        public static void PixelXYToTileXY(int pixelX, int pixelY, out int tileX, out int tileY)
        {
            tileX = pixelX / 256;
            tileY = pixelY / 256;
        }

        public static string TileXYToQuadKey(int tileX, int tileY, int levelOfDetail)
        {
            StringBuilder quadKey = new StringBuilder();
            for (int i = levelOfDetail; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);
                if ((tileX & mask) != 0)
                {
                    digit++;
                }
                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }
                quadKey.Append(digit);
            }
            return quadKey.ToString();
        }
    }
}
