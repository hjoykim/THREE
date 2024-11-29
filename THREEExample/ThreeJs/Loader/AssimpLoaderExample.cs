using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using THREE;
using Num = System.Numerics;

namespace THREEExample.ThreeJs.Loader
{
    public class FilePicker
    {
        static readonly Dictionary<object, FilePicker> _filePickers = new Dictionary<object, FilePicker>();

        public string RootFolder;
        public string CurrentFolder;
        public string SelectedFile;
        public List<string> AllowedExtensions;
        public bool OnlyAllowFolders;

        public static FilePicker GetFolderPicker(object o, string startingPath)
            => GetFilePicker(o, startingPath, null, true);

        public static FilePicker GetFilePicker(object o, string startingPath, string searchFilter = null, bool onlyAllowFolders = false)
        {
            if (File.Exists(startingPath))
            {
                startingPath = new FileInfo(startingPath).DirectoryName;
            }
            else if (string.IsNullOrEmpty(startingPath) || !Directory.Exists(startingPath))
            {
                startingPath = Environment.CurrentDirectory;
                if (string.IsNullOrEmpty(startingPath))
                    startingPath = AppContext.BaseDirectory;
            }

            if (!_filePickers.TryGetValue(o, out FilePicker fp))
            {
                fp = new FilePicker();
                fp.RootFolder = startingPath;
                fp.CurrentFolder = startingPath;
                fp.OnlyAllowFolders = onlyAllowFolders;

                if (searchFilter != null)
                {
                    if (fp.AllowedExtensions != null)
                        fp.AllowedExtensions.Clear();
                    else
                        fp.AllowedExtensions = new List<string>();

                    fp.AllowedExtensions.AddRange(searchFilter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                }

                _filePickers.Add(o, fp);
            }

            return fp;
        }

        public static void RemoveFilePicker(object o) => _filePickers.Remove(o);
        public bool Draw()
        {
            ImGui.Text("Current Folder: " + System.IO.Path.GetFileName(RootFolder) + CurrentFolder.Replace(RootFolder, ""));
            bool result = false;

            if (ImGui.BeginChild(1, new Num.Vector2(400, 400)))
            {
                var di = new DirectoryInfo(CurrentFolder);
                if (di.Exists)
                {
                    if (di.Parent != null && CurrentFolder != RootFolder)
                    {
                        //ImGui.PushStyleColor(ImGuiCol.Text, 0xffffff);
                        if (ImGui.Selectable("../", false, ImGuiSelectableFlags.DontClosePopups))
                            CurrentFolder = di.Parent.FullName;

                        //ImGui.PopStyleColor();
                    }

                    var fileSystemEntries = GetFileSystemEntries(di.FullName);
                    foreach (var fse in fileSystemEntries)
                    {
                        if (Directory.Exists(fse))
                        {
                            var name = System.IO.Path.GetFileName(fse);
                            //ImGui.PushStyleColor(ImGuiCol.Text, 0xffffff);
                            if (ImGui.Selectable(name + "/", false, ImGuiSelectableFlags.DontClosePopups))
                                CurrentFolder = fse;
                            //ImGui.PopStyleColor();
                        }
                        else
                        {
                            var name = System.IO.Path.GetFileName(fse);
                            bool isSelected = SelectedFile == fse;
                            if (ImGui.Selectable(name, isSelected, ImGuiSelectableFlags.DontClosePopups))
                                SelectedFile = fse;

                            if (ImGui.IsMouseDoubleClicked(0))
                            {
                                result = true;
                                ImGui.CloseCurrentPopup();
                            }
                        }
                    }
                }
            }
            ImGui.EndChild();


            if (ImGui.Button("Cancel"))
            {
                result = false;
                ImGui.CloseCurrentPopup();
            }

            if (OnlyAllowFolders)
            {
                ImGui.SameLine();
                if (ImGui.Button("Open"))
                {
                    result = true;
                    SelectedFile = CurrentFolder;
                    ImGui.CloseCurrentPopup();
                }
            }
            else if (SelectedFile != null)
            {
                ImGui.SameLine();
                if (ImGui.Button("Open"))
                {
                    result = true;
                    ImGui.CloseCurrentPopup();
                }
            }

            return result;
        }

        bool TryGetFileInfo(string fileName, out FileInfo realFile)
        {
            try
            {
                realFile = new FileInfo(fileName);
                return true;
            }
            catch
            {
                realFile = null;
                return false;
            }
        }

        List<string> GetFileSystemEntries(string fullName)
        {
            var files = new List<string>();
            var dirs = new List<string>();

            foreach (var fse in Directory.GetFileSystemEntries(fullName,"*"))
            {
                if (Directory.Exists(fse))
                {
                    dirs.Add(fse);
                }
                //else if (!OnlyAllowFolders)
                //{
                //    if (AllowedExtensions != null)
                //    {
                //        var ext = Path.GetExtension(fse);
                //        if (AllowedExtensions.Contains(ext))
                //            files.Add(fse);
                //    }
                //    else
                //    {
                //        files.Add(fse);
                //    }
                //}
            }
            foreach(var f in Directory.GetFiles(fullName,"*",SearchOption.TopDirectoryOnly))
            {
                if (!OnlyAllowFolders)
                {
                    if(AllowedExtensions==null || AllowedExtensions.Contains("*.*"))
                    {
                        files.Add(f);
                    }
                    if (AllowedExtensions != null)
                    {
                        var ext = "*"+System.IO.Path.GetExtension(f);
                        if (AllowedExtensions.Contains(ext))
                            files.Add(f);
                    }
                    else
                    {
                        files.Add(f);
                    }
                }
            }

            var ret = new List<string>(dirs);
            ret.AddRange(files);

            return ret;
        }
    }
    [Example("LoaderAssimp", ExampleCategory.ThreeJs, "loader")]
    public class AssimpLoaderExample : GradientBackgroundShaderExample
    {
        bool isOpen = true;
        public AssimpLoaderExample() : base() { }

        public override void BuildScene()
        {
        }

        public override void Init()
        {
            base.Init();
            var currentDir = "../../../../assets/models";
            var picker = FilePicker.GetFilePicker(this, currentDir, "*.*", false);
            AddGuiControlsAction = () =>
            {
                if (ImGui.Button("File Open"))
                {
                    ImGui.OpenPopup("select file");
                }
                bool result = false;
                if(ImGui.BeginPopupModal("select file",ImGuiWindowFlags.NoTitleBar))
                {
                    
                    if (picker.Draw())
                    {
                        try
                        {
                            scene.Children.Clear();
                            scene.Add(ambientLight);
                            scene.Add(directionalLight);
                            AssimpLoader loader = new AssimpLoader();
                            var obj = loader.Load(picker.SelectedFile);
                            FitModelToWindow(obj, true);
                            scene.Add(obj);
                        }
                        catch (Exception ex)
                        {
                            bool open = true;
                            if(ImGui.BeginPopup("Open error"))
                            {
                                ImGui.Text("An error occurred while reading " + picker.SelectedFile + ". \n it's an illegal file format or not supported.");
                                if (ImGui.Button("Close")) ImGui.CloseCurrentPopup();
                                ImGui.EndPopup();
                            }
                        }
                       
                    }
                    ImGui.EndPopup();
                }
            };
        }
    }
}
