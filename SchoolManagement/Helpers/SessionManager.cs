namespace SchoolManagement.Helpers;

public static class SessionManager
{
    public static string CurrentTenantId { get; set; }
    public static string CurrentUserId { get; set; }
    public static string CurrentUserRole { get; set; }
    public static string CurrentUserName { get; set; }
    public static string AuthToken { get; set; }

    public static bool IsAuthenticated => !string.IsNullOrEmpty(CurrentUserId);

    public static void ClearSession()
    {
        CurrentTenantId = null;
        CurrentUserId = null;
        CurrentUserRole = null;
        CurrentUserName = null;
        AuthToken = null;
    }
}
