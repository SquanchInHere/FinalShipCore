
using FinalShip.Modules.SceneFlow;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FinalShipCore.Runtime.Modules.ModulePair))]
public class ModulePairDrawer : PropertyDrawer
{
    private string[] commandNames = Enum.GetNames(typeof(SceneCommandType));
    private EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty iteration = property.Copy();

        SerializedProperty end = iteration.GetEndProperty();


        if (iteration.NextVisible(true))
        {
            float y = position.y;
            do
            {
                float height = EditorGUI.GetPropertyHeight(iteration, true);
                Rect row = new Rect(
                        position.x,
                        y,
                        position.width,
                        height
                    );

                int selected;

                switch (iteration.name)
                {
                    case "command" :
                        selected = PopupBuilder(row, iteration.name, iteration.stringValue, commandNames);

                        if (selected >= 0) 
                            iteration.stringValue = commandNames[selected];

                        break;
                    case "argument":

                        string[] scenePaths = scenes.Where(scene => scene.enabled)
                            .Select(scene => scene.path).ToArray();

                        string[] sceneNames = scenePaths
                            .Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();

                        selected = PopupBuilder(
                            row,
                            iteration.name,
                            iteration.stringValue,
                            scenePaths,
                            sceneNames
                            );

                        if (selected >= 0) 
                            iteration.stringValue = scenePaths[selected];

                        break;
                    default:
                        EditorGUI.PropertyField(row, iteration, true);
                        break;
                }

                y+= height + EditorGUIUtility.standardVerticalSpacing;
            } while (iteration.NextVisible(false) && !SerializedProperty.EqualContents(iteration, end));

        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = 0f;
        int fieldCount = 0;

        SerializedProperty iteration = property.Copy();

        SerializedProperty end = iteration.GetEndProperty();

        if (iteration.NextVisible(true))
        {
            do {
                totalHeight += EditorGUI.GetPropertyHeight(iteration, true);
                fieldCount++;
            } while (iteration.NextVisible(false) && !SerializedProperty.EqualContents(iteration, end));
        }
        
        return totalHeight + Mathf.Max(0, fieldCount - 1) * EditorGUIUtility.standardVerticalSpacing;
    }

    private int PopupBuilder(Rect row, string name, string value, string[] args, string[] displayNames = null)
    {
        if (args == null || args.Length == 0)
        {
            EditorGUI.LabelField(row, name, "No options");
            return -1;
        }

        int index = Array.IndexOf(args, value);
        int selectedIndex = Mathf.Max(0, index);

        return EditorGUI.Popup(
            row,
            ObjectNames.NicifyVariableName(name),
            selectedIndex,
            displayNames ?? args
        );
    }
}
