using STS2RitsuLib.Settings;
using STS2RitsuLib.Telemetry;

namespace SnakeInSpireExtend.Scripts.Extension;

public static class SnakeInSpireTelemetry
{
    public static void Register()
    {
        TelemetryRegistry.RegisterApplicant(new()
        {
            ApplicantId = Entry.ModId,
            OwnerModId = Entry.ModId,
            DisplayName = "Snake",
            DisplayNameText = ModSettingsText.LocString("static_hover_tips", "SNAKE_IN_SPIRE_TELEMETRY.displayNameText", "Snake"),
            Adapter = new PostHogTelemetryAdapter(
                host: "https://us.i.posthog.com",
                projectApiKey: "phc_vd6NhgRVKUnuwDhiv7oDNYCq8hpWtefPejGDnqV8oQua"),//不要做坏事哦
            Requests =
            [
                TelemetryRequest.BasicUsage(
                    ModSettingsText.LocString("static_hover_tips", "SNAKE_IN_SPIRE_TELEMETRY.requestBasicUsage",
                    "Session start time, framework and game versions, build channel, platform, language, and anonymous install ID.")),
                TelemetryRequest.RunHistoryFiltered(
                    ModSettingsText.LocString("static_hover_tips", "SNAKE_IN_SPIRE_TELEMETRY.requestRunHistoryFiltered",
                    "Completed run history data with Snake character."),
                    captureFilter: context => context.SourceData is STS2RitsuLib.RunEndedEvent evt
                        && !evt.IsAbandoned
                        && evt.Run.Players.Any(p => p.CharacterId?.Entry == "SNAKE_IN_SPIRE_EXTEND_CHARACTER_SNAKE")),
                TelemetryRequest.Diagnostics(
                    ModSettingsText.LocString("static_hover_tips", "SNAKE_IN_SPIRE_TELEMETRY.requestDiagnostics",
                    "Exception reports and framework runtime diagnostics.")),
            ],
        });
    }
}