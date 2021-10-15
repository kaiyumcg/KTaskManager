using AttributeLib;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AttributeEd
{
    [CustomPropertyDrawer(typeof(ToggleFieldAttribute))]
    public class ToggleFieldDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property,
                                               GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        bool val;
        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            ToggleFieldAttribute field = attribute as ToggleFieldAttribute;
            //val = EditorGUI.ToggleLeft(position, field.label, val);
            //val = EditorGUI.Foldout(position, val, field.label);
            //if (val)
            //{
            //    position.y += 30;
            //    EditorGUI.indentLevel++;
            //    EditorGUI.PropertyField(position, property, label, true);
            //    EditorGUI.indentLevel--;
            //    position.y -= 30;
            //    position.y += 30;
            //}
            //todo later implement toggle
            label.text = field.label;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}