using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JournalApp.Interfaces;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System;
using System.IO;

namespace JournalApp.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly IJournalRepository _journalRepository;
        private readonly PaletteHelper _paletteHelper = new();

        [ObservableProperty]
        private bool _isDarkMode;

        [ObservableProperty]
        private string _databasePath = string.Empty;

        [ObservableProperty]
        private string _newPin = string.Empty;

        public SettingsViewModel(IJournalRepository journalRepository)
        {
            _journalRepository = journalRepository;
            LoadSettings();
        }

        private async void LoadSettings()
        {
            var settings = await _journalRepository.GetUserSettingsAsync();
            IsDarkMode = settings.IsDarkMode;
            
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "JournalApp", "journal.db");
            DatabasePath = dbPath;
        }

        [RelayCommand]
        private async Task ChangePin()
        {
            if (string.IsNullOrWhiteSpace(NewPin) || NewPin.Length < 4)
            {
                MessageBox.Show("PIN must be at least 4 digits.", "Invalid PIN", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var settings = await _journalRepository.GetUserSettingsAsync();
            settings.Pin = NewPin;
            await _journalRepository.UpdateUserSettingsAsync(settings);
            
            NewPin = string.Empty;
            MessageBox.Show("PIN changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private async Task DeleteAccount()
        {
            var result = MessageBox.Show("Are you sure you want to DELETE YOUR ACCOUNT and ALL DATA? This cannot be undone.", 
                                       "DANGER: Delete Account", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            
            if (result == MessageBoxResult.Yes)
            {
                var confirm = MessageBox.Show("Please confirm again. This will permanently erase all journal entries and settings.", 
                                            "FINAL CONFIRMATION", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                
                if (confirm == MessageBoxResult.OK)
                {
                    await _journalRepository.ClearAllDataAsync();
                    
                    // Reset settings for "account deletion" simulation
                    var settings = await _journalRepository.GetUserSettingsAsync();
                    settings.Pin = string.Empty;
                    settings.IsDarkMode = false;
                    await _journalRepository.UpdateUserSettingsAsync(settings);
                    IsDarkMode = false;

                    MessageBox.Show("Account and data deleted successfully. The app will now reset.", "Goodbye", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        [RelayCommand]
        private void Backup()
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"journal_backup_{DateTime.Now:yyyyMMdd}.db",
                    Filter = "Database Files (*.db)|*.db",
                    Title = "Export Database Backup"
                };

                if (dialog.ShowDialog() == true)
                {
                    File.Copy(DatabasePath, dialog.FileName, true);
                    MessageBox.Show("Backup completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        partial void OnIsDarkModeChanged(bool value)
        {
            var theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(value ? BaseTheme.Dark : BaseTheme.Light);
            _paletteHelper.SetTheme(theme);
        }
    }
}
