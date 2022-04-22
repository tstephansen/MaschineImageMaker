using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MaschineImageMaker.Controls;

/// <summary>
///     A control that allows the user to select a portion of an image to be cropped.
/// </summary>
/// <seealso cref="Control"/>
public class ImageEditor : Control
{
    static ImageEditor() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageEditor),
    new FrameworkPropertyMetadata(typeof(ImageEditor)));

    public ImageEditor()
    {
        ImageScale = 1;
        ZoomFactor = 1.1;
    }

    #region Events
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        ImageGrid = GetTemplateChild(PART_ImageGrid) as Grid;
        PreviewImage = GetTemplateChild(PART_PreviewImage) as Image;
        SelectionCanvas = GetTemplateChild(PART_SelectionCanvas) as Canvas;
        ImageSelectionBox = GetTemplateChild(PART_ImageSelectionBox) as Rectangle;
        if (ImageGrid != null)
        {
            ImageGrid.Drop += ImageGrid_Drop;
            ImageGrid.DragOver += ImageGrid_DragOver;
        }
        if (ImageSelectionBox != null)
        {
            ImageSelectionBox.MouseMove += ImageSelectionBox_MouseMove;
            ImageSelectionBox.MouseWheel += ImageSelectionBox_MouseWheel;
        }
    }

    private void ImageGrid_DragOver(object sender, DragEventArgs e)
    {
        var dropPosition = e.GetPosition(PreviewImage);
        Canvas.SetLeft(ImageSelectionBox, dropPosition.X);
        Canvas.SetTop(ImageSelectionBox, dropPosition.Y);
        SelectorPositionX = Canvas.GetLeft(ImageSelectionBox);
        SelectorPositionY = Canvas.GetTop(ImageSelectionBox);
        ActualImageWidth = PreviewImage.ActualWidth;
        ActualImageHeight = PreviewImage.ActualHeight;
    }

    private void ImageGrid_Drop(object sender, DragEventArgs e)
    {
        var dropPosition = e.GetPosition(PreviewImage);
        Canvas.SetLeft(ImageSelectionBox, dropPosition.X);
        Canvas.SetTop(ImageSelectionBox, dropPosition.Y);
        ActualImageWidth = PreviewImage.ActualWidth;
        ActualImageHeight = PreviewImage.ActualHeight;
        SelectorPositionX = Canvas.GetLeft(ImageSelectionBox);
        SelectorPositionY = Canvas.GetTop(ImageSelectionBox);
    }

    private void ImageSelectionBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragDrop.DoDragDrop(ImageSelectionBox, ImageSelectionBox, DragDropEffects.Move);
    }

    private void ImageSelectionBox_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (ImageScale == 0)
            ImageScale = 1;
        if (e.Delta > 0)
        {
            ImageScale *= ZoomFactor;
            ImageSelectionBox.Width *= ZoomFactor;
            ImageSelectionBox.Height *= ZoomFactor;
            ActualImageWidth = PreviewImage.ActualWidth;
            ActualImageHeight = PreviewImage.ActualHeight;
            SelectorPositionX = Canvas.GetLeft(ImageSelectionBox);
            SelectorPositionY = Canvas.GetTop(ImageSelectionBox);
        }
        else
        {
            var negativeFactor = 1 - (ZoomFactor - 1);
            ImageScale *= negativeFactor;
            ImageSelectionBox.Width *= negativeFactor;
            ImageSelectionBox.Height *= negativeFactor;
            ActualImageWidth = PreviewImage.ActualWidth;
            ActualImageHeight = PreviewImage.ActualHeight;
            SelectorPositionX = Canvas.GetLeft(ImageSelectionBox);
            SelectorPositionY = Canvas.GetTop(ImageSelectionBox);
        }
    }
    #endregion

    #region Properties
    // ReSharper disable InconsistentNaming
    private const string PART_ImageGrid = "PART_ImageGrid";
    internal Grid ImageGrid;
    private const string PART_PreviewImage = "PART_PreviewImage";
    internal Image PreviewImage;
    private const string PART_SelectionCanvas = "PART_SelectionCanvas";
    internal Canvas SelectionCanvas;
    private const string PART_ImageSelectionBox = "PART_ImageSelectionBox";
    internal Rectangle ImageSelectionBox;
    // ReSharper restore InconsistentNaming
    #endregion

    #region Dependency Properties
    public static readonly DependencyProperty SelectionSizeProperty = DependencyProperty.Register(
        "SelectionSize", typeof(System.Drawing.Size), typeof(ImageEditor),
        new FrameworkPropertyMetadata(default(System.Drawing.Size), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectionSizeChanged));

    /// <summary>
    ///     Gets or sets the size of the selection box.
    /// </summary>
    /// <value>The size of the selection box.</value>
    public System.Drawing.Size SelectionSize
    {
        get => (System.Drawing.Size)GetValue(SelectionSizeProperty);
        set => SetValue(SelectionSizeProperty, value);
    }

    private static void OnSelectionSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImageEditor control && e.NewValue is System.Drawing.Size newSize)
            control.OnSelectionSizeChanged(newSize);
    }

    protected virtual void OnSelectionSizeChanged(System.Drawing.Size newSize)
    {
        Canvas.SetLeft(ImageSelectionBox, 0);
        Canvas.SetTop(ImageSelectionBox, 0);
        ImageSelectionBox.Width = newSize.Width;
        ImageSelectionBox.Height = newSize.Height;
        ImageSelectionBox.Visibility = Visibility.Visible;
    }

    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
        "ImageSource", typeof(BitmapImage), typeof(ImageEditor),
        new FrameworkPropertyMetadata(default(BitmapImage), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    ///     Gets or sets the image source.
    /// </summary>
    /// <value>The image source.</value>
    public BitmapImage ImageSource
    {
        get => (BitmapImage)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly DependencyProperty ImageScaleProperty = DependencyProperty.Register(
        "ImageScale", typeof(double), typeof(ImageEditor),
        new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    ///     Gets or sets the image scale.
    /// </summary>
    /// <value>The image scale.</value>
    public double ImageScale
    {
        get => (double)GetValue(ImageScaleProperty);
        set => SetValue(ImageScaleProperty, value);
    }

    public static readonly DependencyProperty SelectorPositionXProperty = DependencyProperty.Register(
        "SelectorPositionX", typeof(double), typeof(ImageEditor),
        new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    ///     Gets or sets the selector' X coordinate.
    /// </summary>
    /// <value>The selector's X coordinate.</value>
    public double SelectorPositionX
    {
        get => (double)GetValue(SelectorPositionXProperty);
        set => SetValue(SelectorPositionXProperty, value);
    }

    public static readonly DependencyProperty SelectorPositionYProperty = DependencyProperty.Register(
        "SelectorPositionY", typeof(double), typeof(ImageEditor),
        new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    ///     Gets or sets the selector' Y coordinate.
    /// </summary>
    /// <value>The selector's Y coordinate.</value>
    public double SelectorPositionY
    {
        get => (double)GetValue(SelectorPositionYProperty);
        set => SetValue(SelectorPositionYProperty, value);
    }

    public static readonly DependencyProperty ActualImageWidthProperty = DependencyProperty.Register(
        "ActualImageWidth", typeof(double), typeof(ImageEditor),
        new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    ///     Gets or sets the actual width of the image.
    /// </summary>
    /// <value>The actual width of the image.</value>
    public double ActualImageWidth
    {
        get => (double)GetValue(ActualImageWidthProperty);
        set => SetValue(ActualImageWidthProperty, value);
    }

    public static readonly DependencyProperty ActualImageHeightProperty = DependencyProperty.Register(
        "ActualImageHeight", typeof(double), typeof(ImageEditor),
        new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    ///     Gets or sets the actual height of the image.
    /// </summary>
    /// <value>The actual height of the image.</value>
    public double ActualImageHeight
    {
        get => (double)GetValue(ActualImageHeightProperty);
        set => SetValue(ActualImageHeightProperty, value);
    }

    public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(
        "ZoomFactor", typeof(double), typeof(ImageEditor),
        new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    ///     Gets or sets the zoom factor.
    /// </summary>
    /// <value>The zoom factor.</value>
    public double ZoomFactor
    {
        get => (double)GetValue(ZoomFactorProperty);
        set => SetValue(ZoomFactorProperty, value);
    }

    public static readonly DependencyProperty IsSelectorVisibleProperty = DependencyProperty.Register(
        "IsSelectorVisible", typeof(bool), typeof(ImageEditor),
        new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsSelectorVisibleChanged));

    /// <summary>
    ///     Gets or sets a value indicating whether the selector is visible.
    /// </summary>
    /// <value><c>true</c> if the selector is visible, <c>false</c> if not.</value>
    public bool IsSelectorVisible
    {
        get => (bool)GetValue(IsSelectorVisibleProperty);
        set => SetValue(IsSelectorVisibleProperty, value);
    }

    private static void OnIsSelectorVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue == e.NewValue) return;
        if (d is ImageEditor control && e.NewValue is bool isSelectorVisible)
            control.OnIsSelectorVisibleChanged(isSelectorVisible);
    }

    protected virtual void OnIsSelectorVisibleChanged(bool isSelectorVisible) => ImageSelectionBox.Visibility =
        isSelectorVisible ? Visibility.Visible : Visibility.Collapsed;
    #endregion
}
