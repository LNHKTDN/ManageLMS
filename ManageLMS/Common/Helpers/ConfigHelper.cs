using System;
using System.Configuration;

public static class ConfigHelper
{
    public static string SqlConnectionString
    {
        get { return ConfigurationManager.ConnectionStrings["DUE_DashBoardConnection"].ConnectionString; }
    }

    // Lấy URL Moodle (Lưu ý: Key trong XML là Moodle_Domain)
    public static string MoodleDomain
    {
        get { return ConfigurationManager.AppSettings["Moodle_Domain"]; }
    }

    // Lấy Token Moodle
    public static string MoodleToken
    {
        get { return ConfigurationManager.AppSettings["Moodle_Token"]; }
    }

}