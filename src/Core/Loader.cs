namespace ImageToPDF.Core
{
    using System.IO;
    using System.Text;

    using ImageToPDF.Logic;

    internal static class Loader
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="Loader"/> is initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="Loader"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (Loader.Initialized)
            {
                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (!Directory.Exists("Unit PDFs"))
            {
                Directory.CreateDirectory("Unit PDFs");
            }

            ChromeController.Initialize();

            Loader.Initialized = true;
        }
    }
}
