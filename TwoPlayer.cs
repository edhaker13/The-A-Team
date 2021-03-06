using System;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;


using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;

//using System.Net;
//using System.Net.Sockets;
//using System.IO;
//using System.Threading;


namespace TheATeam
{
//	/**
//	 * SocketListenerInterface
//	 */
//	interface ISocketListener
//	{
//		/**
//		 * Accept
//		 */
//		void OnAccept(IAsyncResult AsyncResult);
//		/**
//		 * Connect
//		 */
//		void OnConnect(IAsyncResult AsyncResult);
//		/**
//		 * Receive
//		 */
//		void OnReceive(IAsyncResult AsyncResult);
//		/**
//		 * Send
//		 */
//		void OnSend(IAsyncResult AsyncResult);
//	}
//	/**
//	 * SocketEventCallback
//	 */
//	class SocketEventCallback
//	{
//		/**
//		 * AcceptCallback
//		 */
//		public static void AcceptCallback(IAsyncResult AsyncResult) 
//		{
//			LocalTCPConnection Server = (LocalTCPConnection)AsyncResult.AsyncState;
//			Server.OnAccept(AsyncResult);
//		}
//
//		/**
//		 * ConnectCallback
//		 */
//		public static void ConnectCallback(IAsyncResult AsyncResult)
//		{
//			LocalTCPConnection Client = (LocalTCPConnection)AsyncResult.AsyncState;
//			Client.OnConnect(AsyncResult);
//		}
//		/**
//		 * ReceiveCallback
//		 */
//		public static void ReceiveCallback(IAsyncResult AsyncResult)
//		{
//			LocalTCPConnection TCPs = (LocalTCPConnection)AsyncResult.AsyncState;
//			TCPs.OnReceive(AsyncResult);
//		}
//
//		/**
//		 * SendCallback
//		 */
//		public static void SendCallback(IAsyncResult AsyncResult)
//		{
//			LocalTCPConnection TCPs = (LocalTCPConnection)AsyncResult.AsyncState;
//			TCPs.OnSend(AsyncResult);
//		}
//	}
//	
//	/**
//	 * Class for SocketTCP local connection
//	 */
//	public class LocalTCPConnection : ISocketListener
//	{
//		/**
//		 * Status
//		 */
//		public enum Status
//		{
//			kNone,		
//			kListen,	// Listen or connecting
//			kConnected,	
//			kUnknown
//		}
///*
//		using (CriticalSection CS = new CriticalSection(syncObject))
//		{
//		
//		}
//		public class CriticalSection : IDisposable
//		{
//			private object syncObject = null;
//			public CriticalSection(object SyncObject)
//			{
//				syncObject = SyncObject;
//				Monitor.Enter(syncObject);
//			}
//
//			public virtual void Dispose()
//			{
//				Monitor.Exit(syncObject);
//				syncObject = null;
//			}
//		}
//*/
//        /**
//         * Object for exclusive  socket access
//         */
//        private object syncObject = new object();
//		/**
//		 * Enter critical section
//		 */
//		private void enterCriticalSection() 
//		{
//			Monitor.Enter(syncObject);
//		}
//		/**
//		 * Leave critical section
//		 */
//		private void leaveCriticalSection() 
//		{
//			Monitor.Exit(syncObject);
//		}
//
//		/**
//		 * Get status
//		 * 
//		 * @return Status
//		 */
//		public Status StatusType
//		{
//			get
//			{
//				try
//				{
//					enterCriticalSection();
//					if (Socket == null){
//						return Status.kNone;
//					}
//					else{
//						if (IsServer){
//							if(ClientSocket == null){
//								return Status.kListen;
//							}
//							return Status.kConnected;
//						}
//						else{
//							if(IsConnect == false){
//								return Status.kListen;
//							}
//							return Status.kConnected;
//						}
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//		}
//
//        /**
//         * Get status as string
//         * 
//         * @return status string
//         */
//        public string statusString
//		{
//			get
//			{
//				switch (StatusType)
//				{
//					case Status.kNone:
//						return "None";
//
//					case Status.kListen:
//						if (IsServer){
//							return "Listen";
//						}
//						else{
//							return "Connecting";
//						}
//
//					case Status.kConnected:
//						return "Connected";
//				}
//				return "Unknown";
//			}
//		}
//
//		/**
//		 * Get the button string based on status
//		 * 
//		 * @return button string
//		 */
//		public string buttonString
//		{
//			get
//			{
//				switch (StatusType)
//				{
//					case Status.kNone:
//						if (IsServer){
//							return "Listen";
//						}
//						else{
//							return "Connect";
//						}
//					case Status.kListen:
//						return "Disconnect";
//					case Status.kConnected:
//						return "Disconnect";
//				}
//				return "Unknown";
//			}
//		}
//
//        /**
//		 * Process the button that lets us change the status based on 
//		 * current status 
//         */
//        public void ChangeStatus()
//		{
//			switch(StatusType)
//			{
//				case	Status.kNone:
//					if (IsServer){
//						Listen();
//					}
//					else{
//						Connect();
//					}
//					break;
//
//				case	Status.kListen:
//					Disconnect();
//					break;
//				
//				case	Status.kConnected:
//					Disconnect();
//					break;
//			}
//		}
//
//        /**
//         * transceiver buffer
//         */
//        private byte[] sendBuffer = new byte[8];
//		private byte[] recvBuffer = new byte[8];
//		
//		public string testStatus = "Nothing";
//
//		/**
//		 * Our position or the other party's
//		 */
//		private Sce.PlayStation.Core.Vector2 myPosition		= new Sce.PlayStation.Core.Vector2(999, 999);
//		public	Sce.PlayStation.Core.Vector2 MyPosition
//		{
//			get { return myPosition; }
//		}
//		public	void	SetMyPosition(float X, float Y)
//		{
//			myPosition.X = X;
//			myPosition.Y = Y;
//		}
//
//		
//		public Sce.PlayStation.Core.Vector2 networkPosition	= new Sce.PlayStation.Core.Vector2(999, 999);
//		public Sce.PlayStation.Core.Vector2 NetworkPosition
//		{
//			get { return networkPosition; }
//		}
//		
//		/**
//		 * Are we connected
//		 */
//		private bool isConnect = false;
//		public bool IsConnect
//		{
//					get	{	return isConnect; }
//			private set	{	this.isConnect = value;	}
//		}
//
//        /**
//         * Socket  Listen when server  Server connect when client
//         */
//        private Socket socket;
//		public  Socket Socket 
//		{
//			get	{	return socket;	}
//		}
//
//		/**
//		 * Client socket when server
//		 */
//		private Socket clientSocket;
//		public Socket ClientSocket
//		{
//					get	{	return clientSocket;	}
//			private set	{	this.clientSocket = value;	}
//		}
//
//		/**
//		 * Is this a server
//		 */
//		private bool isServer;
//		public bool IsServer
//		{
//			get	{	return isServer;	}
//		}
//
//		/**
//		 * Port number
//		 */
//		private UInt16 port;
//		public UInt16 Port
//		{
//			get	{	return port;	}
//		}
//		
//		private IPAddress ipAddress;
//		
//
//		/**
//		 * Constructor
//		 */
//		public LocalTCPConnection(bool IsServer, IPAddress ip, UInt16 Port)
//		{
//			isServer  = IsServer;
//			port      = Port;
//			ipAddress = ip;
//		}
//
//        /**
//         * Listen
//         * Can only be executed when server
//         */
//        public bool Listen()
//		{
//			if (isServer == false) {
//				return false;
//			}
//			try
//			{
//				enterCriticalSection();
//				if (socket == null) {
//					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//					
//					
//					// IPEndPoint EP = new IPEndPoint(IPAddress.Any, port);
//					
//					IPAddress ipAdd = IPAddress.Parse("192.168.43.133");
//					//IPAddress ipAdd = IPAddress.Parse(ip);
//					IPEndPoint EP = new IPEndPoint(ipAdd, port);
//					socket.Bind(EP);
//					socket.Listen(1);
//					socket.BeginAccept(new AsyncCallback(SocketEventCallback.AcceptCallback), this);
//				}
//			}
//			finally
//			{
//				leaveCriticalSection();
//			}
//			return true;
//		}
//
//        /**
//         * Connect to the local host server
//         * 
//         * Can only be executed when client
//         */
//        public bool Connect() 
//		{
//			if (isServer == true){
//				return false;
//			}
//			try
//			{
//				enterCriticalSection();
//				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//
//			
//				IPAddress ipAdd = IPAddress.Parse("192.168.43.133");
//				//IPEndPoint EP = new IPEndPoint(IPAddress.Loopback, port);
//				IPEndPoint EP = new IPEndPoint(ipAdd, port);
//				socket.BeginConnect(EP, new AsyncCallback(SocketEventCallback.ConnectCallback), this);
//			}
//			finally
//			{
//				leaveCriticalSection();
//			}
//			return true;
//		}
//
//		/**
//		 * Disconnect
//		 */
//		public void Disconnect() 
//		{
//			try
//			{
//				enterCriticalSection();
//				if (socket != null){
//					if (IsServer){
//						Console.WriteLine("Disconnect Server");
//						if (clientSocket != null){
//							clientSocket.Close();
//							// clientSocket.Shutdown(SocketShutdown.Both);
//							clientSocket = null;
//						}
//					}
//					else{
//						Console.WriteLine("Disconnect Client");
//					}
//					//  socket.Shutdown(SocketShutdown.Both);
//					socket.Close();
//					socket		= null;
//					IsConnect	= false;
//				}
//			}
//			finally
//			{
//				leaveCriticalSection();
//			}
//		}
//
//        /**
//         * Data transceiver 
//         */
//        public bool DataExchange()
//		{
//			try 
//			{
//				try
//				{
//					enterCriticalSection();
//					byte[] ArrayX	= BitConverter.GetBytes(myPosition.X);
//					byte[] ArrayY = BitConverter.GetBytes(myPosition.Y);
//					ArrayX.CopyTo(sendBuffer, 0);
//					ArrayY.CopyTo(sendBuffer, ArrayX.Length);
//					
//				
//					if (isServer){
//						if (clientSocket == null || IsConnect == false){
//							return false;
//						}
//						clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
//						clientSocket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);
//						
//					}
//					else{
//						if (socket == null || IsConnect == false){
//							return false;
//						}
//						socket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, new AsyncCallback(SocketEventCallback.SendCallback), this);
//						socket.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0, new AsyncCallback(SocketEventCallback.ReceiveCallback), this);
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//			catch(System.Net.Sockets.SocketException e)
//			{
//				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
//					Console.WriteLine("DataExchange 切断検出");
//					Disconnect();
//				}
//				Console.WriteLine("ExchangeError " + e.ToString());
//			}
//			return true;
//		}
//
//
//		/***
//		 * Accept
//		 */
//		public void OnAccept(IAsyncResult AsyncResult)
//		{
//			try
//			{
//				try
//				{
//					enterCriticalSection();
//					if (Socket != null){
//						ClientSocket = Socket.EndAccept(AsyncResult);
//						Console.WriteLine("Accept " + ClientSocket.RemoteEndPoint.ToString());
//						IsConnect = true;
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());
//			}
//			Console.WriteLine("OnAccept");
//			testStatus = "OnAccept";
//		}
//		/***
//		 * Connect
//		 */
//		public void OnConnect(IAsyncResult AsyncResult)
//		{
//			try
//			{
//				try
//				{
//					enterCriticalSection();
//					if (Socket != null){
//						// Complete the connection.
//						Socket.EndConnect(AsyncResult);
//						Console.WriteLine("Connect " + Socket.RemoteEndPoint.ToString());
//						IsConnect = true;
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//			catch (System.Net.Sockets.SocketException e)
//			{
//				if (e.SocketErrorCode == SocketError.ConnectionRefused){
//					Disconnect();
//				}
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());
//			}
//			Console.WriteLine("OnConnect");
//			testStatus = "OnConnect";
//		}
//		
//		/**
//		 * Receive
//		 */
//		public void OnReceive(IAsyncResult AsyncResult)
//		{
//			int Len = 0;
//			try
//			{
//				try
//				{
//					enterCriticalSection();
//					if (IsServer){
//						if (ClientSocket != null){
//							Len = ClientSocket.EndReceive(AsyncResult);
//							// 切断
//							if (Len <= 0){
//								Disconnect();
//							}
//							else{
//								
//								
//								////////////////////////////////////////////////////////////////////TODO/////////////////////////////////////////
//								
//								
//								
//								
//								networkPosition.X = BitConverter.ToSingle(recvBuffer, 0);
//								networkPosition.Y = BitConverter.ToSingle(recvBuffer, 4);
//								
//								if(networkPosition.X == 0 && networkPosition.Y == 0)
//								{
//									isConnect = true;
//								}
//								Console.WriteLine("Host: OnReceive");
//								testStatus = "Host: OnReceive";
//							}
//						}
//					}
//					else{
//						if (Socket != null){
//							Len = Socket.EndReceive(AsyncResult);
//							// 切断
//							if (Len <= 0){
//								Disconnect();
//							}
//							else{
//								
//
//								
//								networkPosition.X = BitConverter.ToSingle(recvBuffer, 0);
//								networkPosition.Y = BitConverter.ToSingle(recvBuffer, 4);
//								
//								if(networkPosition.X == 0 && networkPosition.Y == 0)
//								{
//									isConnect = true;
//								}
//								Console.WriteLine("Client: OnReceive");
//								
//							}
//						}
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//			catch (System.Net.Sockets.SocketException e)
//			{
//				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
//					Console.WriteLine("ReceiveCallback 切断検出");
//					Disconnect();
//				}
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());
//			}
//			//Console.WriteLine("OnReceive");
//		}
//		
//		/**
//		 * Send
//		 */
//		public void OnSend(IAsyncResult AsyncResult)
//		{
//			int Len = 0;
////			int a = 0;
//			try
//			{
//				try
//				{
//					enterCriticalSection();
//					if (IsServer){
//						if (ClientSocket != null){
//							Len = ClientSocket.EndSend(AsyncResult);
//						}
//					}
//					else{
//						if (Socket != null){
//							Len = Socket.EndSend(AsyncResult);
//						}
//					}
//                    // Disconnection detection should go here...
//					if (Len <= 0){
//						// send error
//					}
//				}
//				finally
//				{
//					leaveCriticalSection();
//				}
//			}
//			catch (System.Net.Sockets.SocketException e)
//			{
//				if (e.SocketErrorCode == SocketError.ConnectionReset || e.SocketErrorCode == SocketError.ConnectionAborted){
//					Console.WriteLine("SendCallback 切断検出");
//					Disconnect();
//				}
//			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());
//			}
////			Console.WriteLine("OnSend");
//		}
//	};
	public class TwoPlayer : Scene
	{
		bool isHost = AppMain.ISHOST;
		string homeIP = "192.168.0.10";
		string uniIP = "10.54.152.214";
		string phoneIP = "192.168.43.133";
		string ip ;
//		private Socket connection ; 
//        private Thread readThread ;
//        private NetworkStream socketStream;
//        private BinaryWriter writer;
//        private BinaryReader reader;
//		TcpListener listener;
		
		//LocalTCPConnection server;
		//LocalTCPConnection client;
		
		bool testing = true;
		
		public bool             isConnected;
		public bool             readyToSend;
		#region Member Properties - Labels
		private Label lblTopLeft;
		private Label lblTopRight;
		private Label lblBottomLeft;
		private Label lblBottomRight;
		private Label lblDebugLeft;
		private Label lblDebugCenter;
		#endregion
		#region Screen dimensions
		private int screenWidth;
		private int screenHeight;
		#endregion
		
		#region Fonts
		Font font;		
		FontMap debugFont;
		#endregion
		
		bool isPlayer1Ready;
		bool isPlayer2Ready;
		bool startConnection;
		
		bool p1FadeUp;
		bool p2FadeUp;
		bool p1FadeDown = true;
		bool p2FadeDown = true;
		
		private static SpriteUV 	sprite;
		private static TextureInfo	textureInfo;
		
		private static SpriteUV 	sprite2;
		private static TextureInfo	texture2Info;
		public TwoPlayer ()
		{
//			IPAddress ipAddress = null;
//			if(isHost)
//			{
//				IPHostEntry host;
//	  	 		
//	   			host = Dns.GetHostEntry(Dns.GetHostName());
//	   			foreach (IPAddress ipp in host.AddressList)
//	   			{
//	     			if (ipp.AddressFamily == AddressFamily.InterNetwork)
//	     			{
//				       	ipAddress = ipp;
//	       				break;
//	     			}
//	   			}
//				
//				
//				
//			//ip = localIP;
//			}
//			//else ip= phoneIP;
		
		
//			if(!testing)
//				ip = accomIP;
			this.Camera.SetViewFromViewport();	// Check documentation - defines a 2D view that matches the viewport(viewport == display region, in our case, the vita screen)
			
			#region Set screen width and height variables
			screenWidth = Director.Instance.GL.Context.Screen.Width;
			screenHeight = Director.Instance.GL.Context.Screen.Height;
			#endregion
			
			#region Instantiate Fonts
			font = new Font(FontAlias.System, 25, FontStyle.Bold);
			debugFont = new FontMap(font, 25);
			
			// Reload the font becuase FontMap disposes of it
			font = new Font(FontAlias.System, 25, FontStyle.Bold);
			#endregion
			#region Instantiate label objects
			lblTopLeft = new Label();
			
			lblTopRight = new Label();
			lblBottomLeft = new Label();
			lblBottomRight = new Label();
			lblDebugLeft = new Label();
			lblDebugCenter = new Label();
			#endregion
			
			#region Assign label values
			lblTopLeft.FontMap = debugFont;
			lblTopLeft.Text = "Player 1";
			lblTopLeft.Position = new Vector2 (100, screenHeight - 200);
			
			lblTopRight.FontMap = debugFont;
			lblTopRight.Text = "Player 2";
			lblTopRight.Position = new Vector2(screenWidth - 200, screenHeight - 200);
			
			lblBottomLeft.FontMap = debugFont;
			lblBottomLeft.Text = "Waiting";
			lblBottomLeft.Position = new Vector2(100, 300);
			
			lblBottomRight.FontMap = debugFont;
			lblBottomRight.Text = "Waiting";
			lblBottomRight.Position = new Vector2(screenWidth -200, 300);
			
			lblDebugLeft.FontMap = debugFont;
			lblDebugLeft.Text = "Waiting for both connections";
			lblDebugLeft.Position = new Vector2(430, 200);
			
			lblDebugCenter.FontMap = debugFont;
			lblDebugCenter.Text = "----";
			lblDebugCenter.Position = new Vector2(430, 100);
			
			
			
			#endregion
			textureInfo  = new TextureInfo("/Application/assets/bullet.png");
			sprite	 		= new SpriteUV();
			sprite 			= new SpriteUV(textureInfo);	
			sprite.Quad.S 	= textureInfo.TextureSizef;
			sprite.Position = new Vector2(50.0f,Director.Instance.GL.Context.GetViewport().Height*0.2f);
			
			texture2Info  = new TextureInfo("/Application/assets/bullet.png");
			sprite2	 		= new SpriteUV();
			sprite2 			= new SpriteUV(textureInfo);	
			sprite2.Quad.S 	= textureInfo.TextureSizef;
			sprite2.Position = new Vector2(450.0f,Director.Instance.GL.Context.GetViewport().Height*0.2f);
			
			sprite.Visible = false;
					sprite2.Visible = false;
			
			
			Sce.PlayStation.HighLevel.UI.EditableText text= new Sce.PlayStation.HighLevel.UI.EditableText();
				text.Text = "Input IP";
				text.SetPosition(300,300);
			#region Add labels to scene (Debug Overlay)
			this.AddChild(lblTopLeft);
			this.AddChild(lblTopRight);
			this.AddChild(lblBottomLeft);
			this.AddChild(lblBottomRight);
			this.AddChild(lblDebugLeft);
			this.AddChild(lblDebugCenter);
			this.AddChild(sprite);
			this.AddChild(sprite2);
			
			
			#endregion
			
			
			Scheduler.Instance.ScheduleUpdateForTarget(this, 1, false);	// Tells the director that this "node" (a.k.a scene - see doc) requires to be updated
			Director.Instance.DebugFlags = Director.Instance.DebugFlags | DebugFlags.DrawGrid;
			this.DrawGridStep = 20.0f;
			
			if(testing)
			{
				if(isHost)
				{
					AppMain.client = new LocalTCPConnection(true,11000);
					//server = new LocalTCPConnection(true, 11000);
					if(AppMain.client.Listen())
					{
						Console.WriteLine("working ");
						lblBottomLeft.Text = "Ready";
						isPlayer1Ready = true;
						lblDebugCenter.Text = "IP Address = " + AppMain.client.GetIP ;	
					}
					else
					{
						Console.WriteLine("Fucked ");
					}
				}
				else
				{
					AppMain.client = new LocalTCPConnection(false,11000);
					AppMain.client.SetIPAddress(AppMain.IPADDRESS);
					//client = new LocalTCPConnection(false, 11000);
					lblDebugCenter.Text = "Client";
				}
				
			}
		}
		
		public override void Draw ()
		{
			
			base.Draw ();
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
//			if(Type.Equals("SERVER"))
//				RunServer();
			if(Input2.GamePad0.Select.Press)
			{
				AppMain.client.Disconnect();
//				if(isHost)
//				{
//					AppMain.client.Disconnect();	
//				}
//				else
//					client.Disconnect();
				AppMain.QUITGAME = true;
			}
			#region notTesting
			if(!testing)
			{
//				if(startConnection)
//				{
//					if(lblBottomLeft.Text.Equals("Waiting"))
//					{
//						FadeText(dt,lblBottomLeft,1,p1FadeUp,p1FadeDown);
//						
//					}
//					if(lblBottomRight.Text.Equals("Waiting"))
//					{
//						FadeText(dt,lblBottomRight,2,p2FadeUp,p2FadeDown);
//					}
//				}
//				else if (!startConnection)
//				{
//					if(isHost)
//					{
//						lblBottomLeft.Text = "Press X";
//						lblBottomRight.Text = "";
//					}
//					else
//					{
//						lblBottomRight.Text = "Press X";
//						lblBottomLeft.Text = "";
//					}
//				}
//				
//				if(Input2.GamePad0.Cross.Press)
//				{
//					if(!startConnection)
//					{
//						if(!isHost)
//						{
//							lblBottomRight.Text = "Waiting";
//					 		Thread readThread = new Thread(new ThreadStart(RunClient));
//		            		readThread.Start();	
//							startConnection = true;
//						}
//						else if(isHost)
//						{
//							lblBottomLeft.Text = "Waiting";
//							Thread readThread = new Thread(new ThreadStart(RunServer));
//		            		readThread.Start();	
//							startConnection = true;
//						}
//					}
//				}
//				
//				if(isPlayer1Ready && isPlayer2Ready)
//				{
//					lblDebugLeft.Text = "Press start to continue";	
//					if(Input2.GamePad0.Start.Press)
//					{
//						
//					}
//				}
			}
			#endregion
			else
			{
				
				
				if(isHost)
				{
					
					if(AppMain.client.IsConnect)
					{
						lblBottomRight.Text = "Ready";
						isPlayer2Ready = true;
					}
					
				}
				else
				{
					lblDebugCenter.Text = AppMain.client.GetIP;
					if(Input2.GamePad0.Circle.Press)
					{
						AppMain.client.Connect();
						lblBottomRight.Text = "Ready";
						isPlayer2Ready = true;
					}
					
//				
					if(AppMain.client.IsConnect)
					{
						lblBottomLeft.Text = "Ready";
						isPlayer1Ready = true;
					}
				}
				
				if(isPlayer1Ready && isPlayer2Ready)
				{
					//sprite.Visible = true;
					//sprite2.Visible = true;
					lblBottomLeft.Visible = false;
					lblBottomRight.Visible = false;
					lblTopLeft.Visible = false;
					lblTopRight.Visible = false;
					lblDebugLeft.Visible = false;
					lblDebugCenter.Visible = false;
					
					MultiplayerLevel level = new MultiplayerLevel();
					level.Camera.SetViewFromViewport();
					GameSceneManager.currentScene = level;
					Director.Instance.ReplaceScene(level);
				}
			}
		}
		
		private void FadeText(float dt,Label l,int player , bool fadeUp, bool fadeDown)
		{
			
					if(!fadeUp && fadeDown)
				{
					l.Color.A -= dt;
					if(l.Color.A < 0)
					{
						if(player == 1)
						{	
							p1FadeDown = false;
							p1FadeUp = true;
						}
						else
						{
							p2FadeDown = false;
							p2FadeUp = true;
						}
					}
				}
			else if(fadeUp && !fadeDown)
				{
					l.Color.A += dt;
					if(l.Color.A > 1)
					{
						if(player == 1)
						{	
							p1FadeDown = true;
							p1FadeUp = false;
						}
						else 
						{
							p2FadeDown = true;
							p2FadeUp = false;
						}	
					}
				}
			
		}
		
		public void RunServer()
		{
//			try {
//				
//				IPAddress local = IPAddress.Parse(ip);
//				
//				listener = new TcpListener(local,5000);
//				listener.Start();
//				
//				
//				while(true)
//				{
//					connection = listener.AcceptSocket();
//					new Thread(new ParameterizedThreadStart(HandleClient)).Start(connection);
//				}
//			} catch (Exception ex) {
//				lblBottomLeft.Text="Player 1 Error" + ip.ToString();
//			}
		}
		public void RunClient()
		{
//			 try
//            {
//				//lblBottomRight.Text="Player 2 Not Ready";
//                //creates new client
//                TcpClient client = new TcpClient();
//                //sets ip address and port
//				
//                client.Connect(ip, 5000);
//				
//				socketStream = client.GetStream();//
//				
//				writer = new BinaryWriter(socketStream);
//				reader = new BinaryReader(socketStream);
//				isConnected = true;
//				readyToSend = true;
//				while(isConnected)
//               {
//                   if(readyToSend)
//                   {
//                    	writer.Write ("Connected");
//						isPlayer2Ready = true;
//						lblBottomRight.Text = "Ready";
//						lblBottomRight.Color.A = 1;
//						readyToSend = false;
//                   }
//					
//					
//				string message = reader.ReadString();
//					if(message.Equals("Ready"))
//					{
//						lblBottomLeft.Text = "Ready";
//                  		lblBottomLeft.Color.A = 1;
//						isPlayer1Ready = true;
//					}
//               }
//			
//			}
//			catch (Exception ex) {
//				lblBottomRight.Color.A = 1;
//				lblBottomRight.Text="Client ERROR";
//			}
//			
			
		}
		
		public void HandleClient(object obj)
		{
			
//			Socket clientSocket = obj as Socket;
//            if(clientSocket != null)
//            {
//                //create a socketstream and a reader which will be used for recieving messages
//                NetworkStream socketStream = new NetworkStream(clientSocket);
//           		writer = new BinaryWriter(socketStream);		
//                BinaryReader reader = new BinaryReader(socketStream);
//              
//
//              string message = reader.ReadString();
//				
//				
//          if(message.Equals("Connected"))
//				{
//					lblBottomRight.Text = "Ready";
//					lblBottomRight.Color.A = 1;
//					
//					isPlayer2Ready = true;
//					writer.Write("Ready");
//					isPlayer1Ready = true;
//					lblBottomLeft.Text = "Ready";
//					lblBottomLeft.Color.A = 1;
//				}
//				
//			}
		}
	}
}
