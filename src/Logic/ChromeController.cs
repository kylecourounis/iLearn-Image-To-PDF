namespace ImageToPDF.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;

    using ImageToPDF.Core;

    internal class ChromeController
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="ChromeController"/> is initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="ChromeController"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (ChromeController.Initialized)
            {
                return;
            }

            ChromeController.OpenBrowser();

            ChromeController.Initialized = true;
        }

        /// <summary>
        /// Opens an instance of the Chrome browser.
        /// </summary>
        private static void OpenBrowser()
        {
            var deviceDriver = ChromeDriverService.CreateDefaultService(Constants.ChromeDriverLocation);
            deviceDriver.HideCommandPromptWindow = true;

            var options = new ChromeOptions();
            options.AddArguments("--disable-infobars");

            var driver = new ChromeDriver(deviceDriver, options);
            driver.Navigate().GoToUrl(Constants.iLearnDirectory);

            // Waits for the user to login
            while (driver.Url != Constants.iLearnDirectory)
            {
                if (driver.Url == Constants.iLearnDirectory)
                {
                    break;
                }

                Thread.Sleep(2500);
            }

            Console.WriteLine("User logged in. Begin downloading..." + Environment.NewLine);

            ChromeController.DownloadFolders(driver);
        }

        /// <summary>
        /// Downloads the folders.
        /// </summary>
        private static void DownloadFolders(IWebDriver driver)
        {
            var folderElement = By.CssSelector(".directoryIndex a");
            var folders = driver.FindElements(folderElement).Skip(1).ToList();
            
            var folderURLs = folders.Select(folder => folder.GetAttribute("href")).ToList();
            var folderNames = folders.Select(folder => folder.Text).ToList();
            
            for (int i = 0; i < folderURLs.Count; i++)
            {
                var name = folderNames[i];
                Console.WriteLine(name + Environment.NewLine);

                driver.Navigate().GoToUrl(folderURLs[i]);

                ChromeController.DownloadFiles(name, driver);

                driver.Navigate().Back();
            }

            Console.WriteLine();
            Console.WriteLine("Finished.");
        }

        /// <summary>
        /// Downloads the files.
        /// </summary>
        private static void DownloadFiles(string name, IWebDriver driver)
        {
            var fileElement = By.ClassName("org_sakaiproject_content_types_fileUpload");
            var files = driver.FindElements(fileElement);

            var fileLinks = files.Select(folder => folder.GetAttribute("href")).ToList();
            
            var images = new List<string>();

            foreach (var imageLink in fileLinks)
            {
                driver.Navigate().GoToUrl(imageLink);

                if (imageLink.EndsWith(".jpg"))
                {
                    // Credit to https://stackoverflow.com/a/45279910
                    var base64 = ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                    var c = document.createElement('canvas');
                                    var ctx = c.getContext('2d');

                                    var img = document.getElementsByTagName('img')[0];
                                    c.height = img.naturalHeight;
                                    c.width = img.naturalWidth;

                                    ctx.drawImage(img, 0, 0, img.naturalWidth, img.naturalHeight);
                                    var base64String = c.toDataURL();

                                    return base64String;
                                    ") as string;

                    images.Add(base64);
                }
                else if (imageLink.EndsWith(".pdf"))
                {
                    // TODO
                }

                driver.Navigate().Back();

                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine($"Progress: {Math.Round((double)(100 * images.Count) / fileLinks.Count)}%, {images.Count}/{fileLinks.Count}");
            }

            if (images.Count > 0)
            {
                ImageProcessor.CreatePDF(images).Save($"Unit PDFs/{name.Replace(":", " -")}.pdf");
            }

            Console.WriteLine($"Saved to 'Unit PDFs/{name}.pdf'" + Environment.NewLine);
        }
    }
}
