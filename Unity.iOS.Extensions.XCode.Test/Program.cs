using System;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

namespace Unity.iOS.Extensions.XCode.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\P\Streams\bcup_dev_a\Client\tools\build_validator\exec\HSS.xcodeproj\project.pbxproj";

            PBXProject proj = new PBXProject();
            proj.ReadFromFile(path);


            var frameworks = proj.EXT_GetFrameworks();

            Console.ReadKey();
        }
    }
}
