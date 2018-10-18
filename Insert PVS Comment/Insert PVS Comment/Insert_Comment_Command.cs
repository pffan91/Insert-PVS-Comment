// This is an open source non-commercial project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;
using EnvDTE;
using System.IO;

namespace Insert_PVS_Comment
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class Insert_Comment_Command
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6c6b1411-3fdb-454b-908c-f2b82cf11f94");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        public readonly AsyncPackage package;

        public static DTE _dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="Insert_Comment_Command"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private Insert_Comment_Command(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static Insert_Comment_Command Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in Insert_Comment_Command's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            _dte = await package.GetServiceAsync(typeof(DTE)) as DTE;

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new Insert_Comment_Command(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Insert_CommentPackage package = this.package as Insert_CommentPackage;
            if (package == null) return;


            switch (package.selectedLicenseType) {
                case LicenseType.Individual:
                    {
                        WriteComment(Constants.individualComment);
                        break;
                    }
                case LicenseType.OpenSource:
                    {
                        WriteComment(Constants.openSourceComment);
                        break;
                    }
                case LicenseType.Student:
                    {
                        WriteComment(Constants.studentComment);
                        break;
                    }
            }
        }

        private void WriteComment(string comment)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_dte == null) return;

            var activeDocument = _dte.ActiveDocument;
            if (activeDocument == null) return;

            var path = (string)activeDocument.ProjectItem.Properties.Item("FullPath").Value;

            string ext = Path.GetExtension(path);
            ext = ext.ToLower();

            if (ext == ".cpp" || ext == ".c" || ext == ".cc" || ext == ".cxx" || ext == ".c++" || ext == ".h" || ext == ".hh" || ext == ".hxx" || ext == ".hpp" || ext == "h++" || ext == ".cs")
            {
                string currentContent = String.Empty;
                if (File.Exists(path))
                {
                    currentContent = File.ReadAllText(path);

                    if (!currentContent.Contains(comment))
                    {
                        File.WriteAllText(path, comment + currentContent);
                    }
                }
            }
        }
    }
}
