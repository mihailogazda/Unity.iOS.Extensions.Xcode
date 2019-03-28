using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_XCODE_API_BUILD
namespace UnityEditor.iOS.Xcode.Extensions
#else
namespace UnityEditor.iOS.Xcode.Extensions.Custom
#endif
{
    public static class DataProviderExtension
    {
        public static List<string> EXT_GetFrameworks(this PBXProject proj)
        {
            var data = proj.GetProjectDataInternal();
            var fileMap = data.GetFileRefMapInternal();

            var frameworks = data.frameworks.GetObjects();


            foreach (var framework in frameworks)
            {
                foreach (var file in framework.files)
                {
                    var comments = fileMap[file];
                    bool ok = false;
                }
            }

            return null;
        }





    }
}
