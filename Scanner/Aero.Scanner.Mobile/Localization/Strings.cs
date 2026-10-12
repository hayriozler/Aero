using System.Globalization;

namespace Aero.Scanner.Mobile.Localization;

public static class Strings
{
    private static string _lang = "en";

    public static event Action? LanguageChanged;

    public static void SetLanguage(string languageCode)
    {
        _lang = languageCode.ToLowerInvariant() switch
        {
            "tr" => "tr",
            _ => "en"
        };
        CultureInfo.CurrentUICulture = new CultureInfo(_lang);
        LanguageChanged?.Invoke();
    }

    private static string T(string en, string tr) => _lang == "tr" ? tr : en;

    // App
    public static string AppTitle => T("AeroScan HHT", "AeroScan HHT");
    public static string ExitButton => T("EXIT", "ÇIKIŞ");
    public static string ExitPasswordTitle => T("Enter Exit Password", "Çıkış Şifresi Girin");
    public static string ExitPasswordHint => T("Password", "Şifre");
    public static string ExitConfirm => T("Confirm", "Onayla");
    public static string ExitCancel => T("Cancel", "İptal");
    public static string ExitPasswordWrong => T("Incorrect password.", "Hatalı şifre.");
    public static string ConfigErrorTitle => T("Configuration Error", "Yapılandırma Hatası");
    public static string Ok => T("OK", "Tamam");

    // Menu items
    public static string MenuArrivalFlights => T("Arrival Flights", "Varış Uçuşları");
    public static string MenuDepartureFlights => T("Departure Flights", "Gidiş Uçuşları");
    public static string MenuVehicleList => T("Vehicle List", "Araç Liste");
    public static string MenuAddVehicle => T("Add Vehicle", "Araç Ekle");
    public static string MenuStatus => T("Status", "Durum");
    public static string MenuBagActivation => T("Bag Activation Query", "Bagaj Etk. Sorgu");
    public static string MenuDownloadBaggage => T("Download Baggage", "Bagaj İndir");
    public static string MenuDepartureStatus => T("Departure Status", "Gidiş Durum");
    public static string MenuForceLoad => T("Force Load", "Zorla Yükle");
    public static string MenuPassengerQuery => T("Passenger Query", "Yolcu Sorgu");
    public static string MenuBaggageToUnload => T("Baggage to Unload", "İndirilecek Bagaj");
    public static string MenuDeviceQuery => T("Device Query", "Cihaz Sorgu");
    public static string MenuConnectionBaggage => T("Connection Baggage", "Connection Bagaj");
    public static string MenuFlightList => T("Flight List", "Uçuş Listesi");
    public static string MenuArrFlightList => T("ARR Flight List", "ARR Uçuş Listesi");

    // Stub page
    public static string PageNotImplemented => T("This screen is under development.", "Bu ekran geliştirme aşamasındadır.");
    public static string BackButton => T("Back", "Geri");
}
