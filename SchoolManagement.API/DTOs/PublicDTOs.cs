namespace SchoolManagement.API.DTOs;

public record PublicAppInfoResponse(
    string AppName,
    string Tagline,
    string SupportEmail,
    string SupportPhone,
    string SupportWhatsApp,
    string HelpMessage,
    string LoginHint
);
