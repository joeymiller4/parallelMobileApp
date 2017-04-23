using UnityEditor;

public class CreateAssetsBundle
{
    [MenuItem("Assets/Build AssetsBundle")]
	static void BuildAll ()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}