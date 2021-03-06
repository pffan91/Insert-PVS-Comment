﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Task = System.Threading.Tasks.Task;
using System.ComponentModel;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace Insert_PVS_Comment
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.3.1", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Insert_CommentPackage.PackageGuidString)]
    [ProvideOptionPage(typeof(OptionPageGrid), "Insert PVS-Studio Comment", "General", 0, 0, true)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideToolWindow(typeof(BatchPVSComment))]
    public sealed class Insert_CommentPackage : AsyncPackage
    {
        /// <summary>
        /// Insert_CommentPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "b91e293b-3ca5-4d5e-b639-1ac2c171447c";

        public LicenseType selectedLicenseType
        {
            get
            {
                var dialogOptions = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return dialogOptions.selectedLicenseType;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Insert_CommentPackage"/> class.
        /// </summary>
        public Insert_CommentPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await Insert_Comment_Command.InitializeAsync(this);
            await BatchPVSCommentCommand.InitializeAsync(this);
        }

        #endregion
    }

    public class OptionPageGrid : DialogPage
    {
        [Category("General")]
        [DisplayName("License Type")]
        [Description("Select License Type")]
        [DefaultValue(LicenseType.Individual)]
        [TypeConverter(typeof(EnumConverter))]
        public LicenseType selectedLicenseType { get; set; } = LicenseType.Individual;

    }

    public enum LicenseType
    {
        Individual,
        OpenSource,
        Student
    }
}
