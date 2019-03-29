using System;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

namespace Unity.iOS.Extensions.XCode.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];

            PBXProject proj = new PBXProject();
            proj.ReadFromFile(path);

            var frameworks = proj.GetUsedFrameworks();
            var filesInBuild = proj.GetFilesIncludedInBuild();
            var buildProperties = proj.GetBuildProperties();
            var buildCapabilities = proj.GetBuildCapabilities();

            Console.ReadKey();
        }
    }
}
