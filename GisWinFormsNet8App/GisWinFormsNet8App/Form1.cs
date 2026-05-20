using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GisWinFormsNet8App
{
    public partial class Form1 : Form
    {
        private Panel panelSidebar;
        private Panel panelMap;

        // 宣告我們抽離出來的服務
        private readonly MapDataService _dataService;
        private readonly DisasterOverlayManager _disasterManager;
        private MapMeasurementService _measurementService;

        // 解決滑鼠左鍵「拖曳地圖」與「點擊設點」衝突的關鍵變數
        private bool _isMouseDown = false;
        private Point _mouseDownPoint;
        private const int ClickThreshold = 3; // 定義點擊與拖曳的位移閾值（像素）

        public Form1()
        {
            InitializeComponent();

            // 2. 動態初始化 Panel 並重組 UI 階層
            InitializeDynamicPanels();

            InitializeCustomGMapSettings();

            // 3. 初始化 GIS 相關服務，並把地圖控制項傳進去
            _dataService = new MapDataService();
            _disasterManager = new DisasterOverlayManager(gMapControl1);
            _measurementService = new MapMeasurementService(gMapControl1);

            // 4. 綁定地圖滑鼠事件（解決左鍵自由拖曳與點擊測距的衝突）
            gMapControl1.MouseDown += GMapControl1_MouseDown;
            gMapControl1.MouseUp += GMapControl1_MouseUp;
        }

        /// <summary>
        /// 純代碼配置排版：建立側邊欄與地圖容器，並把設計師拉好的元件重新歸位
        /// </summary>
        private void InitializeDynamicPanels()
        {
            // 暫停表單佈局更新，防止重繪時閃爍
            this.SuspendLayout();

            // === 建立左側工具列容器 ===
            panelSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(45, 45, 48), // 質感深灰色
                Padding = new Padding(15)
            };

            // === 建立右側地圖滿版容器 ===
            panelMap = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // === 重新分配元件的「爸爸（Parent）」是誰 ===
            // 將設計師拉好的控制項，從 Form1 移籍到 panelSidebar 中
            panelSidebar.Controls.Add(chkBufferMode);
            panelSidebar.Controls.Add(btnClearMeasure);
            panelSidebar.Controls.Add(chkMeasureMode);
            panelSidebar.Controls.Add(btnToggleDisaster);

            // 將設計師拉好的地圖，從 Form1 移籍到 panelMap 中
            panelMap.Controls.Add(gMapControl1);

            // === 將新容器加回視窗主體 ===
            // 注意：在 WinForms 中，後加入的 Dock.Fill 會填滿「剩餘」空間，順序不能錯
            this.Controls.Add(panelMap);
            this.Controls.Add(panelSidebar);

            // === 調整 panelSidebar 內元件的排版與外觀 ===
            // 災害點開關（讓它有寬度並整齊排列）
            btnToggleDisaster.Dock = DockStyle.Top;
            btnToggleDisaster.Height = 45;
            btnToggleDisaster.Font = new Font("Microsoft JhengHei", 10, FontStyle.Bold);

            // 測距模式 CheckBox（加間距與調整文字顏色）
            chkMeasureMode.Dock = DockStyle.Top;
            chkMeasureMode.Height = 40;
            chkMeasureMode.ForeColor = Color.White;
            chkMeasureMode.Font = new Font("Microsoft JhengHei", 10);
            chkMeasureMode.Padding = new Padding(0, 10, 0, 0);

            // 清除測距按鈕（讓它有寬度並整齊排列）
            btnClearMeasure.Dock = DockStyle.Top;
            btnClearMeasure.Height = 30;
            btnClearMeasure.Font = new Font("Microsoft JhengHei", 10, FontStyle.Bold);

            // 緩衝區模式 CheckBox
            chkBufferMode.Dock = DockStyle.Top;
            chkBufferMode.Height = 40;
            chkBufferMode.ForeColor = Color.White;
            chkBufferMode.Font = new Font("Microsoft JhengHei", 10);
            chkBufferMode.Padding = new Padding(0, 10, 0, 0);

            // === 調整地圖控制項屬性，使其在 panelMap 裡滿版 ===
            gMapControl1.Dock = DockStyle.Fill;

            // 恢復表單佈局
            this.ResumeLayout(false);
        }

        /// <summary>
        /// 地圖底層初始化設定
        /// </summary>
        private void InitializeCustomGMapSettings()
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
                gMapControl1.ShowCenter = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"地圖引擎初始化失敗: {ex.Message}");
            }
        }

        #region 左鍵拖曳與點擊
        private void GMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isMouseDown = true;
                _mouseDownPoint = e.Location; // 記錄點下時的螢幕座標
            }
        }

        private void GMapControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _isMouseDown)
            {
                _isMouseDown = false;

                // 計算滑鼠點下到放開的物理位移距離
                int deltaX = Math.Abs(e.X - _mouseDownPoint.X);
                int deltaY = Math.Abs(e.Y - _mouseDownPoint.Y);

                // 如果位移非常小，代表使用者是想「點擊設點」，而不是「拖曳地圖」
                if (deltaX <= ClickThreshold && deltaY <= ClickThreshold)
                {
                    HandleMapLeftClick(e);
                }
            }
        }

        /// <summary>
        /// 當地圖被點擊時，直接外包給 Service 處理
        /// </summary>
        private void HandleMapLeftClick(MouseEventArgs e)
        {
            // 將畫面上的像素座標 (X, Y) 轉換為地理經緯度 (Lat, Lng)
            PointLatLng clickedPoint = gMapControl1.FromLocalToLatLng(e.X, e.Y);

            // 情況 A：如果「測距模式」有開啟
            if (chkMeasureMode != null && chkMeasureMode.Checked)
            {
                string resultMessage = _measurementService.HandleMapClick(clickedPoint);
            }

            // 情況 B：如果「緩衝區模式」有開啟 (不論測距有無開啟，皆可單獨運作)
            if (chkBufferMode != null && chkBufferMode.Checked)
            {
                // 以 clickedPoint 為中心，建立一個災害緩衝圈
                double bufferRadiusInMeters = 500; // 緩衝區半徑
                _measurementService.CreateBuffer(clickedPoint, bufferRadiusInMeters);
            }
        }
        #endregion 左鍵拖曳與點擊

        #region UI 控制項事件連動
        private void chkBufferMode_CheckedChanged(object sender, EventArgs e)
        {
            // 切換圖層可見度
            _measurementService.SetBufferVisibility(chkBufferMode.Checked);

            // 刷新地圖確保即時渲染
            gMapControl1.Refresh();
        }

        /// <summary>
        /// 顯示/隱藏 災害點大頭針 Toggle Button 事件
        /// </summary>
        private async void btnToggleDisaster_CheckedChanged(object sender, EventArgs e)
        {
            // 狀態取反
            if (btnToggleDisaster.Checked)
            {
                // 變更 UI 按鈕文字提示
                btnToggleDisaster.Text = "讀取中";

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
                    btnToggleDisaster.Text = "隱藏災害觀測點";
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

        #endregion UI 控制項事件連動
    }
}
