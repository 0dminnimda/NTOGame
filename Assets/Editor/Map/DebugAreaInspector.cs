using UnityEditor;

namespace Map
{
    [CustomEditor(typeof(DebugArea))]
    public class DebugAreaInspector : Editor
    {
        private DebugArea _area;

        public override void OnInspectorGUI()
        {
            _area = (DebugArea) target;

            if (_area.areaData != null)
                EditorGUILayout.HelpBox("AreaData: \n" + _area.areaData.ToString(), MessageType.None, true);
            else
                EditorGUILayout.HelpBox("Area data is null", MessageType.Error);
        }
    }
}
