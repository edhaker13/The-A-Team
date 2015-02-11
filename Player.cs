using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public enum PlayerIndex
	{
		PlayerOne = 1,
		PlayerTwo = 2,
	}

	public enum PlayerState
	{
		Idle,
		Moving,
		Shooting,
	}

	public class Player: EntityAlive
	{
		private static int Y_INDEX = 4;
		private static float MoveDelta = 4f;
		private static float PlayerSize = 64; // 64x64 px
		private bool canShoot = true;
		private bool keyboardTest = true;
		private Vector2 Direction;
		private PlayerIndex whichPlayer;
		private PlayerState playerState;
		
		
		//AI variables
		private bool movingLeft = true;
		private bool shooting = false;
		private float fireRate = 800.0f;
		private float curTime = 0.0f;
		private char _element;
		
		//Player Tiles
		public List<Tile> playerTiles = new List<Tile>();
		
		public char Element
		{
			get { return _element; }
			set {
				_element = value;
				TileIndex2D.Y = Y_INDEX - Tile.Elements.IndexOf(value);
			}
		}

		public AttackStatus Attack { get { return attackState; } }
		
		public Player(Vector2 position, bool isPlayer1, List<Tile> tiles):
			base(Y_INDEX, position, new Vector2i(0, 3))
		{

			Element = 'N';

			CenterSprite();
			

			IsDefending = false;

			if(isPlayer1)
				whichPlayer = PlayerIndex.PlayerOne;
			else
				whichPlayer = PlayerIndex.PlayerTwo;
			
			playerState = PlayerState.Idle;
			Direction = new Vector2(1.0f, 0.0f);
			playerTiles = tiles;
		}
		
		override public void Update(float dt)
		{
	
			// Handle battle
			base.Update(dt);
			
			switch(AppMain.TYPEOFGAME)
			{
			case "SINGLE":
				
				// Handle movement/attacks
				HandleInput();
					
				// Apply the movement
				Position = Position + positionDelta;			
				break;
				
			case "MULTIPLAYER":
				if(AppMain.ISHOST && whichPlayer == PlayerIndex.PlayerOne || !AppMain.ISHOST && whichPlayer == PlayerIndex.PlayerTwo)
				{
					// Handle movement/attacks
					HandleInput();
					
					// Apply the movement
					Position = Position + positionDelta;
					// Make camera follow the player
					Info.CameraCenter = Position;
					
					//Set Position for Data Message
					AppMain.client.SetMyPosition(Position.X, Position.Y);
				}
				else
				{
					//set position and direction from the network positions of enemy
					Position = AppMain.client.networkPosition;
					Direction = AppMain.client.NetworkDirection;
					if(AppMain.client.HasShot)
					{
						Shoot();	
						AppMain.client.SetHasShot(false);	
					}
				}
			
				break;
			default:
				break;
			}
			// Rotate player based on his direction
			HandleDirectionAnimation();
			
			// Find current tile and apply collision
			HandleCollision();
	
			// Make camera follow the player
			Info.CameraCenter = Position;
		}
		
		private void HandleInput()
		{
			//var gamePadData = GamePad.GetData(0);

			if(whichPlayer == PlayerIndex.PlayerOne)
			{
				positionDelta.X = Input2.GamePad0.AnalogLeft.X;
				positionDelta.Y = -Input2.GamePad0.AnalogLeft.Y;
			}
			else if(whichPlayer == PlayerIndex.PlayerTwo)
			{
				positionDelta.X = Input2.GamePad0.AnalogRight.X;
				positionDelta.Y = -Input2.GamePad0.AnalogRight.Y;
			}

			
			if(keyboardTest == true)
			{

				if(Input2.GamePad0.Left.Down)
				{
					positionDelta.X = -MoveDelta;
				}
	
				if(Input2.GamePad0.Right.Down)
				{
					positionDelta.X = MoveDelta;	
				}
				
				if(Input2.GamePad0.Up.Down)
				{
					positionDelta.Y = MoveDelta;	
				}
				
				if(Input2.GamePad0.Down.Down)
				{
					positionDelta.Y = -MoveDelta;	
				}
			}

			switch(AppMain.TYPEOFGAME)
			{
			case "SINGLE":
			// Preserve Movement vector in Direction
				if(!positionDelta.IsZero())
				{
					Direction = positionDelta.Normalize();
				}
				break;
			case "MULTIPLAYER":
				if(positionDelta.IsZero())
					AppMain.client.SetActionMessage('I');
				else
				{
					AppMain.client.SetActionMessage('M');
					Direction = positionDelta;
					AppMain.client.SetMyDirection(Direction.X, Direction.Y);
				}			
				break;
			default:
				break;
			}

			
			if(Input2.GamePad0.Cross.Down || Input2.GamePad0.Cross.Down && Input2.GamePad0.Left.Down ||

			   Input2.GamePad0.Cross.Down && Input2.GamePad0.Right.Down || Input2.GamePad0.Cross.Down && Input2.GamePad0.Up.Down
			   || Input2.GamePad0.Cross.Down && Input2.GamePad0.Down.Down)
			{
				if(canShoot)
				{
					Shoot();
				}
			}
			if(Input2.GamePad0.Cross.Release)
				canShoot = true;
		}
		
		private void HandleDirectionAnimation()
		{
			// Convert direction into angle
			if(!Direction.IsZero())
			{
				RotationNormalize = Direction;
			}
			
			// Set frame to start of animation range if outside of range
			if(TileIndex2D.X < animationRangeX.X || TileIndex2D.X > animationRangeX.Y)
				TileIndex2D.X = animationRangeX.X;
		}
		
		private void HandleCollision()
		{
			Vector2 nextPos = Position + positionDelta;
			float screenWidth = Director.Instance.GL.Context.Screen.Width;
			float screenHeight = Director.Instance.GL.Context.Screen.Height - 32; // Blank space for UI.
			
			
			if(nextPos.X + PlayerSize > screenWidth + 50)
			{
				Position = new Vector2(screenWidth + 50 - PlayerSize, Position.Y);
			}
			

			if(nextPos.X < 18)
			{
				Position = new Vector2(18, Position.Y);
			}
			
			if(nextPos.Y < 18)
			{
				Position = new Vector2(Position.X, 18);
			}

			if(nextPos.Y + PlayerSize > screenHeight + 50)
			{
				Position = new Vector2(Position.X, screenHeight + 50 - PlayerSize);
			}

			
			
			// Loop through tiles
			foreach(Tile t in Tile.Collisions)
			{

				bool fromLeft = nextPos.X + PlayerSize > t.Position.X + 64;
				bool fromRight = nextPos.X < t.Position.X + Tile.Width;
				bool fromTop = nextPos.Y < t.Position.Y + 18 + Tile.Height;
				bool fromBottom = nextPos.Y + PlayerSize > t.Position.Y + 50;
				
				if(fromLeft && fromRight && fromTop && fromBottom)
				{
					if(!positionDelta.IsZero() && t.IsCollidable && t.Key != Element)
					{
						if(fromLeft && positionDelta.X > 0)
						{
							Position = new Vector2(t.Position.X + 64 - PlayerSize, Position.Y);
						}
						else if(fromRight && positionDelta.X < 0)
						{
							Position = new Vector2(t.Position.X + PlayerSize, Position.Y);
						}
						else if(fromTop && positionDelta.Y < 0)
						{
							Position = new Vector2(Position.X, t.Position.Y + 18 + PlayerSize);
						}
						else if(fromBottom && positionDelta.Y > 0)
						{
							Position = new Vector2(Position.X, t.Position.Y + 50 - PlayerSize);
						}
						positionDelta = Vector2.Zero;
					}
				}
			}
		}
		
		public void Shoot()
		{
			if(AppMain.TYPEOFGAME.Equals("MULTIPLAYER"))
				AppMain.client.SetActionMessage('S');
			playerState = PlayerState.Shooting;
			Vector2 pos = new Vector2(Position.X + Quad.Bounds2().Point11.X / 2, Position.Y + Quad.Bounds2().Point11.Y / 2);
			ProjectileManager.Instance.Shoot(pos, Direction, _element);
			canShoot = false;
			
		}
		
		public void UpdateAI(float dt, Player p)
		{
			if(movingLeft)
			{
				if(Position.X > 30)
				{
					positionDelta = new Vector2(-0.05f * dt, 0.0f);
					Position += positionDelta;
					Direction = positionDelta;
				}
				else
				{
					movingLeft = false;	
				}
			}
			else
			{
				if(Position.X < 930)
				{
					positionDelta = new Vector2(0.05f * dt, 0.0f);
					Position += positionDelta;
					Direction = positionDelta;
				}
				else
				{
					movingLeft = true;	
				}
			}
			
			float dist = Vector2.Distance(p.Position, Position);	
			if(dist < 300)
			{
				curTime += dt;
				if(curTime > fireRate)
				{
					Direction = p.Position - Position;
					Direction = Direction.Normalize();
					Shoot();
					curTime = 0.0f;	
				}
			}
		}
		
		public void ChangeTiles(string type)
		{
			if(type.Equals("Fire"))
			{
				foreach(Tile t in playerTiles)
				{
					if(t.Key == 'N')
						t.Key = 'F';
				}
			}
			else if(type.Equals("Water"))
			{
				
				foreach(Tile t in playerTiles)
				{
					if(t.Key == 'N')
						t.Key = 'W';
				}
			}
		}
	}
}
