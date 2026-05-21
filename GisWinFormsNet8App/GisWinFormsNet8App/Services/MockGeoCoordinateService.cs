using GisWinFormsNet8App.Models;

namespace GisWinFormsNet8App.Services
{
    /// <summary>
    /// PoC 階段 Mock 實作。
    /// 未來換成真實 API 時，新增 ApiGeoCoordinateService : IGeoCoordinateService 取代此類別即可。
    /// </summary>
    public class MockGeoCoordinateService : IGeoCoordinateService
    {
        public Task<List<GeoCoordinate>> GetCoordinatesAsync()
        {
            var data = new List<GeoCoordinate>
            {
                new GeoCoordinate
                {
                    CoordinateId = "SOL-001",
                    Name        = "太陽能設施 A",
                    EastX       = 302158.0,
                    NorthY      = 2768486.0,
                    BaseElevation = 45.0
                },
                new GeoCoordinate
                {
                    CoordinateId = "WIN-001",
                    Name        = "風能設施 B",
                    EastX       = 214880.0,
                    NorthY      = 2672540.0,
                    BaseElevation = 320.0
                },
                new GeoCoordinate
                {
                    CoordinateId = "ADM-001",
                    Name        = "行政區 C",
                    EastX       = 258320.0,
                    NorthY      = 2720100.0,
                    BaseElevation = 12.0
                }
            };

            return Task.FromResult(data);
        }
    }
}
