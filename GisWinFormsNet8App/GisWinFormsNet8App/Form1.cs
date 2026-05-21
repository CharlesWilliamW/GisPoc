using GMap.NET;
using GMap.NET.MapProviders;

namespace GisWinFormsNet8App
{
    public partial class Form1 : Form
    {
        private readonly MapDataService _dataService;
        private readonly DisasterOverlayManager _disasterManager;
        private readonly MapMeasurementService _measurementService;
        private readonly SidebarController _sidebarController;

        // 解決滑鼠左鍵「拖曳地圖」與「點擊設點」衝突的關鍵變數
        private bool _isMouseDown = false;
        private Point _mouseDownPoint;
        private const int ClickThreshold = 3;

        public Form1()
        {
            InitializeComponent();

            _sidebarController = new SidebarController();
            _sidebarController.Initialize(this, gMapControl1, chkBufferMode, btnClearMeasure, chkMeasureMode, btnToggleDisaster);

            InitializeCustomGMapSettings();

            _dataService = new MapDataService();
            _disasterManager = new DisasterOverlayManager(gMapControl1);
            _measurementService = new MapMeasurementService(gMapControl1);

            gMapControl1.MouseDown += GMapControl1_MouseDown;
            gMapControl1.MouseUp += GMapControl1_MouseUp;
        }

        private void InitializeCustomGMapSettings()
        {
            try
            {
                gMapControl1.MapProvider = GMapProviders.WikiMapiaMap;
                gMapControl1.Position = new PointLatLng(25.0330, 121.5654);
                gMapControl1.MinZoom = 5;
                gMapControl1.MaxZoom = 18;
                gMapControl1.Zoom = 13;
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
                _mouseDownPoint = e.Location;
            }
        }

        private void GMapControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _isMouseDown)
            {
                _isMouseDown = false;

                int deltaX = Math.Abs(e.X - _mouseDownPoint.X);
                int deltaY = Math.Abs(e.Y - _mouseDownPoint.Y);

                if (deltaX <= ClickThreshold && deltaY <= ClickThreshold)
                    HandleMapLeftClick(e);
            }
        }

        private void HandleMapLeftClick(MouseEventArgs e)
        {
            PointLatLng clickedPoint = gMapControl1.FromLocalToLatLng(e.X, e.Y);

            if (chkMeasureMode.Checked)
                _measurementService.HandleMapClick(clickedPoint);

            if (chkBufferMode.Checked)
                _measurementService.CreateBuffer(clickedPoint, 500);
        }
        #endregion

        #region UI 控制項事件連動
        private void chkBufferMode_CheckedChanged(object sender, EventArgs e)
        {
            _measurementService.SetBufferVisibility(chkBufferMode.Checked);
            gMapControl1.Refresh();
        }

        private async void btnToggleDisaster_CheckedChanged(object sender, EventArgs e)
        {
            if (btnToggleDisaster.Checked)
            {
                btnToggleDisaster.Text = "讀取中";
                btnToggleDisaster.Enabled = false;

                try
                {
                    var data = await _dataService.FetchDisasterPoints();
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
                _disasterManager.ToggleVisibility(false);
            }
        }

        private void btnClearMeasure_Click(object sender, EventArgs e)
        {
            _measurementService.ClearMeasurement();
        }
        #endregion
    }
}
