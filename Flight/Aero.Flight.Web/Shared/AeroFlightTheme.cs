using MudBlazor;

namespace Aero.Flights.Web.Shared;

public static class AeroFlightTheme
{
    private const string _bodyFontStack = "\"Barlow\", system-ui, sans-serif";
    private const string _headingFontStack = "\"Barlow Condensed\", system-ui, sans-serif";

    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#b57a2c",
            Secondary = "#bd8f45",
            Success = "#10b981",
            Warning = "#f59e0b",
            Error = "#b3321f",
            Info = "#64748b",
            Background = "#f8f4ea",
            Surface = "#f1e9d9",
            AppbarBackground = "#eaddc4",
            DrawerBackground = "#f1e9d9",
            TextPrimary = "#241d13",
            TextSecondary = "#241d13b3",
            LinesDefault = "#d9d4cd",
            LinesInputs = "#d9d4cd",
            TableLines = "#d9d4cd",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = [_bodyFontStack] },
            Body1 = new Body1Typography { FontFamily = [_bodyFontStack] },
            Body2 = new Body2Typography { FontFamily = [_bodyFontStack] },
            H1 = new H1Typography { FontFamily = [_headingFontStack], FontWeight = "600" },
            H2 = new H2Typography { FontFamily = [_headingFontStack], FontWeight = "600" },
            H3 = new H3Typography { FontFamily = [_headingFontStack], FontWeight = "600" },
            H4 = new H4Typography { FontFamily = [_headingFontStack], FontWeight = "600" },
            H5 = new H5Typography { FontFamily = [_headingFontStack], FontWeight = "600" },
            H6 = new H6Typography { FontFamily = [_headingFontStack], FontWeight = "600" },
            Subtitle1 = new Subtitle1Typography { FontFamily = [_headingFontStack], FontWeight = "600" },
            Subtitle2 = new Subtitle2Typography { FontFamily = [_headingFontStack], FontWeight = "600" },
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "0px",
        },
    };
}
