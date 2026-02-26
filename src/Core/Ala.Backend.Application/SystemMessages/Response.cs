
namespace Ala.Backend.Application.SystemMessages
{
    public static class Response
    {
        public static class Common
        {
            public const string Error = "Common_Error"; // Beklenmedik bir hata oluştu.
            public const string ValidationFailed = "Common_ValidationFailed"; // Girdiğiniz verilerde doğrulama hatası var.
            public const string Unauthorized = "Common_Unauthorized"; // Bu işlem için oturum açmanız gerekiyor.
            public const string Forbidden = "Common_Forbidden"; // Bu işlemi yapmaya yetkiniz yok.
            public const string NotFound = "Common_NotFound"; // İstenen kaynak bulunamadı.
            public const string OperationSuccess = "Common_OperationSuccess"; // İşlem başarıyla tamamlandı.
            public const string OperationFailed = "Common_OperationFailed"; // İşlem gerçekleştirilemedi.
            public const string MaintenanceMode = "Common_MaintenanceMode"; // Sistem şu anda bakım modunda.
            public const string TooManyRequests = "Common_TooManyRequests"; // Çok fazla istek gönderdiniz, lütfen bekleyin.
        }
    }
}
