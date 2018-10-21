// This is an open source non-commercial project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

namespace Insert_PVS_Comment
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("ee5b1a7c-f677-4385-85e8-43279f56608a")]
    public class BatchPVSComment : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchPVSComment"/> class.
        /// </summary>
        public BatchPVSComment() : base(null)
        {
            this.Caption = "Batch PVS-Studio Comment";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new BatchPVSCommentControl();
        }
    }
}
