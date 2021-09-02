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
            var folders = driver.FindElements(folderElement);

            var folderLinks = folders.Skip(3).Take(14).Select(folder => folder.GetAttribute("href")).ToList();

            foreach (var folder in folderLinks)
            {
                Console.WriteLine(folder.Split("/")[7].Replace("%20", " ").Replace("%3A", "-") + Environment.NewLine);

                driver.Navigate().GoToUrl(folder);

                ChromeController.DownloadFiles(driver);

                driver.Navigate().Back();
            }

            Console.WriteLine();
            Console.WriteLine("Finished.");
        }

        /// <summary>
        /// Downloads the files.
        /// </summary>
        private static void DownloadFiles(IWebDriver driver)
        {
            var fileElement = By.ClassName("org_sakaiproject_content_types_fileUpload");
            var files = driver.FindElements(fileElement);

            var fileLinks = files.Select(folder => folder.GetAttribute("href")).ToList();
            
            var images = new List<string>();

            foreach (var imageLink in fileLinks)
            {
                driver.Navigate().GoToUrl(imageLink);

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

                Thread.Sleep(750);

                driver.Navigate().Back();

                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine($"Progress: {Math.Round((double)(100 * images.Count) / fileLinks.Count)}%, {images.Count}/{fileLinks.Count}");
            }

            var name = driver.Url.Split("/")[7].Replace("%20", " ").Replace("%3A", "-");

            ImageProcessor.CreatePDF(images).Save($"Unit PDFs/{name}.pdf");

            Console.WriteLine($"Saved to 'Unit PDFs/{name}.pdf'" + Environment.NewLine);
        }
    }
}
