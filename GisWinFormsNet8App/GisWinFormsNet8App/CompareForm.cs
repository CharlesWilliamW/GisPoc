using GisWinFormsNet8App.Models;

namespace GisWinFormsNet8App
{
    internal class CompareForm : Form
    {
        public CompareForm(GeoCoordinate a, GeoCoordinate b)
        {
            Text = "設施比對";
            Size = new Size(620, 340);
            MinimumSize = new Size(440, 280);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Microsoft JhengHei", 9);
            BackColor = Color.White;

            var grid = BuildGrid(a, b);

            var btnClose = new Button
            {
                Text = "關閉",
                Width = 80,
                Height = 28,
                Anchor = AnchorStyles.Right,
                Font = new Font("Microsoft JhengHei", 9),
                FlatStyle = FlatStyle.Flat
            };
            btnClose.Click += (s, e) => Close();

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 44 };
            bottomPanel.Controls.Add(btnClose);
            btnClose.Location = new Point(bottomPanel.ClientSize.Width - btnClose.Width - 12, 8);
            btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            Controls.Add(grid);
            Controls.Add(bottomPanel);
        }

        private static DataGridView BuildGrid(GeoCoordinate a, GeoCoordinate b)
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(220, 220, 225),
                BackgroundColor = Color.White,
                EnableHeadersVisualStyles = false
            };

            grid.DefaultCellStyle.Font = new Font("Microsoft JhengHei", 9);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;

            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft JhengHei", 9, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 112, 204);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 112, 204);
            grid.ColumnHeadersHeight = 32;

            grid.Columns.Add("field", "欄位");
            grid.Columns.Add("a", a.Name);
            grid.Columns.Add("b", b.Name);
            grid.Columns["field"].FillWeight = 75;
            grid.Columns["a"].FillWeight = 112;
            grid.Columns["b"].FillWeight = 112;

            var rows = new (string label, string av, string bv)[]
            {
                ("設施名稱",           a.Name,                        b.Name),
                ("設施 ID",            a.CoordinateId,                b.CoordinateId),
                ("TWD97 東座標 X (m)", $"{a.EastX:N3}",               $"{b.EastX:N3}"),
                ("TWD97 北座標 Y (m)", $"{a.NorthY:N3}",              $"{b.NorthY:N3}"),
                ("海拔高度 (m)",       $"{a.BaseElevation:N1}",       $"{b.BaseElevation:N1}")
            };

            foreach (var (label, av, bv) in rows)
            {
                int idx = grid.Rows.Add(label, av, bv);
                var row = grid.Rows[idx];

                bool different = av != bv;
                row.DefaultCellStyle.BackColor = different
                    ? Color.FromArgb(255, 245, 220)
                    : (idx % 2 == 0 ? Color.White : Color.FromArgb(246, 248, 252));

                // 欄位 label: always bold + dark grey
                row.Cells["field"].Style.Font = new Font("Microsoft JhengHei", 9, FontStyle.Bold);
                row.Cells["field"].Style.ForeColor = Color.FromArgb(70, 70, 70);
                row.Cells["field"].Style.BackColor = Color.FromArgb(240, 242, 246);

                if (different)
                {
                    row.Cells["a"].Style.ForeColor = Color.FromArgb(176, 60, 0);
                    row.Cells["b"].Style.ForeColor = Color.FromArgb(176, 60, 0);
                    row.Cells["a"].Style.Font = new Font("Microsoft JhengHei", 9, FontStyle.Bold);
                    row.Cells["b"].Style.Font = new Font("Microsoft JhengHei", 9, FontStyle.Bold);
                }
            }

            grid.RowTemplate.Height = 28;
            return grid;
        }
    }
}
