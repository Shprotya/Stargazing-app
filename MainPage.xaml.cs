namespace StargazingApp;

public partial class MainPage : ContentPage
{
    private readonly NasaApiService _nasaApiService;

    public MainPage(NasaApiService nasaApiService)
    {
        InitializeComponent();
        _nasaApiService = nasaApiService;
    }

    private async void OnLoadApodClicked(object sender, EventArgs e)
    {
        try
        {
            var apodData = await _nasaApiService.GetApodAsync();

            if (apodData != null)
            {
                // Display title
                TitleLabel.Text = apodData.Title;

                // Display date
                DateLabel.Text = $"Date: {apodData.Date}";

                // Display image if media type is image
                if (apodData.MediaType == "image")
                {
                    ApodImage.Source = ImageSource.FromUri(new Uri(apodData.Url));
                    ImageFrame.IsVisible = true;
                }

                // Display copyright if available
                if (!string.IsNullOrEmpty(apodData.Copyright))
                {
                    CopyrightLabel.Text = $"© {apodData.Copyright}";
                    CopyrightLabel.IsVisible = true;
                }

                // Display explanation
                ExplanationLabel.Text = apodData.Explanation;
                ExplanationFrame.IsVisible = true;
            }

            else
            {
                ErrorLabel.Text = "Failed to load data. Please check your internet connection and try again.";
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = $"Error: {ex.Message}";
            ErrorLabel.IsVisible = true;
        }
    }
}