using MaschineImageMaker.Helpers;
using MaschineImageMaker.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MaschineImageMaker.ViewModels;

public class MainViewModel : BindableBase
{
    public MainViewModel()
    {
        NiResourcesFolderPath = Properties.Settings.Default.NIResourcesFolder;
        VisualState = "Normal";
        ZoomFactors = new List<double>
        {
            1.01,
            1.02,
            1.03,
            1.04,
            1.05,
            1.06,
            1.07,
            1.08,
            1.09,
            1.10,
            1.11,
            1.12,
            1.13,
            1.14,
            1.15,
            1.16,
            1.17,
            1.18,
            1.19,
            1.2
        };
        SelectedZoomFactor = 1.01;
    }

    #region Commands
    public ICommand SelectImageCommand => new DelegateCommand(BrowseForImage);
    public ICommand EditMstArtworkCommand => new DelegateCommand(EditMstArtwork);
    public ICommand EditMstLogoCommand => new DelegateCommand(EditMstLogo);
    public ICommand EditMstPluginCommand => new DelegateCommand(EditMstPlugin);
    public ICommand EditVbArtworkCommand => new DelegateCommand(EditVbArtwork);
    public ICommand CreateLibraryFolderAndDataCommand => new DelegateCommand(CreateLibraryFolderAndData);

    public ICommand CreateVbLogoCommand =>
        new DelegateCommand(CreateVbLogo, () => LibraryFolderCreated).ObservesCanExecute(() => LibraryFolderCreated);
    public ICommand OpenNiResourcesFolderCommand => new DelegateCommand(OpenNiResourcesFolder);

    public ICommand OpenOutputDirectoryCommand =>
        new DelegateCommand(OpenOutputDirectory, () => LibraryFolderCreated).ObservesCanExecute(() =>
            LibraryFolderCreated);
    public ICommand SaveImageCommand =>
        new DelegateCommand(SaveImage, () => IsSelectorVisible).ObservesCanExecute(() => IsSelectorVisible);

    public ICommand ShowSettingsCommand => new DelegateCommand(ShowSettings);
    public ICommand SaveSettingsCommand => new DelegateCommand(SaveSettings);
    public ICommand CloseSettingsCommand => new DelegateCommand(CloseSettings);
    #endregion

    #region Methods
    private void BrowseForImage()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Image File (*.png,*.jpg,*.jpeg,*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All files (*.*)|*.*",
            RestoreDirectory = false,
            FileName = ""
        };
        if (openFileDialog.ShowDialog() != true) return;
        ImagePath = openFileDialog.FileName;
        ImageScale = 1;
        CreateImageSource();
        IsSelectorVisible = false;
        if (string.IsNullOrEmpty(LibraryName)) return;
        var result =
            MessageBox.Show(
                "Would you like to create a new library for this image?",
                "Create New Library", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;
        LibraryName = null;
        VendorName = null;
        VbLogoName = null;
        LibraryFolderCreated = false;
        ResetArtwork();
    }

    private void SaveImage()
    {
        switch (_selectedImageType)
        {
            case ImageTypes.MstArtwork:
                CreateThumbnailImage(Path.Combine(OutputDirectory, "MST_artwork.png"));
                break;
            case ImageTypes.MstLogo:
                CreateThumbnailImage(Path.Combine(OutputDirectory, "MST_logo.png"));
                break;
            case ImageTypes.MstPlugin:
                CreateThumbnailImage(Path.Combine(OutputDirectory, "MST_plugin.png"));
                break;
            case ImageTypes.VbArtwork:
                CreateThumbnailImage(Path.Combine(OutputDirectory, "VB_artwork.png"));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CreateImageSource()
    {
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.UriSource = new Uri(ImagePath);
        bmp.EndInit();
        var width = Convert.ToInt32(bmp.Width);
        var height = Convert.ToInt32(bmp.Height);
        ImageSize = new Size(width, height);
        ImageSource = bmp;
    }

    private void EditMstArtwork() => CreateMstArtworkVisible = true;
    private void EditMstLogo() => CreateMstLogoVisible = true;
    private void EditMstPlugin() => CreateMstPluginVisible = true;
    private void EditVbArtwork() => CreateVbArtworkVisible = true;

    private void CreateLibraryFolderAndData()
    {
        if (string.IsNullOrEmpty(LibraryName))
        {
            MessageBox.Show("Please enter a name for the library before creating the folder.",
                "Library Name Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrEmpty(VendorName))
        {
            MessageBox.Show("Please enter a vendor before creating the folder.",
                "Vendor Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        try
        {
            var metaPath = Path.Combine(OutputDirectory, $"{LibraryName}.meta");
            if (Directory.Exists(OutputDirectory))
                Directory.Delete(OutputDirectory, true);
            Directory.CreateDirectory(OutputDirectory);
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF - 8\" standalone=\"no\" ?>");
            sb.AppendLine("<resource version=\"1.6.0.0\">");
            sb.AppendLine($"	<name>{LibraryName}</name>");
            sb.AppendLine($"    <vendor>{VendorName}</vendor>");
            sb.AppendLine("    <type>image</type>");
            sb.AppendLine("</resource>");
            File.WriteAllText(metaPath, sb.ToString());
            LibraryFolderCreated = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating the library folder.\n{ex.Message.Trim()}", "Error Creating Folder",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CreateThumbnailImage(string savePath)
    {
        try
        {
            if (string.IsNullOrEmpty(savePath))
                throw new ArgumentNullException(nameof(savePath));
            if (File.Exists(savePath)) File.Delete(savePath);
            var width = Convert.ToInt32(SelectionSize.Width * ImageScale);
            var height = Convert.ToInt32(SelectionSize.Height * ImageScale);
            var x = Convert.ToInt32(SelectorPositionX);
            var y = Convert.ToInt32(SelectorPositionY);
            var resizedBitmap = System.Drawing.Image.FromFile(ImagePath).Resize((int)ActualImageWidth, (int)ActualImageHeight);
            var resizedImage = resizedBitmap.ToBitmapImage();
            var cropped = new CroppedBitmap(resizedImage, new Int32Rect(x, y, width, height));
            var bitmapImage = new BitmapImage();
            var encoder = new PngBitmapEncoder();
            using var stream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(cropped));
            encoder.Save(stream);
            stream.Position = 0;
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            var croppedImage = bitmapImage.Clone().ToBitmap();
            var resizedCroppedImage = _selectedImageType switch
            {
                ImageTypes.MstArtwork => croppedImage.Resize(MstArtworkWidth, MstArtworkHeight),
                ImageTypes.MstLogo => croppedImage.Resize(MstLogoWidth, MstLogoHeight),
                ImageTypes.MstPlugin => croppedImage.Resize(MstPluginWidth, MstPluginHeight),
                ImageTypes.VbArtwork => croppedImage.Resize(VbArtworkWidth, VbArtworkHeight),
                _ => croppedImage
            };
            resizedCroppedImage.Save(savePath);
            stream.Close();
            switch (_selectedImageType)
            {
                case ImageTypes.MstArtwork:
                    MstArtworkCreated = true;
                    break;
                case ImageTypes.MstLogo:
                    MstLogoCreated = true;
                    break;
                case ImageTypes.MstPlugin:
                    MstPluginCreated = true;
                    break;
                case ImageTypes.VbArtwork:
                    VbArtworkCreated = true;
                    break;
            }
            IsSelectorVisible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating the image.\n{ex.Message.Trim()}", "Error Creating Image",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CreateVbLogo()
    {
        try
        {
            var canvas = new Canvas
            {
                Width = 227,
                Height = 47,
                Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
            };
            var label = new Label
            {
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(8, 0, 0, 0),
                Content = VbLogoName,
                Width = 227,
                Height = 47,
                FontSize = 18,
                FontFamily = new FontFamily("Roboto")
            };
            canvas.Children.Add(label);
            canvas.Measure(new Size(2000, 2000));
            canvas.Arrange(new Rect(new Size(canvas.DesiredSize.Width, 47)));
            canvas.UpdateLayout();
            var rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width, (int)canvas.RenderSize.Height, 72d,
                72d, PixelFormats.Default);
            rtb.Render(canvas);
            var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, 227, 47));
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(crop));
            using var fs = File.OpenWrite(Path.Combine(OutputDirectory, "VB_logo.png"));
            pngEncoder.Save(fs);
            VbLogoCreated = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating the VB Logo.\n{ex.Message.Trim()}", "Error Creating VB Logo",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenNiResourcesFolder() => OpenDirectory(NiResourcesFolderPath);

    private void OpenOutputDirectory() => OpenDirectory(OutputDirectory);

    private static void OpenDirectory(string path)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                Arguments = path,
                FileName = "explorer.exe"
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to open directory.\n{ex.Message.Trim()}", "Error Opening Directory",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ResetArtworkVisibility()
    {
        CreateMstArtworkVisible = false;
        CreateMstLogoVisible = false;
        CreateMstPluginVisible = false;
        CreateVbArtworkVisible = false;
    }

    private void ResetArtwork()
    {
        MstArtworkCreated = false;
        MstLogoCreated = false;
        MstPluginCreated = false;
        VbArtworkCreated = false;
        VbLogoCreated = false;
    }

    private void ShowSettings()
    {
        VisualState = "Settings";
    }

    private void SaveSettings()
    {
        Properties.Settings.Default.NIResourcesFolder = NiResourcesFolderPath;
        Properties.Settings.Default.Save();
        VisualState = "Normal";
    }

    private void CloseSettings()
    {
        VisualState = "Normal";
    }
    #endregion

    #region Properties and Fields
    private const int MstArtworkWidth = 134;
    private const int MstArtworkHeight = 66;
    private const int MstLogoWidth = 240;
    private const int MstLogoHeight = 196;
    private const int MstPluginWidth = 127;
    private const int MstPluginHeight = 65;
    private const int VbArtworkWidth = 96;
    private const int VbArtworkHeight = 47;
    private ImageTypes _selectedImageType;

    private string _niResourcesFolderPath;
    public string NiResourcesFolderPath
    {
        get => _niResourcesFolderPath;
        set => SetProperty(ref _niResourcesFolderPath, value);
    }

    private string _outputDirectory;
    public string OutputDirectory
    {
        get => _outputDirectory;
        set => SetProperty(ref _outputDirectory, value);
    }

    private string _imagePath;
    public string ImagePath
    {
        get => _imagePath;
        set => SetProperty(ref _imagePath, value);
    }

    private string _vendorName;
    public string VendorName
    {
        get => _vendorName;
        set => SetProperty(ref _vendorName, value, () =>
        {
            LibraryFolderCreated = false;
            if (string.IsNullOrEmpty(value)) return;
            OutputDirectory = !string.IsNullOrEmpty(LibraryName)
                ? Path.Combine(NiResourcesFolderPath, value, LibraryName)
                : Path.Combine(NiResourcesFolderPath, value);
        });
    }

    private string _libraryName;
    public string LibraryName
    {
        get => _libraryName;
        set => SetProperty(ref _libraryName, value, () =>
        {
            LibraryFolderCreated = false;
            if (string.IsNullOrEmpty(value)) return;
            OutputDirectory = !string.IsNullOrEmpty(VendorName)
                ? Path.Combine(NiResourcesFolderPath, VendorName, value)
                : Path.Combine(NiResourcesFolderPath, value);
        });
    }

    private BitmapImage _imageSource;
    public BitmapImage ImageSource
    {
        get => _imageSource;
        set => SetProperty(ref _imageSource, value);
    }

    private Size _imageSize;
    public Size ImageSize
    {
        get => _imageSize;
        set => SetProperty(ref _imageSize, value);
    }

    private System.Drawing.Size _selectionSize;
    public System.Drawing.Size SelectionSize
    {
        get => _selectionSize;
        set => SetProperty(ref _selectionSize, value);
    }

    private string _vbLogoName;
    public string VbLogoName
    {
        get => _vbLogoName;
        set => SetProperty(ref _vbLogoName, value);
    }

    private double _imageScale;
    public double ImageScale
    {
        get => _imageScale;
        set => SetProperty(ref _imageScale, value);
    }

    private double _selectorPositionX;
    public double SelectorPositionX
    {
        get => _selectorPositionX;
        set => SetProperty(ref _selectorPositionX, value);
    }

    private double _selectorPositionY;
    public double SelectorPositionY
    {
        get => _selectorPositionY;
        set => SetProperty(ref _selectorPositionY, value);
    }

    private double _actualImageWidth;
    public double ActualImageWidth
    {
        get => _actualImageWidth;
        set => SetProperty(ref _actualImageWidth, value);
    }

    private double _actualImageHeight;
    public double ActualImageHeight
    {
        get => _actualImageHeight;
        set => SetProperty(ref _actualImageHeight, value);
    }

    private double _selectedZoomFactor;
    public double SelectedZoomFactor
    {
        get => _selectedZoomFactor;
        set => SetProperty(ref _selectedZoomFactor, value);
    }

    private List<double> _zoomFactors;
    public List<double> ZoomFactors
    {
        get => _zoomFactors;
        set => SetProperty(ref _zoomFactors, value);
    }

    private bool _libraryFolderCreated;
    public bool LibraryFolderCreated
    {
        get => _libraryFolderCreated;
        set => SetProperty(ref _libraryFolderCreated, value);
    }

    private bool _mstArtworkCreated;
    public bool MstArtworkCreated
    {
        get => _mstArtworkCreated;
        set => SetProperty(ref _mstArtworkCreated, value);
    }

    private bool _mstLogoCreated;
    public bool MstLogoCreated
    {
        get => _mstLogoCreated;
        set => SetProperty(ref _mstLogoCreated, value);
    }

    private bool _mstPluginCreated;
    public bool MstPluginCreated
    {
        get => _mstPluginCreated;
        set => SetProperty(ref _mstPluginCreated, value);
    }

    private bool _vbArtworkCreated;
    public bool VbArtworkCreated
    {
        get => _vbArtworkCreated;
        set => SetProperty(ref _vbArtworkCreated, value);
    }

    private bool _vbLogoCreated;
    public bool VbLogoCreated
    {
        get => _vbLogoCreated;
        set => SetProperty(ref _vbLogoCreated, value);
    }

    private bool _createMstArtworkVisible;
    public bool CreateMstArtworkVisible
    {
        get => _createMstArtworkVisible;
        set => SetProperty(ref _createMstArtworkVisible, value, () =>
        {
            if (!value) return;
            IsSelectorVisible = true;
            ImageScale = 1;
            _selectedImageType = ImageTypes.MstArtwork;
            SelectionSize = new System.Drawing.Size(MstArtworkWidth, MstArtworkHeight);
        });
    }

    private bool _createMstLogoVisible;
    public bool CreateMstLogoVisible
    {
        get => _createMstLogoVisible;
        set => SetProperty(ref _createMstLogoVisible, value, () =>
        {
            if (!value) return;
            IsSelectorVisible = true;
            ImageScale = 1;
            _selectedImageType = ImageTypes.MstLogo;
            SelectionSize = new System.Drawing.Size(MstLogoWidth, MstLogoHeight);
        });
    }

    private bool _createMstPluginVisible;
    public bool CreateMstPluginVisible
    {
        get => _createMstPluginVisible;
        set => SetProperty(ref _createMstPluginVisible, value, () =>
        {
            if (!value) return;
            IsSelectorVisible = true;
            ImageScale = 1;
            _selectedImageType = ImageTypes.MstPlugin;
            SelectionSize = new System.Drawing.Size(MstPluginWidth, MstPluginHeight);
        });
    }

    private bool _createVbArtworkVisible;
    public bool CreateVbArtworkVisible
    {
        get => _createVbArtworkVisible;
        set => SetProperty(ref _createVbArtworkVisible, value, () =>
        {
            if (!value) return;
            IsSelectorVisible = true;
            ImageScale = 1;
            _selectedImageType = ImageTypes.VbArtwork;
            SelectionSize = new System.Drawing.Size(VbArtworkWidth, VbArtworkHeight);
        });
    }

    private bool _isSelectorVisible;
    public bool IsSelectorVisible
    {
        get => _isSelectorVisible;
        set => SetProperty(ref _isSelectorVisible, value, () =>
        {
            if (!value)
                ResetArtworkVisibility();
        });
    }

    private string _visualState;
    public string VisualState
    {
        get => _visualState;
        set => SetProperty(ref _visualState, value);
    }
    #endregion
}