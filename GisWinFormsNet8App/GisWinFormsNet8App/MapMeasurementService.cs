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

        // 記錄量測狀態
        private PointLatLng? _startPoint = null;
        private GMarkerGoogle _startMarker = null;
        private GMarkerGoogle _endMarker = null;
        private GMapRoute _measureRoute = null;

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
            _measureOverlay = new GMapOverlay("measure_overlay");
            _mapControl.Overlays.Add(_measureOverlay);
        }

        /// <summary>
        /// 處理地圖點擊量測邏輯
        /// </summary>
        public void HandleMapClick(MouseEventArgs e)
        {
            // 只處理滑鼠左鍵點擊
            if (e.Button != MouseButtons.Left) return;

            // 將畫面上的像素座標 (X, Y) 轉換為地理經緯度 (Lat, Lng)
            PointLatLng clickedPoint = _mapControl.FromLocalToLatLng(e.X, e.Y);

            if (_startPoint == null)
            {
                // 【第一點點擊】：設定起點
                ClearMeasurement();
                _startPoint = clickedPoint;

                // 插上綠色起點大頭針
                _startMarker = new GMarkerGoogle(clickedPoint, GMarkerGoogleType.green)
                {
                    ToolTipText = "起點",
                    ToolTipMode = MarkerTooltipMode.Always
                };
                _measureOverlay.Markers.Add(_startMarker);
            }
            else
            {
                // 【第二點點擊】：設定終點，計算距離並連線
                PointLatLng endPoint = clickedPoint;

                // 插上紅色終點大頭針
                _endMarker = new GMarkerGoogle(endPoint, GMarkerGoogleType.blue);
                _measureOverlay.Markers.Add(_endMarker);

                // 計算 Haversine 實際地理距離
                double distanceKm = CalculateHaversineDistance(_startPoint.Value, endPoint);

                // 更新終點標籤顯示公里數
                _endMarker.ToolTipText = $"終點\n距離: {distanceKm:F2} 公里";
                _endMarker.ToolTipMode = MarkerTooltipMode.Always;

                // 畫出兩點之間的連線 (GMapRoute)
                var points = new System.Collections.Generic.List<PointLatLng> { _startPoint.Value, endPoint };
                _measureRoute = new GMapRoute(points, "measure_line")
                {
                    Stroke = new Pen(Color.Blue, 3) // 藍色、粗細 3
                };
                _measureOverlay.Routes.Add(_measureRoute);

                // 提示使用者
                MessageBox.Show($"量測完成！\n點對點真實地理距離為：{distanceKm:F2} 公里", "距離量測提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 重設狀態，讓使用者下一次點擊可以重新測量
                _startPoint = null;
            }

            // 強制地圖刷新渲染
            _mapControl.Refresh();
        }

        /// <summary>
        /// 清除當前量測的圖資與狀態
        /// </summary>
        public void ClearMeasurement()
        {
            _startPoint = null;
            _measureOverlay.Markers.Clear();
            _measureOverlay.Routes.Clear();
            _mapControl.Refresh();
        }

        /// <summary>
        /// 核心數學公式：Haversine Formula (計算地球曲面大圓距離)
        /// 對於具備遊戲物理引擎經驗的你，這段邏輯非常直覺
        /// </summary>
        private double CalculateHaversineDistance(PointLatLng pos1, PointLatLng pos2)
        {
            double R = 6371.0; // 地球平均半徑 (公里)

            // 轉弧度 (Radians)
            double lat1Rad = Math.PI * pos1.Lat / 180.0;
            double lat2Rad = Math.PI * pos2.Lat / 180.0;

            double deltaLat = Math.PI * (pos2.Lat - pos1.Lat) / 180.0;
            double deltaLng = Math.PI * (pos2.Lng - pos1.Lng) / 180.0;

            // Haversine 公式
            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLng / 2) * Math.Sin(deltaLng / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // 回傳公里
        }
    }
}
