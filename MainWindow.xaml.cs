using System;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace ModernDesign
{
  /// <summary>
  /// MainWindow.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class MainWindow : Window
  {
    [DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    [DllImport("user32.dll")]
    static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

    private Point _startPos;
    private System.Windows.Forms.Screen[] _screens = System.Windows.Forms.Screen.AllScreens;

    public MainWindow()
    {
      InitializeComponent();
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
      if (this.WindowState != WindowState.Maximized)
      {
        main.BorderThickness = new Thickness(0);
        rectMax.Visibility = Visibility.Hidden;
        rectMin.Visibility = Visibility.Visible;
      }
      else
      {
        main.BorderThickness = new Thickness(1);
        rectMax.Visibility = Visibility.Visible;
        rectMin.Visibility = Visibility.Hidden;
      }
    }

    private void Window_LocationChanged(object sender, EventArgs e)
    {
      int sum = 0;
      foreach (var screen in _screens)
      {
        sum += screen.WorkingArea.Width;
        if (sum >= Left + Width / 2)
        {
          MaxHeight = screen.WorkingArea.Height;
          break;
        }
      }
    }

    private void System_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        if (WindowState == WindowState.Maximized && Math.Abs(_startPos.Y - e.GetPosition(null).Y) > 2)
        {
          var point = PointToScreen(e.GetPosition(null));
          WindowState = WindowState.Normal;

          Left = point.X - ActualWidth / 2;
          Top = point.Y - border.ActualHeight / 2;
        }

        DragMove();
      }
    }

    private void System_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        if (e.ClickCount >= 2)
        {
          WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }
        else
        {
          _startPos = e.GetPosition(null);
        }
      }
      else if (e.ChangedButton == MouseButton.Right)
      {
        var pos = PointToScreen(e.GetPosition(this));
        IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
        IntPtr hMenu = GetSystemMenu(hWnd, false);
        int cmd = TrackPopupMenu(hMenu, 0x100, (int)pos.X, (int)pos.Y, 0, hWnd, IntPtr.Zero);
        if (cmd > 0) SendMessage(hWnd, 0x112, (IntPtr)cmd, IntPtr.Zero);
      }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
      WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
    }

    private void Mimimize_Click(object sender, RoutedEventArgs e)
    {
      WindowState = WindowState.Minimized;
    }
  }
}
