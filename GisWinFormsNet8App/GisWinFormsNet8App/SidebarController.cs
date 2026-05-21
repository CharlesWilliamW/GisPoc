using GMap.NET.WindowsForms;

namespace GisWinFormsNet8App
{
    internal class SidebarController
    {
        private SplitContainer? _splitContainer;
        private readonly int _sidebarExpandedWidth = 250;

        public void Initialize(Form form, GMapControl mapControl,
            CheckBox chkBufferMode, Button btnClearMeasure,
            CheckBox chkMeasureMode, CheckBox btnToggleDisaster)
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

            _splitContainer.Panel1.Controls.Add(chkBufferMode);
            _splitContainer.Panel1.Controls.Add(btnClearMeasure);
            _splitContainer.Panel1.Controls.Add(chkMeasureMode);
            _splitContainer.Panel1.Controls.Add(btnToggleDisaster);
            _splitContainer.Panel2.Controls.Add(mapControl);

            form.Controls.Add(_splitContainer);

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

            mapControl.Dock = DockStyle.Fill;

            form.ResumeLayout(false);
        }
    }
}
