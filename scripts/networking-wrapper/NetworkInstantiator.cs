using System.Collections.Generic;
using System.Net.Sockets;
using Godot;
using MessagePack;
using powdered_networking.messages;

namespace PowderRoom.scripts.networking_wrapper;

public class ServerInstantiator
{
    public Node TargetParentNode;

    /*public ServerInstantiator(Node node)
    {
        this.TargetParentNode = node;
    }*/
    
    private Dictionary<string, List<Node>> _nodesByPlayer = new Dictionary<string, List<Node>>();

    public void Instantiate(PackedScene scene, string nodeName, string ownerId, NetworkStream stream)
    {
        Node node = instantiateLocally(scene);
        
        RegisterNodesByPlayer(node, ownerId);

        var instantiate = new NetworkInstantiate(nodeName, ownerId);
        var msg = MessagePackSerializer.Serialize<INetworkMessage>(instantiate);
        stream.Write(msg);
    }

    private Node instantiateLocally(PackedScene scene)
    {
        
        Node instantiatedNode = scene.Instantiate();
        //TargetParentNode.AddChild(instantiatedNode);
        return instantiatedNode;
    }

    private void RegisterNodesByPlayer(Node node, string ownerId)
    {
        var nodes = _nodesByPlayer.GetValueOrDefault(ownerId, new List<Node>());
        nodes.Add(node);
        
        _nodesByPlayer.Add(ownerId, nodes);
    }
}