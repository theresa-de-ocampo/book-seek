using System.Windows;
using System.Windows.Controls;

namespace BookSeek
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            ToolTipService.ShowOnDisabledProperty.OverrideMetadata
            (
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(true)
            );
        }
    }
}
