using System.Windows;
using System.Windows.Controls;

namespace MaschineImageMaker.Helpers;

/// <summary>
///     This allows the visual state to be changed by changing the state name in the view model.
///     This was taken from https://tdanemar.wordpress.com/2009/11/15/using-the-visualstatemanager-with-the-model-view-viewmodel-pattern-in-wpf-or-silverlight/
/// </summary>
/// <seealso cref="System.Windows.DependencyObject"/>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class StateManager : DependencyObject
{
    /// <summary>
    ///     The visual state property
    /// </summary>
    public static readonly DependencyProperty VisualStateProperty = DependencyProperty.RegisterAttached(
        "VisualState", typeof(string), typeof(StateManager), new PropertyMetadata((s, e) =>
        {
            var stateName = (string)e.NewValue;
            if (string.IsNullOrEmpty(stateName))
                return;
            if (s is not Control control)
                throw new InvalidOperationException("This attached property only supports types derived from Control.");
            VisualStateManager.GoToState(control, stateName, true);
        }));

    /// <summary>
    ///     Sets the visual state.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    public static void SetVisualState(DependencyObject element, string value) => element.SetValue(VisualStateProperty, value);

    /// <summary>
    ///     Gets the visual state.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>System.String.</returns>
    public static string GetVisualState(DependencyObject element) => (string)element.GetValue(VisualStateProperty);
}