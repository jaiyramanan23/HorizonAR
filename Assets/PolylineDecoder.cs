using System.Collections.Generic;
using UnityEngine;

public static class PolylineDecoder
{
    public static Vector2[] Decode(string encodedPoints)
    {
        List<Vector2> points = new List<Vector2>();

        int index = 0;
        int latitude = 0;
        int longitude = 0;

        while (index < encodedPoints.Length)
        {
            int shift = 0;
            int result = 0;

            int b;
            do
            {
                b = encodedPoints[index++] - 63;
                result |= (b & 0x1F) << shift;
                shift += 5;
            } while (b >= 0x20);

            int dLatitude = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
            latitude += dLatitude;

            shift = 0;
            result = 0;

            do
            {
                b = encodedPoints[index++] - 63;
                result |= (b & 0x1F) << shift;
                shift += 5;
            } while (b >= 0x20);

            int dLongitude = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
            longitude += dLongitude;

            points.Add(new Vector2((latitude * 1E-5f), (longitude * 1E-5f)));
        }

        return points.ToArray();
    }
}
