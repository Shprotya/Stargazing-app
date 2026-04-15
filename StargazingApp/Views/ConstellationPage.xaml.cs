using System.Collections.Generic;
using System.Linq;
using StargazingApp.Models;

namespace StargazingApp.Views;

public partial class ConstellationPage : ContentPage
{
    private readonly ConstellationViewModel _viewModel;
    private List<Constellation> _allConstellations = new();

    public ConstellationPage(ConstellationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Subscribe to filter changes to update button colors
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Always load the full list first (needed for the client-side cache)
        await _viewModel.LoadAllCommand.ExecuteAsync(null);
        UpdateButtonColors();

        // Cache the FULL list for live typing filter
        _allConstellations = _viewModel.Constellations.ToList();

        // === FIX 1: Pre-filled search from "Tonight's Best" on MainPage ===
        if (!string.IsNullOrWhiteSpace(_viewModel.SearchText))
        {
            var filtered = _allConstellations
                .Where(c => c.Name.ToLower().Contains(_viewModel.SearchText.ToLower()))
                .OrderBy(c => c.Name);

            _viewModel.UpdateList(filtered);
        }
    }

    private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Update button colors when active filter changes
        if (e.PropertyName == nameof(ConstellationViewModel.ActiveFilter))
        {
            UpdateButtonColors();
        }
    }

    private void UpdateButtonColors()
    {
        var activeColor = (Color)Application.Current.Resources["NovaPurple"];
        var inactiveColor = (Color)Application.Current.Resources["Button"];

        BtnAll.BackgroundColor = _viewModel.ActiveFilter == "All" ? activeColor : inactiveColor;
        BtnNorth.BackgroundColor = _viewModel.ActiveFilter == "Northern" ? activeColor : inactiveColor;
        BtnSouth.BackgroundColor = _viewModel.ActiveFilter == "Southern" ? activeColor : inactiveColor;
        BtnFav.BackgroundColor = _viewModel.ActiveFilter == "Favorites" ? activeColor : inactiveColor;
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue))
        {
            _viewModel.UpdateList(_allConstellations);
            return;
        }

        var filtered = _allConstellations
            .Where(c => c.Name.ToLower().Contains(e.NewTextValue.ToLower()))
            .OrderBy(c => c.Name);

        _viewModel.UpdateList(filtered);
    }

}