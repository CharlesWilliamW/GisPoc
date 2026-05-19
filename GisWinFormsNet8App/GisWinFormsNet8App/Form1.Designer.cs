namespace GisWinFormsNet8App
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            chkShowData = new CheckBox();
            btnRefresh = new Button();
            chkMeasureMode = new CheckBox();
            btnClearMeasure = new Button();
            SuspendLayout();
            // 
            // gMapControl1
            // 
            gMapControl1.Bearing = 0F;
            gMapControl1.CanDragMap = true;
            gMapControl1.Dock = DockStyle.Fill;
            gMapControl1.EmptyTileColor = Color.Navy;
            gMapControl1.GrayScaleMode = false;
            gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            gMapControl1.LevelsKeepInMemory = 5;
            gMapControl1.Location = new Point(0, 0);
            gMapControl1.Margin = new Padding(2);
            gMapControl1.MarkersEnabled = true;
            gMapControl1.MaxZoom = 2;
            gMapControl1.MinZoom = 2;
            gMapControl1.MouseWheelZoomEnabled = true;
            gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            gMapControl1.Name = "gMapControl1";
            gMapControl1.NegativeMode = false;
            gMapControl1.PolygonsEnabled = true;
            gMapControl1.RetryLoadTile = 0;
            gMapControl1.RoutesEnabled = true;
            gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            gMapControl1.SelectedAreaFillColor = Color.FromArgb(33, 65, 105, 225);
            gMapControl1.ShowTileGridLines = false;
            gMapControl1.Size = new Size(509, 293);
            gMapControl1.TabIndex = 0;
            gMapControl1.Zoom = 0D;
            // 
            // chkShowData
            // 
            chkShowData.AutoSize = true;
            chkShowData.Location = new Point(8, 8);
            chkShowData.Margin = new Padding(2);
            chkShowData.Name = "chkShowData";
            chkShowData.Size = new Size(110, 19);
            chkShowData.TabIndex = 1;
            chkShowData.Text = "顯示災害觀測點";
            chkShowData.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(8, 29);
            btnRefresh.Margin = new Padding(2);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(103, 22);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "重新整理數據";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // chkMeasureMode
            // 
            chkMeasureMode.AutoSize = true;
            chkMeasureMode.Location = new Point(8, 70);
            chkMeasureMode.Margin = new Padding(2);
            chkMeasureMode.Name = "chkMeasureMode";
            chkMeasureMode.Size = new Size(157, 19);
            chkMeasureMode.TabIndex = 3;
            chkMeasureMode.Text = "開啟測距模式 (點擊兩點)";
            chkMeasureMode.UseVisualStyleBackColor = true;
            // 
            // btnClearMeasure
            // 
            btnClearMeasure.Location = new Point(8, 93);
            btnClearMeasure.Margin = new Padding(2);
            btnClearMeasure.Name = "btnClearMeasure";
            btnClearMeasure.Size = new Size(103, 22);
            btnClearMeasure.TabIndex = 5;
            btnClearMeasure.Text = "清除距離量測";
            btnClearMeasure.UseVisualStyleBackColor = true;
            btnClearMeasure.Click += btnClearMeasure_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(509, 293);
            Controls.Add(btnClearMeasure);
            Controls.Add(chkMeasureMode);
            Controls.Add(btnRefresh);
            Controls.Add(chkShowData);
            Controls.Add(gMapControl1);
            Margin = new Padding(2);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private CheckBox chkShowData;
        private Button btnRefresh;
        private CheckBox chkMeasureMode;
        private Button btnClearMeasure;
    }
}
