using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyDI
{
    [CustomEditor(typeof(GameObjectContext))]
    public class GameObjectContextEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            var myScript = target as GameObjectContext;
            if (myScript.MonoInstaller.FindAll(_ => _ != null).Count() == 0)
            {
                myScript.IsAutoSearchInstallerInThisGameObject = EditorGUILayout.ToggleLeft(nameof(myScript.IsAutoSearchInstallerInThisGameObject), myScript.IsAutoSearchInstallerInThisGameObject);
            }
            //else

            //SerializedObject serializedObject = new UnityEditor.SerializedObject(myScript);
            var list = serializedObject.FindProperty(nameof(myScript.MonoInstaller));
            EditorGUILayout.PropertyField(list);
            //myScript.MonoInstaller = (List<MonoInstaller>)list.exposedReferenceValue;
            serializedObject.ApplyModifiedProperties();

        }
    }
}
