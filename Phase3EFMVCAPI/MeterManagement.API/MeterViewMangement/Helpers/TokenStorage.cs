namespace MeterViewMangement.Helpers
{
    public static class TokenStorage
    {
        private const string Key = "JWT_TOKEN";

        public static void Save(HttpContext context, string token)
        {
            context.Session.SetString(Key, token);
        }

        public static string? Get(HttpContext context)
        {
            return context.Session.GetString(Key);
        }

        public static void Clear(HttpContext context)
        {
            context.Session.Remove(Key);
        }
    }
}

