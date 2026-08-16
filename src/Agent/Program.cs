using Agent;
using System.Net.WebSockets;
using System.Text;

string nodeId = "test-node-123";

WebSocketHandler.HandleConnection(nodeId).Wait();