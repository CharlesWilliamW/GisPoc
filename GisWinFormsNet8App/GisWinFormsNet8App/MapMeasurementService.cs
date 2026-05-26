using System;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace GisWinFormsNet8App
{
    public class MapMeasurementService
    {
        private readonly GMapControl _mapControl;
        private GMapOverlay _measureOverlay;
        // 專門管理緩衝區的圖層
        private GMapOverlay _bufferOverlay;

        // 記錄量測狀態
        private PointLatLng? _startPoint = null;
        private PointLatLng? _lastClickedPoint = null; // 記錄最後點擊的位置，供緩衝區即時生成使用

        public MapMeasurementService(GMapControl mapControl)
        {
            _mapControl = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
            InitializeOverlay();
        }

        /// <summary>
        /// 初始化獨立的量測圖層
        /// </summary>
        private void InitializeOverlay()
        {
            // 建立專屬的量測圖層，避免污染其他業務圖層
            _measureOverlay = new GMapOverlay("measure_overlay");
            _mapControl.Overlays.Add(_measureOverlay);

            _bufferOverlay = new GMapOverlay("BufferOverlay");
            _mapControl.Overlays.Add(_bufferOverlay);
        }

        /// <summary>
        /// 處理地圖點擊量測邏輯
        /// </summary>
        public string HandleMapClick(PointLatLng clickedPoint)
        {
            _lastClickedPoint = clickedPoint;

            if (_startPoint == null)
            {
                // 【第一點點擊】：設定起點
                ClearMeasurement();
                _startPoint = clickedPoint;

                // 插上綠色起點大頭針
                GMarkerGoogle _startMarker = new GMarkerGoogle(clickedPoint, GMarkerGoogleType.green)
                {
                    ToolTipText = "起點",
                    ToolTipMode = MarkerTooltipMode.Always
                };
                _measureOverlay.Markers.Add(_startMarker);

                return "距離：請點擊第二點...";
            }
            else
            {
                // 【第二點點擊】：設定終點，計算距離並連線
                PointLatLng endPoint = clickedPoint;

                // 插上藍色終點大頭針
                GMarkerGoogle _endMarker = new GMarkerGoogle(endPoint, GMarkerGoogleType.blue);
                _measureOverlay.Markers.Add(_endMarker);

                // 計算 Haversine 實際地理距離
                double distanceKm = CalculateHaversineDistance(_startPoint.Value, endPoint);

                // 更新終點標籤顯示公里數
                _endMarker.ToolTipText = $"終點\n距離: {distanceKm:F2} 公里";
                _endMarker.ToolTipMode = MarkerTooltipMode.Always;

                // 畫出兩點之間的連線 (GMapRoute)
                var points = new System.Collections.Generic.List<PointLatLng> { _startPoint.Value, endPoint };
                GMapRoute _measureRoute = new GMapRoute(points, "measure_line")
                {
                    Stroke = new Pen(Color.Blue, 3) // 藍色、粗細 3
                };
                _measureOverlay.Routes.Add(_measureRoute);

                // 提示使用者
                MessageBox.Show($"量測完成！\n點對點真實地理距離為：{distanceKm:F2} 公里", "距離量測提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 重設狀態，讓使用者下一次點擊可以重新測量
                _startPoint = null;

                // 強制地圖刷新渲染
                _mapControl.Refresh();
                return $"距離：{distanceKm:F2} 公里";
            }
        }

        /// <summary>
        /// 清除當前量測的圖資與狀態
        /// </summary>
        public void ClearMeasurement()
        {
            _startPoint = null;
            _lastClickedPoint = null;
            _measureOverlay.Markers.Clear();
            _measureOverlay.Routes.Clear();
            _mapControl.Refresh();
        }

        public void ClearBuffer()
        {
            _bufferOverlay.Polygons.Clear();
            _mapControl.Refresh();
        }

        /// <summary>
        /// 在指定的中心點建立指定半徑(公尺)的圓形緩衝區
        /// </summary>
        public void CreateBuffer(PointLatLng center, double radiusInMeters)
        {
            // 每次生成前先清空舊的緩衝區
            _bufferOverlay.Polygons.Clear();

            List<PointLatLng> bufferPoints = new List<PointLatLng>();
            int segments = 36; // 用 36 個點來擬合圓形（每 10 度一個點）

            // 地球半徑 (公尺)
            double earthRadius = 6371000;

            // 計算緯度與經度的每公尺轉換率
            double latDegreesPerMeter = 1.0 / (earthRadius * Math.PI / 180.0);
            double lngDegreesPerMeter = 1.0 / (earthRadius * Math.PI / 180.0 * Math.Cos(ToRadians(center.Lat)));

            for (int i = 0; i < segments; i++)
            {
                double angle = i * (360.0 / segments);
                double angleRad = ToRadians(angle);

                // 計算圓周上該角度點的偏移量
                double offsetLat = radiusInMeters * Math.Cos(angleRad) * latDegreesPerMeter;
                double offsetLng = radiusInMeters * Math.Sin(angleRad) * lngDegreesPerMeter;

                PointLatLng point = new PointLatLng(center.Lat + offsetLat, center.Lng + offsetLng);
                bufferPoints.Add(point);
            }

            // 建立 GMapPolygon (多邊形)
            GMapPolygon bufferPolygon = new GMapPolygon(bufferPoints, "BufferCircle")
            {
                // 外框顏色：半透明深藍
                Stroke = new Pen(Color.FromArgb(150, 0, 102, 204), 2),
                // 填滿顏色：高透明淺藍 (Alpha = 50 確保看得到底圖)
                Fill = new SolidBrush(Color.FromArgb(50, 0, 102, 204))
            };

            _bufferOverlay.Polygons.Add(bufferPolygon);
        }

        /// <summary>
        /// 🌟 提供 UI 控制緩衝區圖層的可見性
        /// </summary>
        public void SetBufferVisibility(bool visible, double defaultRadiusIfCreated = 1000)
        {
            _bufferOverlay.IsVisibile = visible;

            // 如果開啟時圖層是空的，且使用者之前有點擊過地圖，就自動在點擊處補上一個緩衝區
            if (visible && _bufferOverlay.Polygons.Count == 0 && _lastClickedPoint != null)
            {
                CreateBuffer(_lastClickedPoint.Value, defaultRadiusIfCreated);
            }
        }

        /// <summary>
        /// 核心數學公式：Haversine Formula (計算地球曲面大圓距離)
        /// 對於具備遊戲物理引擎經驗的你，這段邏輯非常直覺
        /// </summary>
        private double CalculateHaversineDistance(PointLatLng pos1, PointLatLng pos2)
        {
            double R = 6371.0; // 地球平均半徑 (公里)

            // 轉弧度 (Radians)
            double lat1Rad = ToRadians(pos1.Lat);
            double lat2Rad = ToRadians(pos2.Lat);

            double deltaLat = ToRadians(pos2.Lat - pos1.Lat);
            double deltaLng = ToRadians(pos2.Lng - pos1.Lng);

            // Haversine 公式
            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLng / 2) * Math.Sin(deltaLng / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // 回傳公里
        }

        private double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}
