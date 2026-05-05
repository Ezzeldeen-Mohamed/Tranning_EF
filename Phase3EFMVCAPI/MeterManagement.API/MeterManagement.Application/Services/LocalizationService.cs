using System.Globalization;

namespace MeterManagement.Application.Services
{
    public interface ILocalizationService
    {
        string GetString(string key);
    }

    public class LocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, Dictionary<string, string>> _translations;

        public LocalizationService()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>
            {
                ["en"] = new Dictionary<string, string>
                {
                    ["InvalidEmailOrPassword"] = "Invalid email or password",
                    ["LoginError"] = "An error occurred during login",
                    ["RegistrationError"] = "An error occurred during registration",
                    ["RegistrationFailed"] = "Registration failed",
                    ["RegistrationSuccess"] = "User registered successfully",
                    ["UserAlreadyExists"] = "User with this email already exists",
                    ["UserNotAuthenticated"] = "User not authenticated",
                    ["UserNotFound"] = "User not found",
                    ["GeneralError"] = "An error occurred"
                },
                ["ar"] = new Dictionary<string, string>
                {
                    ["InvalidEmailOrPassword"] = "البريد الإلكتروني أو كلمة المرور غير صحيحة",
                    ["LoginError"] = "حدث خطأ أثناء تسجيل الدخول",
                    ["RegistrationError"] = "حدث خطأ أثناء التسجيل",
                    ["RegistrationFailed"] = "فشل التسجيل",
                    ["RegistrationSuccess"] = "تم تسجيل المستخدم بنجاح",
                    ["UserAlreadyExists"] = "يوجد مستخدم مسجل بهذا البريد مسبقًا",
                    ["UserNotAuthenticated"] = "المستخدم غير مصادق",
                    ["UserNotFound"] = "لم يتم العثور على المستخدم",
                    ["GeneralError"] = "حدث خطأ"
                }
            };
        }

        public string GetString(string key)
        {
            var currentCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            if (_translations.ContainsKey(currentCulture) && _translations[currentCulture].ContainsKey(key))
            {
                return _translations[currentCulture][key];
            }

            if (_translations["ar"].ContainsKey(key))
            {
                return _translations["ar"][key];
            }


            return key;
        }
    }
}
