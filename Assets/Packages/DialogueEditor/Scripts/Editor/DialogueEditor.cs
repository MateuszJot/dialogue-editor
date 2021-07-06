using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
//--------------------------------------
//TODO: CLEANUP by regions
//--------------------------------------

public class DialogueEditorWindow : EditorWindow {

    private List<Node> nodes = new List<Node>();
    private List<Connection> nodesConnections = new List<Connection>();

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 dragDelta;
    private Vector2 offsetDelta;

    [MenuItem("Window/Dialogue Editor")]
    private static void OpenWindow() {
        DialogueEditorWindow window = GetWindow<DialogueEditorWindow>();
        window.titleContent = new GUIContent("Dialogue Editor");
    }

    private void OnGUI() {

        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawConnections();
        DrawPreviewConnectionLine(Event.current);
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

    private void DrawConnections() {

        if(nodesConnections == null)
            return;

        for (int i = 0; i < nodesConnections.Count; i++) {
            nodesConnections[i].Draw();
        } 
    }

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
                    OnDrag(e.delta);
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

        nodes.Add(new TextNode(mousePos, 200, 50, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    private void OnClickInPoint(ConnectionPoint inPoint) {

        selectedInPoint = inPoint;
 
        if (selectedOutPoint == null)
            return;

        if (selectedOutPoint.node != selectedInPoint.node) {
            CreateConnection();
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
            CreateConnection();
            ClearConnectionSelection();
        } else {
            ClearConnectionSelection();
        }
    }

    private void OnClickRemoveConnection(Connection connection) {
        nodesConnections.Remove(connection);
    }
 
    private void CreateConnection() {
        if (nodesConnections == null)
            nodesConnections = new List<Connection>();
 
        nodesConnections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        GUI.changed = true;
    }
 
    private void ClearConnectionSelection() {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void OnClickRemoveNode(Node node) {

        if(nodesConnections == null)
            return;

        List<Connection> connectionsToRemove = new List<Connection>();
 
        for (int i = 0; i < nodesConnections.Count; i++) {
            if (nodesConnections[i].inPoint == node.inPoint || nodesConnections[i].outPoint == node.outPoint) {
                connectionsToRemove.Add(nodesConnections[i]);
            }
        }
 
        for (int i = 0; i < connectionsToRemove.Count; i++) {
            nodesConnections.Remove(connectionsToRemove[i]);
        }
 
        connectionsToRemove = null;
        nodes.Remove(node);
    }

    private void OnDrag(Vector2 delta) {

        dragDelta = delta;
 
        if(nodes == null)
            return;

        for (int i = 0; i < nodes.Count; i++) {
            nodes[i].Drag(delta);
        }
 
        GUI.changed = true;
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
}