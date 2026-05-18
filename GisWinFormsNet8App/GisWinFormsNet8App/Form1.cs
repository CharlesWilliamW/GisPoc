using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
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
        // 宣告 GIS 地圖特有的圖層元件 (Overlay)
        private GMapOverlay _markerOverlay;
        private readonly HttpClient _httpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
            InitializeGisMap();
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

                // 在 GIS 系統中，所有點、線、面都必須放在「圖層 (Overlay)」上，再把圖層加進地圖
                _markerOverlay = new GMapOverlay("disaster_layer");
                gMapControl1.Overlays.Add(_markerOverlay);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"地圖引擎初始化失敗: {ex.Message}");
            }
        }

        // 2. 業主的核心功能：按下按鈕後判斷勾選狀態、呼叫 API、地圖呈現
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            // 檢查 UI 元件是否有被勾選
            if (!chkShowData.Checked)
            {
                _markerOverlay.Markers.Clear(); // 清空圖層上的點
                gMapControl1.Refresh();
                MessageBox.Show("請先勾選「顯示災害觀測點」UI 元件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnRefresh.Enabled = false; // 停用按鈕，防止使用者在連線時重複狂點
            btnRefresh.Text = "讀取中...";
            _markerOverlay.Markers.Clear(); // 清除舊的點

            try
            {
                // 3. 模擬呼叫政府 API 獲取地理資料 (未來串接時只需替換此網址)
                string apiUrl = "https://api.mock_government.gov.tw/v1/disaster";
                string jsonResult = await FetchMapDataAsync(apiUrl);

                // 4. 解析符合政府規格的 JSON 資料結構
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                List<DisasterPoint> points = JsonSerializer.Deserialize<List<DisasterPoint>>(jsonResult, options);

                // 5. 將解析出的經緯度逐一在螢幕上打點
                foreach (var pt in points)
                {
                    // 建立地理座標點 (緯度 Latitude, 經度 Longitude)
                    PointLatLng position = new PointLatLng(pt.Latitude, pt.Longitude);

                    // 呼叫 GMap 內建最穩定的紅色大頭針標記 (美工部分不用管，使用預設值)
                    GMarkerGoogle marker = new GMarkerGoogle(position, GMarkerGoogleType.red_pushpin)
                    {
                        // 實作提示功能：滑鼠移到點上方時會顯示這行文字
                        ToolTipText = $"{pt.Title}\n目前狀態: {pt.Status}"
                    };

                    // 將大頭針放進圖層中
                    _markerOverlay.Markers.Add(marker);
                }

                // 資料載入後，自動將地圖視野中心移到第一個取得的資料點上
                if (points != null && points.Count > 0)
                {
                    gMapControl1.Position = new PointLatLng(points[0].Latitude, points[0].Longitude);
                }

                gMapControl1.Refresh(); // 強制地圖重新渲染畫面
            }
            catch (Exception ex)
            {
                MessageBox.Show($"資料對接失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefresh.Enabled = true;
                btnRefresh.Text = "重新整理數據";
            }
        }

        private async Task<string> FetchMapDataAsync(string url)
        {
            try
            {
                // 【未來對接解鎖碼】：等業主把正式 API Key 申請下來後，把下面兩行解開即可直接通訊
                // HttpResponseMessage response = await _httpClient.GetAsync(url);
                // return await response.Content.ReadAsStringAsync();

                // 【PoC 原型階段 Mock】：目前業主尚未提供 API 規格，我們先用標準格式假裝拿到資料
                await Task.Delay(800); // 模擬網路傳輸的延遲感 (0.8秒)
                return @"
                [
                    {""Title"":""大安區積水觀測站 A"", ""Latitude"":25.0339, ""Longitude"":121.5434, ""Status"":""注意""},
                    {""Title"":""信義區水位監測點 B"", ""Latitude"":25.0375, ""Longitude"":121.5662, ""Status"":""正常""},
                    {""Title"":""松山區淹水警戒區 C"", ""Latitude"":25.0524, ""Longitude"":121.5540, ""Status"":""危險""}
                ]";
            }
            catch
            {
                throw new Exception("外部圖資伺服器連線逾時");
            }
        }
    }

    // 定義與 API 欄位對接的資料實體類別
    public class DisasterPoint
    {
        public string Title { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; }
    }
}
