# Maschine Image Maker

#### What is it?
Maschine Image Maker is a small windows application that helps you create custom images for Maschine 2 samples.

![Screenshot1](https://github.com/tstephansen/MaschineImageMaker/blob/master/Screenshots/Screenshot1.png)

![Screenshot2](https://github.com/tstephansen/MaschineImageMaker/blob/master/Screenshots/Screenshot2.png)

![Screenshot3](https://github.com/tstephansen/MaschineImageMaker/blob/master/Screenshots/Screenshot3.png)

#### Prerequisites:
This software requires you to install the [.NET Desktop Runtime 6.0.4](https://dotnet.microsoft.com/en-us/download/dotnet/6.0). Please install that before downloading and trying to run the software.

#### Install:
There isn't an installer for this app. Just [download the latest release from the releases page](https://github.com/tstephansen/MaschineImageMaker/releases), extract it, and run MaschineImageMaker.exe.

#### How do I use it?
- Open the software.
- Click the "Select Image" button and pick an image you want to use.
- Enter a name for the library in the Name field (this **MUST** match the name for the library in the Maschine 2 software).
- Enter the name of the vendor in the Vendor field (this also **MUST** match the vendor that's listed for the samples in the Maschine 2 software).
- Click the "Create Library and Output Folder" button. This will create the folder and the meta file in the NI Resources folder.
- Enter the text you want to display when the library is selected in Maschine 2.
- Click the "Create VB Logo" button.
- Click the "Edit MST Artwork" button. This will show a rectangle on the screen that you can use to select the part of the image to use. This will maintain its aspect ratio. You can use the mouse scroll wheel (when over the selector) to increase or decrease the size of the selection box. The Zoom Factor drop down menu changes the amount that the image is zoomed when you use the mouse scroll wheel.
- Once you've selected the part of the image to use click the "Save Image" button.
- Repeat the two previous steps for the MST Logo, MST Plugin, and VB Artwork.
- You're done! Either select a new image or close the software. You can also click the "Open NI Resources" button to go to the NI Resources folder (C:\Users\Public\Documents\NI Resources\image) or the "Open Output Folder" button to go to the folder that was just created by the software.

#### Known Issues:
As this is a small side project I didn't spend too much time working on it so there are a few things that need to be fixed eventually.
- You can't have any portion of the selection box outside of the image itself. If you do then an error will be thrown when you click the Save Image button. Eventually I plan on adding a button to expand or shrink the image by creating a new image with a solid background and centering the existing image inside of it.
- When you click and drag the selection box it is anchored to the top left corner of the selection box. Ideally when you click to drag it the position of the mouse inside the box will not change. I may fix this at some point in the future.

#### Contributions:
All contributions or ideas are welcome. Feel free to fork this project or submit PRs if you want to make changes!
