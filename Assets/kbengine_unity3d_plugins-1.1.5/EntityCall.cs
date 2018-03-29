namespace KBEngine
{
  	using UnityEngine; 
	using System; 
	using System.Collections; 
	using System.Collections.Generic;
	
	/*
		实体的EntityCall
		关于EntityCall请参考API手册中对它的描述
		https://github.com/kbengine/kbengine/tree/master/docs/api
	*/
    public class EntityCall 
    {
    	// EntityCall的类别
		public enum ENTITYCALL_TYPE
		{
			ENTITYCALL_TYPE_CELL = 0,		// CELL_ENTITYCALL
			ENTITYCALL_TYPE_BASE = 1		// BASE_ENTITYCALL
		}
		
    	public Int32 id = 0;
		public string className = "";
		public ENTITYCALL_TYPE type = ENTITYCALL_TYPE.ENTITYCALL_TYPE_CELL;
		
		private NetworkInterface networkInterface_;
		
		public Bundle bundle = null;
		
		public EntityCall()
		{
			networkInterface_ = KBEngineApp.app.networkInterface();
		}
		
		public virtual void __init__()
		{
		}
		
		bool isBase()
		{
			return type == ENTITYCALL_TYPE.ENTITYCALL_TYPE_BASE;
		}
	
		bool isCell()
		{
			return type == ENTITYCALL_TYPE.ENTITYCALL_TYPE_CELL;
		}
		
		/*
			创建新的call
		*/
		public Bundle newCall()
		{  
			if(bundle == null)
				bundle = Bundle.createObject();
			
			if(type == EntityCall.ENTITYCALL_TYPE.ENTITYCALL_TYPE_CELL)
				bundle.newMessage(Message.messages["Baseapp_onRemoteCallCellMethodFromClient"]);
			else
				bundle.newMessage(Message.messages["Entity_onRemoteMethodCall"]);
	
			bundle.writeInt32(this.id);
			
			return bundle;
		}
		
		/*
			向服务端发送这个call
		*/
		public void sendCall(Bundle inbundle)
		{
			if(inbundle == null)
				inbundle = bundle;
			
			inbundle.send(networkInterface_);
			
			if(inbundle == bundle)
				bundle = null;
		}
    }
    
} 
