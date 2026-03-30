using CommunityToolkit.Mvvm.ComponentModel;
using StargazingApp.Models;
using System.Collections.ObjectModel;


namespace StargazingApp.ViewModels;
public partial class JournalViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    //Observable state
    [ObservableProperty] private ObservableCollection<JournalEntry> entries = new();
    [ObservableProperty] private JournalEntry? selectedEntry;
    [ObservableProperty] private bool isEditing;
    [ObservableProperty] private bool hasEntries;

    // Editor fields (bound when adding/editing)
    [ObservableProperty] private string editTitle = string.Empty;
    [ObservableProperty] private string editBody = string.Empty;
    [ObservableProperty] private string editConditions = string.Empty;
    [ObservableProperty] private int editRating = 3;

    // Search / filter
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string activeFilter = "All";

    // Date filter working-with-dates support
    [ObservableProperty] private DateTime filterFrom = DateTime.Now.AddMonths(-1);
    [ObservableProperty] private DateTime filterTo = DateTime.Now;
    [ObservableProperty] private bool isDateFilterActive;
}

