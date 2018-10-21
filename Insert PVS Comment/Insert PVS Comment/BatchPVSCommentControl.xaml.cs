// This is an open source non-commercial project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

namespace Insert_PVS_Comment
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Diagnostics;

    /// <summary>
    /// Interaction logic for BatchPVSCommentControl.
    /// </summary>
    public partial class BatchPVSCommentControl : System.Windows.Controls.UserControl
    {
        public ObservableCollection<Node> nodes { get; private set; }
        private List<string> selectedFilePaths = new List<string> ();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchPVSCommentControl"/> class.
        /// </summary>
        public BatchPVSCommentControl()
        {
            nodes = new ObservableCollection<Node>();
            this.InitializeComponent();
            ParseSolution();
        }

        /// <summary>
        /// Take Id from CheckBox Uid and transfer value to CheckBoxId struct
        /// </summary>
        /// <param name="sender">The CheckBox clicked.</param>
        /// <param name="e">Parameters associated to the mouse event.</param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;
            CheckBoxId.checkBoxId = currentCheckBox.Uid;
        }

        private void BtnRunPressed(object sender, RoutedEventArgs e)
        {
            selectedFilePaths.Clear();
            ParseSelectedItems(nodes);

            Insert_CommentPackage package = Insert_Comment_Command.Instance.package as Insert_CommentPackage;
            if (package == null) return;

            switch (package.selectedLicenseType)
            {
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

        public void BtnRefreshPressed(object sender, RoutedEventArgs e)
        {
            nodes.Clear();
            ParseSolution();
        }

        private void ParseSolution()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = Insert_Comment_Command._dte;
            if (dte == null) return;

            Array projects = (Array)dte.ActiveSolutionProjects;

            foreach (Project project in projects)
            {
                Node projectRoot = iterateOverItems(project.ProjectItems, new Node() { Text = project.Name });
                nodes.Add(projectRoot);
                treeView.ItemsSource = nodes;
            }

            updateStatusLable("Refreshed!");
        }

        private Node iterateOverItems(ProjectItems items, Node treeViewItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (ProjectItem item in items)
            {
                if (item.ProjectItems.Count > 0)
                {
                    Node folderTreeItem = new Node() { Text = item.Name };
                    treeViewItem.Children.Add(iterateOverItems(item.ProjectItems, folderTreeItem));
                }
                else
                {
                    if (item.Kind == VSConstants.ItemTypeGuid.PhysicalFolder_string || item.Kind == VSConstants.ItemTypeGuid.VirtualFolder_string) // empty folder
                    {
                        continue;
                    }

                    Node newItem = new Node() { Text = item.Name + " " + (string)item.Properties.Item("FullPath").Value, Path = (string)item.Properties.Item("FullPath").Value };
                    treeViewItem.Children.Add(newItem);
                    iterateOverItems(item.ProjectItems, treeViewItem);
                }
            }

            return treeViewItem;
        }

        private void ParseSelectedItems(ObservableCollection<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                if (node.Children.Count > 0)
                {
                    ParseSelectedItems(node.Children);
                }
                else
                {
                    if (node.Path != null && node.IsChecked == true)
                    {
                        selectedFilePaths.Add(node.Path);
                    }
                }
            }
        }

        private void WriteComment(string comment)
        {
            foreach (string path in selectedFilePaths)
            {
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

            updateStatusLable("Done!");
        }

        private void updateStatusLable(string text)
        {
            lblStatus.Text = text;
            System.Threading.Tasks.Task.Delay(5000).ContinueWith(t => ClearUI());
        }

        private void ClearUI()
        {
            this.Dispatcher.Invoke(() =>
            {
                lblStatus.Text = "";
            });
        }
    }
}