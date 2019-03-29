# Unity.iOS.Extensions.XCode (.NETCORE Port)

For our internal tooling we need more capabilities from the Unity's XCODE API and we need the module to be .NETCORE 2.0 compliant.

This project does just that. 

## Usage 
Additional methods are listed bellow, and are rather self explanatory.

~~~
	//	Usage example
	PBXProject proj = new PBXProject();
	proj.ReadFromFile(path);

	var frameworks = proj.GetUsedFrameworks();
	var filesInBuild = proj.GetFilesIncludedInBuild();
	var buildProperties = proj.GetBuildProperties();
	var buildCapabilities = proj.GetBuildCapabilities();
~~~

To use the extension methods you need to import the namespace:

`using UnityEditor.iOS.Xcode.Extensions;`

## Notes
Fork from: https://bitbucket.org/Unity-Technologies/xcodeapi
