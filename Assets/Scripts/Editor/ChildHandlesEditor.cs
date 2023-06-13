using System.Collections.Generic;
using System.Linq;
using EditorTools;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(ChildHandles))]
	public class ChildHandlesEditor : UnityEditor.Editor
	{
		private ChildHandles _target;
		private IEnumerable<Transform> _children;

		private void Awake()
		{
			_target = (ChildHandles) target;
			_children = _target.GetComponentsInChildren<Transform>().Where(transform => transform != _target.transform);
		}

		private void OnValidate() => Awake();

		private void OnSceneGUI()
		{
			foreach (Transform child in _children)
			{
				Undo.RecordObject(child, "Moved child");
				child.position = Handles.PositionHandle(child.position, child.rotation);
			}
		}
	}
}
