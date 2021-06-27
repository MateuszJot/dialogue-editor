using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DialogueEditorWindow : EditorWindow {

    private List<Node> nodes = new List<Node>();

    [MenuItem("Window/Dialogue Editor")]
    private static void OpenWindow() {
        DialogueEditorWindow window = GetWindow<DialogueEditorWindow>();
        window.titleContent = new GUIContent("Dialogue Editor");
    }

    private void OnGUI() {
        DrawNodes();

        ManageNodeEvents(Event.current);
        ManageEvents(Event.current);
        if(GUI.changed) Repaint();
    }

    private void DrawNodes() {
        if(nodes == null)
            return;

        for(int i = 0; i < nodes.Count; i++) {
            nodes[i].Draw();
        }
    }

    private void ManageEvents(Event e) {
        switch(e.type) {
            case EventType.MouseDown:
                if(e.button == 1){
                    ShowContextMenu(e.mousePosition);
                }
            break;
        }
    }

    private void ManageNodeEvents(Event e) {
        if(nodes == null)
            return;

        for(int i = nodes.Count - 1; i >= 0; i--)
            GUI.changed = nodes[i].ManageEvents(e);
    }

    private void ShowContextMenu(Vector2 mousePos) {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Create node"), false, () => CreateNodeFromContextMenu(mousePos)); 
        genericMenu.ShowAsContext();
    }

    private void CreateNodeFromContextMenu(Vector2 mousePos) {
        nodes.Add(new TextNode(mousePos, 200, 50));
    }
}