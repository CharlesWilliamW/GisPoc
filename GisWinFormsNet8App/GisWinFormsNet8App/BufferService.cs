using GMap.NET;
using GMap.NET.WindowsForms;

namespace GisWinFormsNet8App
{
    internal class BufferService
    {
        private readonly GMapControl _mapControl;
        private readonly GMapOverlay _bufferOverlay;

        public BufferService(GMapControl mapControl)
        {
            _mapControl = mapControl;
            _bufferOverlay = new GMapOverlay("buffer_overlay");
            _mapControl.Overlays.Add(_bufferOverlay);
        }

        public void CreateBuffer(PointLatLng center, double radiusInMeters)
        {
            _bufferOverlay.Polygons.Clear();

            var bufferPoints = new List<PointLatLng>();
            double earthRadius = 6371000;
            double latDegreesPerMeter = 1.0 / (earthRadius * Math.PI / 180.0);
            double lngDegreesPerMeter = 1.0 / (earthRadius * Math.PI / 180.0 * Math.Cos(ToRadians(center.Lat)));

            for (int i = 0; i < 36; i++)
            {
                double angleRad = ToRadians(i * 10.0);
                double offsetLat = radiusInMeters * Math.Cos(angleRad) * latDegreesPerMeter;
                double offsetLng = radiusInMeters * Math.Sin(angleRad) * lngDegreesPerMeter;
                bufferPoints.Add(new PointLatLng(center.Lat + offsetLat, center.Lng + offsetLng));
            }

            var polygon = new GMapPolygon(bufferPoints, "buffer")
            {
                Stroke = new Pen(Color.FromArgb(150, 0, 102, 204), 2),
                Fill = new SolidBrush(Color.FromArgb(50, 0, 102, 204))
            };
            _bufferOverlay.Polygons.Add(polygon);
        }

        public void ClearBuffer()
        {
            _bufferOverlay.Polygons.Clear();
            _mapControl.Refresh();
        }

        private static double ToRadians(double val) => val * Math.PI / 180.0;
    }
}
