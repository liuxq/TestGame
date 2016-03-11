namespace KBEngine
{
  	using UnityEngine; 
	using System; 
	using System.Collections;
	using System.Collections.Generic;
	
	/*
		这个模块将多个数据包打捆在一起
		由于每个数据包都有最大上限， 向Bundle中写入大量数据将会在内部产生多个MemoryStream
		在send时会全部发送出去
	*/
    public class Bundle 
    {
    	public MemoryStream stream = new MemoryStream();
		public List<MemoryStream> streamList = new List<MemoryStream>();
		public int numMessage = 0;
		public int messageLength = 0;
		public Message msgtype = null;
		private int _curMsgStreamIndex = 0;
		
		public Bundle()
		{
		}
		
		public void newMessage(Message mt)
		{
			fini(false);
			
			msgtype = mt;
			numMessage += 1;

			writeUint16(msgtype.id);

			if(msgtype.msglen == -1)
			{
				writeUint16(0);
				messageLength = 0;
			}

			_curMsgStreamIndex = 0;
		}
		
		public void writeMsgLength()
		{
			if(msgtype.msglen != -1)
				return;

			MemoryStream stream = this.stream;
			if(_curMsgStreamIndex > 0)
			{
				stream = streamList[streamList.Count - _curMsgStreamIndex];
			}
			stream.data()[2] = (Byte)(messageLength & 0xff);
			stream.data()[3] = (Byte)(messageLength >> 8 & 0xff);
		}
		
		public void fini(bool issend)
		{
			if(numMessage > 0)
			{
				writeMsgLength();

				streamList.Add(stream);
				stream = new MemoryStream();
			}
			
			if(issend)
			{
				numMessage = 0;
				msgtype = null;
			}

			_curMsgStreamIndex = 0;
		}
		
		public void send(NetworkInterface networkInterface)
		{
			fini(true);
			
			if(networkInterface.valid())
			{
				for(int i=0; i<streamList.Count; i++)
				{
					stream = streamList[i];
					networkInterface.send(stream.getbuffer());
				}
			}
			else
			{
				Dbg.ERROR_MSG("Bundle::send: networkInterface invalid!");  
			}
			
			streamList.Clear();
			stream.clear();
		}
		
		public void checkStream(int v)
		{
			if(v > stream.space())
			{
				streamList.Add(stream);
				stream = new MemoryStream();
				++ _curMsgStreamIndex;
			}
	
			messageLength += v;
		}
		
		//---------------------------------------------------------------------------------
		public void writeInt8(SByte v)
		{
			checkStream(1);
			stream.writeInt8(v);
		}
	
		public void writeInt16(Int16 v)
		{
			checkStream(2);
			stream.writeInt16(v);
		}
			
		public void writeInt32(Int32 v)
		{
			checkStream(4);
			stream.writeInt32(v);
		}
	
		public void writeInt64(Int64 v)
		{
			checkStream(8);
			stream.writeInt64(v);
		}
		
		public void writeUint8(Byte v)
		{
			checkStream(1);
			stream.writeUint8(v);
		}
	
		public void writeUint16(UInt16 v)
		{
			checkStream(2);
			stream.writeUint16(v);
		}
			
		public void writeUint32(UInt32 v)
		{
			checkStream(4);
			stream.writeUint32(v);
		}
	
		public void writeUint64(UInt64 v)
		{
			checkStream(8);
			stream.writeUint64(v);
		}
		
		public void writeFloat(float v)
		{
			checkStream(4);
			stream.writeFloat(v);
		}
	
		public void writeDouble(double v)
		{
			checkStream(8);
			stream.writeDouble(v);
		}
		
		public void writeString(string v)
		{
			checkStream(v.Length + 1);
			stream.writeString(v);
		}
		
		public void writeBlob(byte[] v)
		{
			checkStream(v.Length + 4);
			stream.writeBlob(v);
		}
    }
} 
