using System.IO;
using System.Text.Json;
using ModbusMaster.Models;

namespace ModbusMaster.Utilities
{
    public static class JsonSettingsManager
    {
        private static readonly string FilePath = "modbus_settings.json";

        // 설정 저장
        public static void SaveSettings(ConnectModel model)
        {
            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                WriteIndented = true // 보기 좋은 포맷
            });
            File.WriteAllText(FilePath, json);
        }

        // 설정 로드
        public static ConnectModel LoadSettings()
        {
            if (!File.Exists(FilePath))
            {
                return new ConnectModel(); // 파일이 없으면 기본값 반환
            }

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<ConnectModel>(json);
        }
    }
}
