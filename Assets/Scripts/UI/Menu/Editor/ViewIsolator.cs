using UnityEditor;

namespace UI.Menu.Editor
{
    /// <summary>
    /// This script makes sure only one view is ever active in the inspector.
    /// If you don't want that, set enabled to false.
    /// </summary>
    [CustomEditor(typeof(View), true)]
    public class ViewIsolator : Isolator<View>
    {
        protected override bool enabled => true;
    }
}
