﻿using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Mygod.Windows;
using Mygod.Windows.Input;
using Application = System.Windows.Application;

namespace Mygod.WorldOfGoo.Cursor
{
    public sealed partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            refreshTimer = new DispatcherTimer(TimeSpan.FromSeconds(1 / Settings.Current.RefreshRate), DispatcherPriority.Render, 
                                               Refresh, Dispatcher);
            Settings.Current.ShowOriginalCursorData.DataChanged += Update;
            Settings.Current.RefreshRateData.DataChanged +=
                (sender, e) => refreshTimer.Interval = TimeSpan.FromSeconds(1 / Settings.Current.RefreshRate);
            notifyIcon = new NotifyIcon { Icon = CurrentApp.DrawingIcon, Text = CurrentApp.Title, Visible = true };
            notifyIcon.MouseClick += NotifyIconClicked;
            notifyIcon.ShowBalloonTip(10000, CurrentApp.Title, "Welcome! Left click here to configure your cursor, right click to quit.",
                                      ToolTipIcon.Info);
        }

        // ReSharper disable NotAccessedField.Local
        private readonly DispatcherTimer refreshTimer;
        // ReSharper restore NotAccessedField.Local

        private SettingsWindow settingsWindow;
        private SettingsWindow SettingsWindow { get { return settingsWindow ?? (settingsWindow = new SettingsWindow(this)); } }
        private readonly NotifyIcon notifyIcon;

        private void NotifyIconClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) Close();
            else
            {
                SettingsWindow.Show();
                SettingsWindow.Activate();
            }
        }

        private readonly CursorHider hider = new CursorHider();
        private void ShowCursor()
        {
            hider.Show(StandardCursor.AppStarting);
            hider.Show(StandardCursor.Arrow);
            hider.Show(StandardCursor.Hand);
            hider.Show(StandardCursor.Help);
            hider.Show(StandardCursor.No);
            hider.Show(StandardCursor.SizeAll);
            hider.Show(StandardCursor.UpArrow);
            hider.Show(StandardCursor.Wait);
        }
        private void HideCursor()
        {
            hider.Hide(StandardCursor.AppStarting);
            hider.Hide(StandardCursor.Arrow);
            hider.Hide(StandardCursor.Hand);
            hider.Hide(StandardCursor.Help);
            hider.Hide(StandardCursor.No);
            hider.Hide(StandardCursor.SizeAll);
            hider.Hide(StandardCursor.UpArrow);
            hider.Hide(StandardCursor.Wait);
        }

        private void Refresh(object sender, EventArgs e)
        {
            WoGCursor.InvalidateVisual();
            if (!WoGCursor.Paused) BringWindowToTop();
        }

        private void OnClose(object sender, EventArgs e)
        {
            ShowCursor();   // force show the cursor
            Application.Current.Shutdown();
        }

        private void Update(object sender = null, EventArgs e = null)
        {
            if (WoGCursor.Paused) return;
            if (Settings.Current.ShowOriginalCursor) ShowCursor();
            else HideCursor();
        }

        public void SetPaused(bool paused)
        {
#pragma warning disable 665
            if (WoGCursor.Paused = paused) ShowCursor();
            else Update();
#pragma warning restore 665
        }

        protected override void OnLoad(object sender, RoutedEventArgs e)
        {
            base.OnLoad(sender, e);
            Update();
        }
    }
}
