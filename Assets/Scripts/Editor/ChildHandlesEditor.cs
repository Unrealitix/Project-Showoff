using System;
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

		private void OnEnable()
		{
			_target = (ChildHandles) target;
			_children = _target.GetComponentsInChildren<Transform>().Where(transform => transform != _target.transform);
		}


		private void OnSceneGUI()
		{
			foreach (Transform child in _children)
			{
				EditorGUI.BeginChangeCheck();
				Vector3 newPos = Handles.PositionHandle(child.position, child.rotation);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(child, "Moved child");
					child.position = newPos;
				}
			}
		}
	}
}
