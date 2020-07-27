// Copyright (c) 2018 stefan.v
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Demo
{
	[CustomEditor(typeof(HittablesController), true)]
	public class HittablesControllerEditor : Editor
	{
		private void OnEnable()
		{
			GatherRefs();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			//draw button
			EditorGUILayout.BeginHorizontal(GUILayout.Height(EditorGUIUtility.singleLineHeight + 4f * EditorGUIUtility.standardVerticalSpacing));
				EditorGUILayout.Space();
					
					if(GUILayout.Button("Gather Hittables"))
						GatherRefs();

				EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();

			DrawDefaultInspector();
			serializedObject.ApplyModifiedProperties();
		}

		protected void GatherRefs()
		{
			if(EditorApplication.isPlaying)
				return;

			HittablesController controller = (HittablesController)target;
			Hittable[] hittables = FindObjectsOfType<Hittable>();

			//check if we already have all hittables so we don't dirty scene for nothing
			if(hittables.Length == controller.hittables.Count)
			{
				int same = 0;

				foreach(Hittable hittable in controller.hittables)
					//if one not found, save them all
					if(System.Array.IndexOf(hittables, hittable) == -1)
						break;
					else
						same++;
				
				//already saved, exit
				if(same == controller.hittables.Count)
					return;
			}

			//create undo
			Undo.RecordObject(controller, "Ref Set");
			//store
			controller.hittables = new List<Hittable>(hittables);
		}
	}
}