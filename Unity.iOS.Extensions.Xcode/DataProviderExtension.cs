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

    public enum ProjectSchemaType
    {
        Debug,
        Release,
        ReleaseForRunning,
        ReleaseForProfiling
    }

    public class ProjectProperty
    {
        public string Key;
        public string Value;
        public ProjectSchemaType Schema;

        public override string ToString()
        {
            return string.Format("[{2}] {0} == {1}", Key, Value, Schema);
        }
    }



    public class ProjectCapability
    {
        public string Name;
        public string FrameworkName;
        public bool FrameworkRequired;
        public bool EntitlementRequired;

        public override string ToString()
        {
            return string.Format("{0} Framework: {1}", Name, string.IsNullOrEmpty(FrameworkName) ? "None" : FrameworkName);
        }
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

        public static List<ProjectProperty> GetBuildProperties(this PBXProject proj)
        {
            var toReturn = new List<ProjectProperty>();
            var tmp = proj.buildConfigLists[proj.GetConfigListForTarget(proj.GetUnityProjectGUID())].buildConfigs;

            foreach (var guid in tmp)
            {
                var configList = proj.buildConfigs[guid];

                foreach (var entry in configList.GetEntries())
                {
                    BuildConfigEntryData value = entry.Value;
                    if (value != null)
                    {
                        ProjectProperty prop = new ProjectProperty();
                        prop.Key = entry.Key;
                        prop.Value = entry.Value.val[0];

                        foreach (var schemaType in Enum.GetValues(typeof(ProjectSchemaType)))
                        {
                            if (configList.name.Equals(schemaType.ToString()))
                                prop.Schema = (ProjectSchemaType) schemaType;
                        }

                        toReturn.Add(prop);
                    }
                }
            }

            return toReturn;
        }

        public static List<ProjectCapability> GetBuildCapabilities(this PBXProject proj)
        {
            proj.GetProjectInternal().UpdateVars();
            proj.GetProjectInternal().UpdateProps();
            var capabilities = proj.GetProjectInternal().capabilities;

            if (capabilities != null && capabilities.Count > 0)
            {
                List<ProjectCapability> caps = new List<ProjectCapability>();

                foreach (var capability in capabilities)
                {
                    ProjectCapability cap = new ProjectCapability();
                    cap.Name = capability.capability.id;
                    cap.FrameworkName = capability.capability.framework;
                    cap.FrameworkRequired = !capability.capability.optionalFramework;
                    cap.EntitlementRequired = capability.capability.requiresEntitlements;
                    caps.Add(cap);
                }

                return caps;
            }

            return null;
        }

    }
}
