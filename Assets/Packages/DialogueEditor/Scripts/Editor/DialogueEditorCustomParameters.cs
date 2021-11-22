using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum DialogueEditorCustomParameterType { Int, Float, String };
public class DialogueEditorCustomParameter {
    public DialogueEditorCustomParameterType type = DialogueEditorCustomParameterType.Int;
    public string name = string.Empty;
    public object value;

    public DialogueEditorCustomParameter(DialogueEditorCustomParameterType t) {
        this.type = t;
    }
}
public class DialogueEditorCustomParameters {
    public List<DialogueEditorCustomParameter> customParameters { get; private set; } = new List<DialogueEditorCustomParameter>();

    public void Draw() {
        GUILayout.BeginVertical();
            for(int i = 0; i < customParameters.Count; i++) {
                GUILayout.BeginHorizontal();
                    customParameters[i].name = GUILayout.TextField(customParameters[i].name, GUILayout.Width(200));
                    GUI.enabled = false;
                    customParameters[i].type = (DialogueEditorCustomParameterType) EditorGUILayout.EnumPopup(customParameters[i].type, GUILayout.Width(50));
                    GUI.enabled = true;
                    switch(customParameters[i].type) {
                        case DialogueEditorCustomParameterType.Int:
                            if(customParameters[i].value == null)
                                customParameters[i].value = int.Parse(EditorGUILayout.TextField("0"));
                            else
                                customParameters[i].value = int.Parse(EditorGUILayout.TextField(customParameters[i].value.ToString()));
                            break;
                        case DialogueEditorCustomParameterType.Float:
                            if(customParameters[i].value == null)
                                customParameters[i].value = float.Parse(EditorGUILayout.TextField("0,0"));
                            else
                                customParameters[i].value = float.Parse(EditorGUILayout.TextField(customParameters[i].value.ToString()));
                            break;
                        case DialogueEditorCustomParameterType.String:
                            if(customParameters[i].value == null)
                                customParameters[i].value = EditorGUILayout.TextField("...");
                            else
                                customParameters[i].value = EditorGUILayout.TextField(customParameters[i].value.ToString());
                            break;
                    }
                    if(GUILayout.Button("-", GUILayout.Width(25))) {
                        GUI.FocusControl(null);
                        customParameters.RemoveAt(i);
                    }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
                if(GUILayout.Button("+ Integer")) {
                    customParameters.Add(new DialogueEditorCustomParameter(DialogueEditorCustomParameterType.Int));
                    GUI.FocusControl(null);
                }
                if(GUILayout.Button("+ Float")) {
                    customParameters.Add(new DialogueEditorCustomParameter(DialogueEditorCustomParameterType.Float));
                    GUI.FocusControl(null);
                }
                if(GUILayout.Button("+ String")) {
                    customParameters.Add(new DialogueEditorCustomParameter(DialogueEditorCustomParameterType.String));
                    GUI.FocusControl(null);
                }
            GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
}