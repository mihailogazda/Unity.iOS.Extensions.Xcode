using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.iOS.Xcode.PBX;

#if UNITY_XCODE_API_BUILD
namespace UnityEditor.iOS.Xcode.Extensions
#else
namespace UnityEditor.iOS.Xcode.Extensions.Custom
#endif
{
    public class GenericFileInfo
    {
        public string Name;
        public string Path;
        public string GUID;

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Path);
        }
    }

    public class FrameworkFileInfo : GenericFileInfo
    {
    }


    public static class DataProviderExtension
    {
        public static string GetUnityFrameworkGUID(this PBXProject proj)
        {
            var frameworksGuid = proj.GetFrameworksBuildPhaseByTarget(proj.TargetGuidByName(PBXProject.GetUnityTargetName()));
            return frameworksGuid;
        }

        public static string GetUnityProjectGUID(this PBXProject proj)
        {
            return proj.TargetGuidByName(PBXProject.GetUnityTargetName());
        }

        internal static PBXFileReferenceData GetFileInfoFromGUID(this PBXProject proj, string guid)
        {
            var fileRef = proj.BuildFilesGet(guid);
            if (fileRef != null)
            {
                return proj.FileRefsGet(fileRef.fileRef);
            }
            return null;
        }

        public static List<FrameworkFileInfo> GetUsedFrameworks(this PBXProject proj)
        {
            List<FrameworkFileInfo> toReturn = new List<FrameworkFileInfo>();

            var frameworksGuid = proj.GetUnityFrameworkGUID();
            var data = proj.GetProjectDataInternal();
            var frameworks = data.frameworks[frameworksGuid];

            if (frameworks != null)
            {
                foreach (var file in frameworks.files)
                {
                    var fileInfo = proj.GetFileInfoFromGUID(file);
                    if (fileInfo != null)
                    {
                        FrameworkFileInfo info = new FrameworkFileInfo();
                        info.Path = fileInfo.path;
                        info.Name = fileInfo.name;
                        info.GUID = fileInfo.guid;
                        toReturn.Add(info);
                    }
                }
            }

            return toReturn;
        }

        internal static PBXNativeTargetData GetBuildTargetData(this PBXProject proj)
        {
            var targetID = proj.GetUnityProjectGUID();



            foreach (var target in proj.GetProjectDataInternal().nativeTargets.GetEntries())
            {
                if (target.Key.Equals(targetID))
                {
                    return target.Value;
                }
            }
            return null;
        }

        public static List<GenericFileInfo> GetFilesIncludedInBuild(this PBXProject proj)
        {
            var target = proj.GetBuildTargetData();
            if (target == null)
                return null;

            List<GenericFileInfo> fileInfos = new List<GenericFileInfo>();

            foreach (var phase in target.phases)
            {
                var sources = proj.sources[phase];

                if (sources != null)
                {
                    foreach (var file in sources.files)
                    {
                        var fileInfo = proj.GetFileInfoFromGUID(file);

                        if (fileInfo != null)
                        {
                            GenericFileInfo fi = new GenericFileInfo();
                            fi.Name = fileInfo.name;
                            fi.Path = fileInfo.path;
                            fi.GUID = file;
                            fileInfos.Add(fi);
                        }
                    }
                }
            }

            return fileInfos;
        }







    }
}
