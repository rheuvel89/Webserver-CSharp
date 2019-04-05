using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class WebSocketFrameDispatcher {

        private static WebSocketFrameDispatcher dispatcher = new WebSocketFrameDispatcher();
        private List<ServerHandlerListener> listeners = new List<ServerHandlerListener>();

        public static void AddListener(ServerHandlerListener listener) {
            lock (dispatcher) {
                dispatcher.listeners.Add(listener);
            }
        }

        public static void RemoveListener(ServerHandlerListener listener) {
            lock (dispatcher) {
                dispatcher.listeners.Remove(listener);
            }
        }

        public static void ClearListeners() {
            lock (dispatcher) {
                dispatcher.listeners.Clear();
            }
        }

        public static void Dispatch(Response response) {
            lock (dispatcher) {
                dispatcher.listeners.ForEach(l => l.OnServerMessage(response));
            }
        }

        public static void Dispatch(WebSocketFrame frame) {
            lock (dispatcher) {
                dispatcher.listeners.ForEach(l => l.OnServerMessage(frame));
            }
        }

    }

}
