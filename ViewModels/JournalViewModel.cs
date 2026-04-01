using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public JournalViewModel(DatabaseService db)
    {
        _db = db;
    }

    // Load
    [RelayCommand]
    public async Task LoadAllAsync()
    {
        ActiveFilter = "All";
        IsDateFilterActive = false;

        try
        {
            var items = await _db.GetJournalEntriesAsync();
            UpdateList(items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Journal load error: {ex.Message}");
        }
    }

    //Search
    [RelayCommand]
    public async Task SearchAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            await LoadAllAsync();
            return;
        }

        try
        {
            var items = await _db.SearchJournalAsync(text);
            UpdateList(items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Journal search error: {ex.Message}");
        }
    }

    //Date filter
    [RelayCommand]
    public async Task FilterByDateRangeAsync()
    {
        ActiveFilter = "DateRange";
        IsDateFilterActive = true;

        try
        {
            var items = await _db.GetJournalEntriesByDateRangeAsync(FilterFrom, FilterTo);
            UpdateList(items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Journal date filter error: {ex.Message}");
        }
    }

    // Filter by this month
    [RelayCommand]
    public async Task FilterThisMonthAsync()
    {
        ActiveFilter = "ThisMonth";
        IsDateFilterActive = false;

        try
        {
            var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var items = await _db.GetJournalEntriesByDateRangeAsync(start, DateTime.Now);
            UpdateList(items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Journal month filter error: {ex.Message}");
        }
    }

    // New entry
    [RelayCommand]
    public void NewEntry()
    {
        SelectedEntry = null;
        EditTitle = string.Empty;
        EditBody = string.Empty;
        EditConditions = string.Empty;
        EditRating = 3;
        IsEditing = true;
    }

    // Edit existing entry
    [RelayCommand]
    public void EditEntry(JournalEntry entry)
    {
        SelectedEntry = entry;
        EditTitle = entry.Title;
        EditBody = entry.Body;
        EditConditions = entry.Conditions;
        EditRating = entry.Rating;
        IsEditing = true;
    }

    // Save
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(EditTitle)) return;

        try
        {
            var entry = SelectedEntry ?? new JournalEntry();
            entry.Title = EditTitle.Trim();
            entry.Body = EditBody.Trim();
            entry.Conditions = EditConditions.Trim();
            entry.Rating = Math.Clamp(EditRating, 1, 5);

            await _db.SaveJournalEntryAsync(entry);

            IsEditing = false;
            await LoadAllAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Journal save error: {ex.Message}");
        }
    }

    // Delete
    [RelayCommand]
    public async Task DeleteAsync(JournalEntry entry)
    {
        try
        {
            bool confirmed = await Shell.Current.DisplayAlert(
                "Delete Entry",
                $"Delete \"{entry.Title}\"?",
                "Delete", "Cancel");

            if (!confirmed) return;

            await _db.DeleteJournalEntryAsync(entry);
            await LoadAllAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Journal delete error: {ex.Message}");
        }
    }

    // Cancel edit
    [RelayCommand]
    public void CancelEdit()
    {
        IsEditing = false;
    }

    // Rating helpers
    [RelayCommand]
    public void SetRating(string rating) => EditRating = int.TryParse(rating, out int r) ? r : 3;

    // Helper
    private void UpdateList(IEnumerable<JournalEntry> items)
    {
        Entries.Clear();
        foreach (var e in items)
            Entries.Add(e);

        HasEntries = Entries.Count > 0;
    }
}

