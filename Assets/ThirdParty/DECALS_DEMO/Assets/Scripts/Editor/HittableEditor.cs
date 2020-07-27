﻿// Copyright (c) 2018 stefan.v
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

using UnityEditor;
using UnityEngine;

namespace Demo
{
	[CustomEditor(typeof(Hittable), true)]
	public class HittableEditor : Editor
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
					
					if(GUILayout.Button("Set Refs"))
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

			Hittable hittable = (Hittable)target;

			hittable.Trans =  hittable.transform as Transform;
			hittable.Filter = hittable.GetComponentInChildren<MeshFilter>();
			hittable.Renderer = hittable.GetComponentInChildren<MeshRenderer>();
		}
	}
}