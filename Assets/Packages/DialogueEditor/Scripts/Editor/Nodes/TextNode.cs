using System;
using UnityEngine;
using UnityEditor;

public class TextNode : Node {

    public string value { get; private set; } = string.Empty;
    public string valuePreview { get; private set; } = string.Empty;
    public DialogueEditorCustomParameters customParameters { get; private set; } = new DialogueEditorCustomParameters();

    public TextNode(Vector2 position, float width, float height, Action<ConnectionPoint> OnClickInPoint, 
                                Action <ConnectionPoint> OnClickOutPoint, Action<Node> OnClickRemoveNode) {
        this.rect = new Rect(position.x, position.y, width, height);
        this.header = "Text";
        this.inPoint = new ConnectionPoint(this, ConnectionPointType.In, DefaultConnectionPointStyle, OnClickInPoint);
        this.outPoint = new ConnectionPoint(this, ConnectionPointType.Out, DefaultConnectionPointStyle, OnClickOutPoint);
        this.OnRemoveNode = OnClickRemoveNode;
    }

    public override void Draw() {
        GUI.Box(rect, string.Empty, CurrentNodeStyle);
        GUILayout.BeginArea(rect);
            DrawHeader();
            DrawContent();
        GUILayout.EndArea();

        inPoint.Draw();
        outPoint.Draw();
    }

    public override void DrawInspectorContent() {
        GUILayout.Label("Dialogue:");
        value = GUILayout.TextArea(value);
        GUILayout.Label("Dialogue Preview:");
        valuePreview = GUILayout.TextField(valuePreview);
        GUILayout.Label("Custom Properties:");
        customParameters.Draw();
    }

    private void DrawContent() {
        if(valuePreview == string.Empty)
            if(value == string.Empty)
                GUILayout.Label("Empty text node...");
            else
                GUILayout.Label(value);
        else
            GUILayout.Label(valuePreview);
    } 
}