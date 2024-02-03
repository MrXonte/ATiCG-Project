using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
#if UNITY_EDITOR
    public class MeshExtractor : MonoBehaviour
    {
        [MenuItem("Assets/Extract Mesh")]
        static void ExtractMesh()
        {
            GameObject obj = Selection.activeGameObject;
            if (obj != null)
            {
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Mesh mesh = meshFilter.sharedMesh;
                    if (mesh != null)
                    {
                        string path = "Assets/" + mesh.name + ".asset";
                        AssetDatabase.CreateAsset(Instantiate(mesh), path);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }
    }
#endif
}