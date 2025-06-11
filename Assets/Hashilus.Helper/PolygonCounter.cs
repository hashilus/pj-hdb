using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hashilus.Helper
{
    public class PolygonCounter : MonoBehaviour
    {
        public bool includeMeshFilter = true;
        public bool includeSkinnedMeshRenderer = true;
        public bool includeInactive = false;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PolygonCounter))]
    public class PolygonCounterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var polys = 0;

            var polygonCounter = target as PolygonCounter;
            if (polygonCounter.includeMeshFilter) polys += GetMeshFilterPolygons(polygonCounter.includeInactive);
            if (polygonCounter.includeSkinnedMeshRenderer) polys += GetSkinnedMeshRendererPolygons(polygonCounter.includeInactive);

            EditorGUILayout.LabelField("Triangles: " + polys.ToString());
        }

        int GetMeshFilterPolygons(bool includeInactive = false)
        {
            var filters = (target as Component).GetComponentsInChildren<MeshFilter>(includeInactive);
            return filters.Sum(filter => filter.sharedMesh.triangles.Length) / 3;
        }

        int GetSkinnedMeshRendererPolygons(bool includeInactive = false)
        {
            var skins = (target as Component).GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);
            return skins.Sum(skin => skin.sharedMesh.triangles.Length) / 3;
        }
    }
#endif
}
