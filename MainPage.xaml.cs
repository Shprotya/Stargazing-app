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
            // apodData is 'dynamic'
            var apodData = await _nasaApiService.GetApodAsync();

            if (apodData != null)
            {
                TitleLabel.Text = apodData.title;

                DateLabel.Text = $"Date: {apodData.date}";

                // Checking media_type
                if (apodData.media_type == "image")
                {
                    ApodImage.Source = ImageSource.FromUri(new Uri((string)apodData.url));
                    ImageFrame.IsVisible = true;
                }

                // Checking copyright
                if (apodData.copyright != null)
                {
                    CopyrightLabel.Text = $"© {apodData.copyright}";
                    CopyrightLabel.IsVisible = true;
                }

                ExplanationLabel.Text = apodData.explanation;
                ExplanationFrame.IsVisible = true;
            }
            else
            {
                ErrorLabel.Text = "Failed to load data.";
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = $"Error: {ex.Message}";
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            LoadApodButton.IsEnabled = true;
        }
    }
}