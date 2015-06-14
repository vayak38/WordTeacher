﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using WordTeacher.Commands;
using WordTeacher.Models;
using WordTeacher.Utilities;
using WordTeacher.Views;

namespace WordTeacher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, ITranslationsLoadable
    {
        private double _positionX;
        private double _positionY;
        private bool _isSettingsOpened;
        private ICommand _closeCommand;
        private ICommand _settingsCommand;
        private ObservableCollection<TranslationItem> _translationItems = new ObservableCollection<TranslationItem>(); 

        public MainViewModel()
        {
            ArrangeWindowPosition();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Position of the window on the X axis.
        /// </summary>
        public double PositionX
        {
            get { return _positionX; }
            set
            {
                _positionX = value;
                OnPropertyChanged("PositionX");
            }
        }

        /// <summary>
        /// Position of the window on the Y axis.
        /// </summary>
        public double PositionY
        {
            get { return _positionY; }
            set
            {
                _positionY = value;
                OnPropertyChanged("PositionY");
            }
        }

        /// <summary>
        /// Position of the window on the Y axis.
        /// </summary>
        public bool IsSettingsOpened
        {
            get { return _isSettingsOpened; }
            set
            {
                _isSettingsOpened = value;
                OnPropertyChanged("IsSettingsOpened");
            }
        }

        /// <summary>
        /// The list of words and their translations.
        /// </summary>
        public ObservableCollection<TranslationItem> TranslationItems
        {
            get { return _translationItems; }
            set
            {
                _translationItems = value;
                OnPropertyChanged("TranslationItems");
            }
        }

        public ICommand SettingsCommand
        {
            get
            {
                return _settingsCommand ?? (_settingsCommand = new CommandHandler(OpenSettings, true));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new CommandHandler(CloseApplication, true));
            }
        }

        public void ReloadSettings(IList<TranslationItem> translationItems)
        {
            TranslationItems = new ObservableCollection<TranslationItem>(translationItems);
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Assign position to the window.
        /// </summary>
        private void ArrangeWindowPosition()
        {
            var topCenterPoint = ScreenUtility.GetTopCenterPoint();
            PositionX = topCenterPoint.X;
            PositionY = topCenterPoint.Y;
        }

        private void OpenSettings()
        {
            var settingsView = new SettingsView();

            settingsView.Closed += SettingsViewOnClosed;
            settingsView.Show();

            IsSettingsOpened = true;
        }

        private void SettingsViewOnClosed(object sender, EventArgs eventArgs)
        {
            var settingsView = sender as SettingsView;
            if (settingsView != null)
            {
                // Copy items from settings.
                var settingsViewModel = (SettingsViewModel)settingsView.DataContext;
                TranslationItems.Clear();
                foreach (var translationItem in settingsViewModel.SavedTranslationItems)
                {
                    TranslationItems.Add(new TranslationItem(translationItem.Word, translationItem.Translation));                    
                }
            }

            IsSettingsOpened = false;
        }

        private static void CloseApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
