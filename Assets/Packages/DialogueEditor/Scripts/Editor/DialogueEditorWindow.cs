using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DialogueEditorWindow : EditorWindow {

    public Node selectedNode = null;
    public Rect nodeInspector;

    private static DialogueEditor dialogueEditor;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 dragDelta;
    private Vector2 offsetDelta;

    private static DialogueEditorWindow window;

    [MenuItem("Window/Dialogue Editor")]
    private static void OpenWindow() {
        window = GetWindow<DialogueEditorWindow>();
        window.titleContent = new GUIContent("Dialogue Editor");
        dialogueEditor = new DialogueEditor();
    }

    private void OnGUI() {

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawConnections();
        DrawPreviewConnectionLine(Event.current);
        DrawNodes();
        DrawNodeInspector();

        ManageNodeEvents(Event.current);
        ManageEvents(Event.current);

        if(GUI.changed)
            Repaint();
    }

    #region GUI Drawers
        private void DrawNodeInspector() {
            if(selectedNode == null)
                return;

            nodeInspector = new Rect(window.position.size.x - 502, 0, 500, window.position.size.y);
            GUI.Box(nodeInspector, string.Empty, DialogueEditorStyle.DefaultWindowStyle);

            GUILayout.BeginArea(nodeInspector);
                GUILayout.BeginVertical();
                    GUILayout.Label("<b><size=20>" + selectedNode.header + " Inspector" + "</size></b>", DialogueEditorStyle.HeaderStyle);
                    selectedNode.DrawInspectorContent();
                GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawNodes() {
            if(dialogueEditor.nodes == null)
                return;

            for(int i = 0; i < dialogueEditor.nodes.Count; i++)
                dialogueEditor.nodes[i].Draw();
        }

        private void DrawConnections() {
            if(dialogueEditor.nodesConnections == null)
                return;

            for (int i = 0; i < dialogueEditor.nodesConnections.Count; i++)
                dialogueEditor.nodesConnections[i].Draw(); 
        }

        private void DrawPreviewConnectionLine(Event e) {
            if (selectedInPoint != null && selectedOutPoint == null) {
                Handles.DrawBezier(
                    selectedInPoint.rect.center,
                    e.mousePosition,
                    selectedInPoint.rect.center + Vector2.left * 50f,
                    e.mousePosition - Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );
    
                GUI.changed = true;
            }
    
            if (selectedOutPoint != null && selectedInPoint == null) {
                Handles.DrawBezier(
                    selectedOutPoint.rect.center,
                    e.mousePosition,
                    selectedOutPoint.rect.center - Vector2.left * 50f,
                    e.mousePosition + Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );
    
                GUI.changed = true;
            }
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)  {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);
    
            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
    
            offsetDelta += dragDelta * 0.5f;
            Vector3 newOffset = new Vector3(offsetDelta.x % gridSpacing, offsetDelta.y % gridSpacing, 0);
    
            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }
    
            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }
    
            Handles.color = Color.white;
            Handles.EndGUI();
        }
    #endregion

    #region Logic
        private void ManageEvents(Event e) {
            dragDelta = Vector2.zero;

            switch(e.type) {
                case EventType.MouseDown:
                    if (e.button == 0) {
                        ClearConnectionSelection();
                    } else if(e.button == 1) {
                        ShowContextMenu(e.mousePosition);
                    }
                break;
                case EventType.MouseDrag:
                    if (e.button == 0) {
                        OnDrag(e.delta, e);
                    }
                break;
            }
        }

        private void ManageNodeEvents(Event e) {
            if(dialogueEditor.nodes == null)
                return;

            for(int i = dialogueEditor.nodes.Count - 1; i >= 0; i--)
                GUI.changed = dialogueEditor.nodes[i].ManageEvents(e, this);
        }

        private void ShowContextMenu(Vector2 mousePos) {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Create node"), false, () => CreateNodeFromContextMenu(mousePos)); 
            genericMenu.ShowAsContext();
        }

        private void CreateNodeFromContextMenu(Vector2 mousePos) {
            dialogueEditor.AddNode(new TextNode(mousePos, 200, 120, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
        }
    
        private void CreateConnection(ConnectionPoint a, ConnectionPoint b) {
            dialogueEditor.AddConnection(a, b, OnClickRemoveConnection);
            GUI.changed = true;
        }
    
        private void ClearConnectionSelection() {
            selectedInPoint = null;
            selectedOutPoint = null;
        }

        private void OnClickInPoint(ConnectionPoint inPoint) {
            selectedInPoint = inPoint;
    
            if (selectedOutPoint == null)
                return;

            if (selectedOutPoint.node != selectedInPoint.node) {
                CreateConnection(selectedOutPoint, selectedInPoint);
                ClearConnectionSelection(); 
            } else {
                ClearConnectionSelection();
            }
        }

        private void OnClickOutPoint(ConnectionPoint outPoint) {

            selectedOutPoint = outPoint;
    
            if (selectedInPoint == null)
                return;

            if (selectedOutPoint.node != selectedInPoint.node) {
                CreateConnection(selectedOutPoint, selectedInPoint);
                ClearConnectionSelection();
            } else {
                ClearConnectionSelection();
            }
        }

        private void OnClickRemoveNode(Node node) {
            dialogueEditor.RemoveNode(node);

            if(selectedNode == node)
                selectedNode = null;
        }

        private void OnClickRemoveConnection(Connection connection) {
            connection.inPoint.OnConnectionStop(connection);
            connection.outPoint.OnConnectionStop(connection);
            dialogueEditor.RemoveConnection(connection);
        }

        private void OnDrag(Vector2 delta, Event e) {

            if(nodeInspector.Contains(e.mousePosition))
                return;

            dragDelta = delta;
    
            if(dialogueEditor.nodes == null)
                return;

            for (int i = 0; i < dialogueEditor.nodes.Count; i++) {
                dialogueEditor.nodes[i].Drag(delta);
            }
    
            GUI.changed = true;
        }
    #endregion
}