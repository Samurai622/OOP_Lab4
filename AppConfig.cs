using System;

namespace OOP_Lab4
{
    public static class AppConfig
    {
        public static string ApiBaseUrl { get; set; } = "http://localhost:3000/api";
        public static Action OnDdosBlocked { get; set; }
    }
}