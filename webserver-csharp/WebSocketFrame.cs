using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class WebSocketFrame {

        public int Size { get; private set; }
        public byte[] Frame {
            get { return (byte[])frame.Clone(); }
        }

        private byte[] frame;
        private byte[] mask;
        private byte[] data;
        private bool isMaskSet;
        private int payloadLength;

        private WebSocketFrame(byte[] frame, int frameSize) {
            this.frame = frame;
            Size = frameSize;
            isMaskSet = IsMaskSet(frame);
            int maskOffset = ExtractMaskOffset();
            mask = ExtractMask(maskOffset);
            int dataOffset = maskOffset + (isMaskSet ? 4 : 0);
            payloadLength = GetPayLoadLength(frame);
            data = ExtractData(dataOffset);
        }

        public WebSocketFrame(byte[] data) {
            this.data = data;
            frame = new byte[256];
            frame[0] = 129;
            payloadLength = data.Length;
            frame[1] = (byte)payloadLength;
            int dataOffset = 2;
            if (payloadLength > 125 && payloadLength < short.MaxValue) {
                frame[1] = 126;
                byte[] bytes = BitConverter.GetBytes((short)payloadLength);
                Array.ConstrainedCopy(bytes, 0, frame, 2, bytes.Length);
                dataOffset += 2;
            } else if (payloadLength > short.MaxValue) {
                frame[1] = 127;
                byte[] bytes = BitConverter.GetBytes((long)payloadLength);
                Array.ConstrainedCopy(bytes, 0, frame, 2, bytes.Length);
                dataOffset += 8;
            }
            isMaskSet = IsMaskSet(frame);
            Array.ConstrainedCopy(data, 0, frame, dataOffset, data.Length);
            Size = 2 + payloadLength;
        }

        public WebSocketFrame(string data) : this(Encoding.UTF8.GetBytes(data)) { }

        private int ExtractMaskOffset() {
            int offset = 2;
            offset += (frame[1] & 0b01111111) == 126 ? 2 : 0;
            offset += (frame[1] & 0b01111111) == 127 ? 8 : 0;
            return offset;
        }

        private byte[] ExtractMask(int maskOffset) {
            byte[] extractedMask = null;
            if (isMaskSet) {
                extractedMask = new byte[4];
                Array.ConstrainedCopy(frame, maskOffset, extractedMask, 0, 4);
            }
            return extractedMask;
        }

        private byte[] ExtractData(int dataOffset) {
            byte[] extractedData = new byte[payloadLength];
            Array.ConstrainedCopy(frame, dataOffset, extractedData, 0, payloadLength);
            return UnmaskBytes(extractedData, mask);
        }

        public string AsString() {
            return Encoding.UTF8.GetString(data);
        }

        public static byte[] UnmaskBytes(byte[] data, byte[] mask) {
            byte[] unmaskedBytes = new byte[data.Length];
            for (int i = 0; i < data.Length; i++) {
                unmaskedBytes[i] = (byte)(data[i] ^ mask[i % mask.Length]);
            }
            return unmaskedBytes;
        }

        public static int GetPayLoadLength(byte[] frame) {
            int payloadLength = frame[1] & 0b01111111;
            if (payloadLength == 126) {
                byte[] bytes = new byte[2];
                Array.ConstrainedCopy(frame, 2, bytes, 0, 2);
                payloadLength = BitConverter.ToInt16(bytes, 0);                
            } else if (payloadLength == 127) {
                throw new NotImplementedException();
                byte[] bytes = new byte[2];
                Array.ConstrainedCopy(frame, 8, bytes, 0, 2);
                payloadLength = BitConverter.ToInt32(bytes, 0);
            }
            return payloadLength;
        }

        public static bool IsMaskSet(byte[] frame) {
            return frame.Length > 1 && (frame[1] & (1 << 7)) != 0;
        }

        public static WebSocketFrame Parse(byte[] frame) {
            int frameSize = 2;
            frameSize += (frame[1] & 0b01111111) == 126 ? 2 : 0;
            frameSize += (frame[1] & 0b01111111) == 127 ? 8 : 0;
            frameSize += (IsMaskSet(frame) ? 4 : 0);
            frameSize += GetPayLoadLength(frame); ;
            return Parse(frame, frameSize);
        }

        public static WebSocketFrame Parse(byte[] frame, int frameSize) {
            return new WebSocketFrame(frame, frameSize);
        }

    }

}
