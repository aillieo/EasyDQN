using System;
using System.Collections.Generic;
using AillieoUtils.AI;
using UnityEditor;
using UnityEngine;

namespace Sample
{
    [CustomEditor(typeof(TrainingManager))]
    public class TrainingManagerEditor : Editor
    {
        private static string defaultPath = ".";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Save"))
            {
                string filePath = EditorUtility.SaveFilePanel("where to save?", defaultPath, "dqn", "bytes");
                if(!string.IsNullOrWhiteSpace(filePath))
                {
                    defaultPath = filePath;
                    (target as TrainingManager).Save(filePath);
                }
            }
            if (GUILayout.Button("Load"))
            {
                string filePath = EditorUtility.OpenFilePanel("where to load?", defaultPath, "bytes");
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    defaultPath = filePath;
                    (target as TrainingManager).Load(filePath);
                }
            }
        }
    }
}
