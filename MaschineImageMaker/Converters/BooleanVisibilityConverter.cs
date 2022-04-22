using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MaschineImageMaker.Converters;

public class BooleanVisibilityConverter : MarkupExtension, IValueConverter
{
    public BooleanVisibilityConverter()
    {
        Negate = false;
        FalseVisibility = Visibility.Collapsed;
    }

    public bool Negate { get; set; }

    public Visibility FalseVisibility { get; set; }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var result = bool.TryParse(value?.ToString(), out var bValue);
        if (!result) return value;
        if (bValue && !Negate || !bValue && Negate)
            return Visibility.Visible;
        return FalseVisibility;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is Visibility currentVisibility)
        {
            return currentVisibility == Visibility.Visible;
        }
        return false;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}