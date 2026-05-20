using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace GisWinFormsNet8App
{
    public partial class Form1 : Form
    {
        // 宣告我們抽離出來的服務
        private readonly MapDataService _dataService;
        private readonly DisasterOverlayManager _disasterManager;
        private MapMeasurementService _measurementService;

        public Form1()
        {
            InitializeComponent();
            InitializeGisMap();

            // 初始化服務，並把地圖控制項傳進去
            _dataService = new MapDataService();
            _disasterManager = new DisasterOverlayManager(gMapControl1);
            _measurementService = new MapMeasurementService(gMapControl1);

            // 程式碼綁定 MouseClick 事件
            gMapControl1.MouseClick += GMapControl1_MouseClick;
        }

        // 1. 初學者第一步：初始化免費圖資
        private void InitializeGisMap()
        {
            try
            {
                // 使用 WikiMapia 開源圖資
                gMapControl1.MapProvider = GMapProviders.WikiMapiaMap;

                // 設定初始畫面定位在台灣台北市 (經緯度座標)
                gMapControl1.Position = new PointLatLng(25.0330, 121.5654);

                // 設定縮放級別 (Zoom)，數字越大看越細
                gMapControl1.MinZoom = 5;
                gMapControl1.MaxZoom = 18;
                gMapControl1.Zoom = 13;

                // 實作 GIS 必備互動：允許使用者按住滑鼠「左鍵」來拖曳平移地圖
                gMapControl1.DragButton = MouseButtons.Left;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"地圖引擎初始化失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 當地圖被點擊時，直接外包給 Service 處理
        /// </summary>
        private void GMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 將畫面上的像素座標 (X, Y) 轉換為地理經緯度 (Lat, Lng)
                PointLatLng clickedPoint = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                // 情況 A：如果「測距模式」有開啟
                if (chkMeasureMode != null && chkMeasureMode.Checked)
                {
                    string resultMessage = _measurementService.HandleMapClick(clickedPoint);
                }

                // 情況 B：如果「緩衝區模式」有開啟 (不論測距有無開啟，皆可單獨運作)
                if (chkBufferMode.Checked)
                {
                    // 以 clickedPoint 為中心，建立一個災害緩衝圈
                    double bufferRadiusInMeters = 500; // 緩衝區半徑
                    _measurementService.CreateBuffer(clickedPoint, bufferRadiusInMeters);
                }
            }
        }

        private void chkBufferMode_CheckedChanged(object sender, EventArgs e)
        {
            // 切換圖層可見度
            _measurementService.SetBufferVisibility(chkBufferMode.Checked);

            // 刷新地圖確保即時渲染
            gMapControl1.Refresh();
        }

        // 用來追蹤 Toggle 狀態的變數（如果使用一般 Button）
        private bool _isDisasterShown = false;
        private async void btnToggleDisaster_CheckedChanged(object sender, EventArgs e)
        {
            // 狀態取反
            _isDisasterShown = !_isDisasterShown;
            if (_isDisasterShown)
            {
                // 變更 UI 按鈕文字提示
                btnToggleDisaster.Text = "隱藏災害觀測點";

                // 停用按鈕，防止使用者在非同步載入期間重複連續點擊
                btnToggleDisaster.Enabled = false;

                try
                {
                    // 呼叫獨立的 DataService 抓取資料
                    var data = await _dataService.FetchDisasterPoints();

                    // 呼叫圖層管理器顯示並渲染
                    _disasterManager.ToggleVisibility(true, data);
                }
                finally
                {
                    btnToggleDisaster.Enabled = true;
                }
            }
            else
            {
                btnToggleDisaster.Text = "顯示災害觀測點";
                // 隱藏圖層
                _disasterManager.ToggleVisibility(false);
            }
        }

        private void btnClearMeasure_Click(object sender, EventArgs e)
        {
            _measurementService.ClearMeasurement();
        }
    }
}
