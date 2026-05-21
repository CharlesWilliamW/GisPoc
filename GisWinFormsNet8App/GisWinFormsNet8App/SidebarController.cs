using GMap.NET.WindowsForms;
using GisWinFormsNet8App.Services;

namespace GisWinFormsNet8App
{
    internal class SidebarController
    {
        private SplitContainer? _splitContainer;
        private CheckedListBox? _geoList;
        private readonly int _sidebarExpandedWidth = 250;

        public void Initialize(Form form, GMapControl mapControl,
            CheckBox chkBufferMode, Button btnClearMeasure,
            CheckBox chkMeasureMode, CheckBox btnToggleDisaster,
            IGeoCoordinateService geoService)
        {
            form.SuspendLayout();

            // Shown 事件才套用 SplitterDistance：
            // Load 時 Dock=Fill 尚未執行，Width 仍是預設值，設定會觸發內部驗證 exception
            _splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 5,
                BackColor = Color.FromArgb(60, 60, 63)
            };
            form.Shown += (s, e) =>
            {
                _splitContainer.Panel1MinSize = 30;
                _splitContainer.Panel2MinSize = 200;
                _splitContainer.SplitterDistance = Math.Min(
                    _sidebarExpandedWidth,
                    _splitContainer.Width - _splitContainer.Panel2MinSize - _splitContainer.SplitterWidth
                );
            };

            _splitContainer.Panel1.BackColor = Color.FromArgb(45, 45, 48);
            _splitContainer.Panel1.Padding = new Padding(15, 15, 15, 0);
            _splitContainer.Panel2.BackColor = Color.White;

            _splitContainer.Panel2.Controls.Add(mapControl);
            form.Controls.Add(_splitContainer);

            // --- 底部控制列樣式 ---
            btnToggleDisaster.Dock = DockStyle.Top;
            btnToggleDisaster.Height = 45;
            btnToggleDisaster.Font = new Font("Microsoft JhengHei", 10, FontStyle.Bold);

            chkMeasureMode.Dock = DockStyle.Top;
            chkMeasureMode.Height = 40;
            chkMeasureMode.ForeColor = Color.White;
            chkMeasureMode.Font = new Font("Microsoft JhengHei", 10);
            chkMeasureMode.Padding = new Padding(0, 10, 0, 0);

            btnClearMeasure.Dock = DockStyle.Top;
            btnClearMeasure.Height = 30;
            btnClearMeasure.Font = new Font("Microsoft JhengHei", 10, FontStyle.Bold);

            chkBufferMode.Dock = DockStyle.Top;
            chkBufferMode.Height = 40;
            chkBufferMode.ForeColor = Color.White;
            chkBufferMode.Font = new Font("Microsoft JhengHei", 10);
            chkBufferMode.Padding = new Padding(0, 10, 0, 0);

            var controlPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 165,
                Padding = new Padding(0, 0, 0, 10),
                BackColor = Color.Transparent
            };
            // Dock=Top 時，最後加入的控制項排在最上方，依序往下疊
            controlPanel.Controls.Add(chkBufferMode);
            controlPanel.Controls.Add(btnClearMeasure);
            controlPanel.Controls.Add(chkMeasureMode);
            controlPanel.Controls.Add(btnToggleDisaster);

            // --- 設施清單 CheckList ---
            var lblGeoTitle = new Label
            {
                Text = "設施清單",
                Dock = DockStyle.Top,
                Height = 28,
                ForeColor = Color.FromArgb(180, 180, 180),
                Font = new Font("Microsoft JhengHei", 9, FontStyle.Bold),
                Padding = new Padding(0, 6, 0, 0)
            };

            _geoList = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(37, 37, 38),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Microsoft JhengHei", 9),
                CheckOnClick = true
            };

            // 非同步載入設施清單（在視窗顯示後執行）
            form.Shown += async (s, e) =>
            {
                var items = await geoService.GetCoordinatesAsync();
                foreach (var item in items)
                    _geoList.Items.Add(item);
            };

            // Panel1 加入順序決定 Dock 優先權：
            //   Bottom → Fill → Top（最後加入的 Top 排最上方）
            _splitContainer.Panel1.Controls.Add(controlPanel);  // Dock=Bottom
            _splitContainer.Panel1.Controls.Add(_geoList);      // Dock=Fill（中間填滿）
            _splitContainer.Panel1.Controls.Add(lblGeoTitle);   // Dock=Top（最後加入 = 最上方）

            mapControl.Dock = DockStyle.Fill;

            form.ResumeLayout(false);
        }
    }
}
