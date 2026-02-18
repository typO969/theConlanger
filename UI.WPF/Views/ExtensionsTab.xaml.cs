using System.Windows;
using System.Windows.Controls;
using UI.WPF.ViewModels;

namespace UI.WPF.Views;

public partial class ExtensionsTab : UserControl
{
    public ExtensionsTab() => InitializeComponent();
}

public sealed class PluginArgumentTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? BoolTemplate { get; set; }
    public DataTemplate? OptionsTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is not PluginArgumentViewModel arg)
            return TextTemplate;

        if (arg.HasOptions)
            return OptionsTemplate ?? TextTemplate;

        if (arg.IsBoolean)
            return BoolTemplate ?? TextTemplate;

        return TextTemplate;
    }
}
