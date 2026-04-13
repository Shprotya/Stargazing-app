using System.Diagnostics;

namespace StargazingApp.Services;

public class LocationService
{
    public async Task<(double Lat, double Lon)?> GetLocationAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted) return null;

            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Low,
                Timeout = TimeSpan.FromSeconds(10)
            });

            return location != null ? (location.Latitude, location.Longitude) : null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Location error: {ex.Message}");
            return null;
        }
    }
}