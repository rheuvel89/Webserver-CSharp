using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class MessageDispatcher {

        private static MessageDispatcher messageDispatcher = new MessageDispatcher();
        private List<ServerHandlerListener> listeners = new List<ServerHandlerListener>();

        public static void AddListener(ServerHandlerListener listener) {
            lock (messageDispatcher) {
                messageDispatcher.listeners.Add(listener);
            }
        }

        public static void RemoveListener(ServerHandlerListener listener) {
            lock (messageDispatcher) {
                messageDispatcher.listeners.Remove(listener);
            }
        }

        public static void ClearListeners() {
            lock (messageDispatcher) {
                messageDispatcher.listeners.Clear();
            }
        }

        public static void Dispatch(Response response) {
            lock (messageDispatcher) {
                messageDispatcher.listeners.ForEach(l => l.OnServerMessage(response));
            }
        }

        public static void Dispatch(WebSocketFrame frame) {
            lock (messageDispatcher) {
                messageDispatcher.listeners.ForEach(l => l.OnServerMessage(frame));
            }
        }

    }

}
