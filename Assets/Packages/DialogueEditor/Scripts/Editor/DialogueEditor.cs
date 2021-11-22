using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEditor : MonoBehaviour {

    public List<Node> nodes { get; private set; } = new List<Node>();
    public List<Connection> nodesConnections { get; private set; } = new List<Connection>();

    public void AddNode(Node n) {
        nodes.Add(n);
    }

    public bool RemoveNode(Node n) {
        if(nodesConnections == null)
            return false;

        for (int i = 0; i < n.myConnections.Count; i++)
            nodesConnections.Remove(n.myConnections[i]);

        return nodes.Remove(n);
    }

    public bool AddConnection(ConnectionPoint a, ConnectionPoint b, System.Action<Connection> OnRemoveConnection) {
        if (nodesConnections == null)
            nodesConnections = new List<Connection>();

        if(a == b)
            return false;

        if(!a.allowMultipleConnections && a.IsAlreadyConnected)
            return false;

        if(!b.allowMultipleConnections && b.IsAlreadyConnected)
            return false;

        Connection newConnection = new Connection(a, b, OnRemoveConnection);
        if(nodesConnections.Contains(newConnection))
            return false;
            
        a.OnConnectionStart(newConnection);
        b.OnConnectionStart(newConnection);
        nodesConnections.Add(newConnection);
        return true;
    }

    public bool RemoveConnection(Connection c) {
        return nodesConnections.Remove(c);
    }
}