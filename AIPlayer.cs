using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;


namespace TheATeam
{
	enum Behaviour
	{
		Attacking,
		SeekElement,
		GetFlag,
		ReturnFlag,
	}
	public class AIPlayer: Player
	{
		private Player player1;
		private Vector2 velocity;
		private Vector2 target, finalTarget;
		private Waypoint targetWP;
		private float maxSpeed = 20.0f;
		private PathFinder pathfinder;
		private bool havePath = false;
		private Behaviour behaviour = Behaviour.SeekElement;
		private float attackDistance = 200.0f;
		private float attackTime = 2.5f;
		private float attackTimer = 0.0f;
		private float attackAngle;
		private float maxFireRate = 600.0f;
		private float shootTimer = 0.0f;
		private Item player1Flag;// = ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag");
		private Item player2Flag;// = ItemManager.Instance.GetItem(ItemType.flag, "Player2Flag");
		private List<Waypoint> path;
		private Item elementTarget;
			
		public bool HavePath{ set{havePath = value;}}
		
		public AIPlayer(Vector2 position, bool isPlayer1, List<Tile> tiles, Player player1):base(position, isPlayer1, tiles)
		{
			this.player1 = player1;
			velocity = new Vector2(0.0f, 0.0f);
			target = Position;
			finalTarget = Position;
			pathfinder = new PathFinder(this);
			path = new List<Waypoint>();
			targetWP = null;
			
			
		}
		
		override public void Update(float dt)
		{
			if(player1Flag == null)
				player1Flag = ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag");
			if(player2Flag == null)
				player2Flag = ItemManager.Instance.GetItem(ItemType.flag, "Player2Flag");

//			List<TouchData> touches = Touch.GetData(0);
//			foreach(TouchData data in touches)
//			{
//				float xPos = (data.X +0.5f) * 960.0f;
//				float yPos = 544.0f -((data.Y +0.5f) * 544.0f);
//				
//				if(new Vector2(xPos, yPos) != finalTarget)
//				{
//					finalTarget = new Vector2(xPos, yPos);
//					path = pathfinder.FindPath(finalTarget);
//				}
//			}
			
			updateBehaviours(dt);
			
			MoveInHeadingDirection(dt);
			HandleCollision(dt);			
			
			shootTimer += dt;
			
			UpdateTarget();
			
			base.HandleDirectionAnimation();
			base.UpdateMana(dt);
			base.UpdateShield(dt);
			base.SlowEffect(dt);
			base.RegenHealth(dt);
		}
		
		void updateBehaviours(float dt)
		{
			
			canShoot = CanShoot();
			
			switch(behaviour)
			{
			case Behaviour.GetFlag:
				GetFlag();
				break;
			case Behaviour.ReturnFlag:
				ReturnFlag();
				break;
			case Behaviour.SeekElement:
				SeekElement();
				break;
			case Behaviour.Attacking:
					Attack (dt);
				break;
			}
			
			if(canShoot)
				Shoot(player1.Center);
			
			if(targetWP != null)
			{
				if(targetWP.tile.IsCollidable && (targetWP.tile.Key != Element || targetWP.tile.Key == 'N'))
				{
					if(_stats.mana > _stats.MaxMana/2)
						Shoot (targetWP.tile.Center);
				}
			}
			
		}
		private bool CanShoot()
		{
			Vector2 toPlayer = player1.Center - Center;
			Vector2 ahead = new Vector2(Center.X + Vec2DNormalize(toPlayer).X * attackDistance* 2/3, Center.Y + Vec2DNormalize(toPlayer).Y * attackDistance* 2/3);
			Vector2 ahead2 = new Vector2(Center.X + Vec2DNormalize(toPlayer).X * attackDistance* 1/3, Center.Y + Vec2DNormalize(toPlayer).Y * attackDistance * 1/3);
			
			if(player1.Center.Distance(Center) < attackDistance)
			{
				foreach(Tile t in Tile.Collisions)
				{
					if(!LineIntersectsTile(ahead, t) && !LineIntersectsTile(ahead2, t))
					{
						return true;
					}
				}
			}
			return false;
		}
		
		private void Attack(float dt)
		{
			if(ItemManager.Player1HoldingFlag && !ItemManager.Player2HoldingFlag) // if p1 has flag and AI doesnt - rush flag
			{
				ChangeBehaviour(Behaviour.GetFlag);
				attackTimer = 0.0f;
				return;
			}
			else if(!ItemManager.Player2HoldingFlag && Center.Distance(player1.Center) > Center.Distance(player1Flag.position)) // if flag is closer then the player - grab flag
			{
				ChangeBehaviour(Behaviour.GetFlag);
				attackTimer = 0.0f;
				return;
			}
			attackTimer += dt / 1000;
			//Debug.WriteLine(attackTimer);
			if(attackTimer < attackTime)
			{
				if(ItemManager.Player2HoldingFlag && !ItemManager.Player1HoldingFlag) // while holding p1 flag (if killed player and they drop flag) - return flag
				{
					ChangeBehaviour(Behaviour.ReturnFlag);
					return;
				}
				if(!havePath)
				{
					finalTarget = attackPosition();
					FindPath(finalTarget);
				}
				if(Center.Distance(finalTarget) < 30.0)
				{
					float attackGap = 250.0f;
					if(!canShoot)
						attackGap = Info.Rnd.Next(-10, 250);
					
					Vector2 point;
					if(ItemManager.Player1HoldingFlag)
						point= player1.Center + Vec2DNormalize(player1Flag.position- player1.Center ) * attackGap;
					else
						point= player1.Center + Vec2DNormalize(player2Flag.position- player1.Center ) * attackGap;
					Vector2 p = new Vector2(point.X * FMath.Cos(attackAngle) + point.Y * FMath.Sin(attackAngle), point.X * -FMath.Sin(attackAngle) + point.Y * FMath.Cos(attackAngle)); 
					if(p.Distance(finalTarget) > 75.0f)// || (p.Distance(finalTarget) < 75.0f && !canShoot))
						havePath = false;
				}
			}
			else if(player1.Center.Distance(Center) < attackDistance)
			{
				attackTimer = 0.0f;
			}
			else
			{
				ChangeBehaviour(Behaviour.GetFlag);
				attackTimer = 0.0f;
			}
		}
		private Vector2 attackPosition()
		{
			float attackGap = 250.0f;
			if(!canShoot)
				attackGap = Info.Rnd.Next(-10, 250);
			Vector2 point;
			if(ItemManager.Player1HoldingFlag && !ItemManager.Player2HoldingFlag || player1.Center.X > Director.Instance.GL.Context.GetViewport().Width * 0.66f)
				point= player1.Center + Vec2DNormalize(player1Flag.position- player1.Center ) * attackGap;
			else
				point= player1.Center + Vec2DNormalize(player2Flag.position- player1.Center ) * attackGap;
			attackAngle = FMath.Radians(Info.Rnd.Next(-10, 10));
			
			//(x cos alpha + y sin alpha, -x sin alpha + y cos alpha)
			Vector2 p = new Vector2(point.X * FMath.Cos(attackAngle) + point.Y * FMath.Sin(attackAngle), point.X * -FMath.Sin(attackAngle) + point.Y * FMath.Cos(attackAngle)); 
			
			if(p.X > Director.Instance.GL.Context.GetViewport().Width || p.Y > Director.Instance.GL.Context.GetViewport().Height || p.X < 0 || p.Y < 0)
				p = attackPosition();
			
			return p;
		}
		
		private void GetFlag()
		{
			if(!ItemManager.Player1HoldingFlag)
			{
				if(ItemManager.Player2HoldingFlag)
				{
					ChangeBehaviour(Behaviour.ReturnFlag);
					return;
				}
				else if(Element == 'N')
				{
					ChangeBehaviour(Behaviour.SeekElement);
					return;
				}
				else if(player1.Center.Distance(Center) < attackDistance)
				{
					if(Center.Distance(player1.Center) < Center.Distance(player1Flag.position))
					{
						ChangeBehaviour(Behaviour.Attacking);
						return;
					}
				}
			}
			else if(ItemManager.Player2HoldingFlag)
			{
				
				ChangeBehaviour(Behaviour.ReturnFlag);
				
				return;
			}

			
			if(!havePath)
			{
				finalTarget = ItemManager.Instance.GetItem(ItemType.flag, "Player1Flag").position;
				FindPath(finalTarget);
			}
			
		}
		
		private void ReturnFlag()
		{
			if(!ItemManager.Player2HoldingFlag)
			{
				ChangeBehaviour(Behaviour.GetFlag);
				return;
			}
			if(ItemManager.Player1HoldingFlag)
			{
				ChangeBehaviour(Behaviour.Attacking);
				return;
			}
			if(!havePath)
			{
				finalTarget = ItemManager.Instance.GetItem(ItemType.flag, "Player2Flag").position;
				FindPath(finalTarget);
			}
	
			
		}
		private void SeekElement()
		{
			if(Element == 'N' || Element2 == 'N')
			{
				if(player1.Element == 'N' && player1.Center.Distance(Center) < attackDistance)
				{
					ChangeBehaviour(Behaviour.Attacking);
					return;
				}
				
				if(!havePath)
				{
					switch(Info.Rnd.Next(1,6))
					{
						case 1:
						elementTarget = ItemManager.Instance.GetItem(ItemType.element, "Fire");
						break;
						case 2:
						elementTarget = ItemManager.Instance.GetItem(ItemType.element, "Water");
						break;
						case 3:
						elementTarget = ItemManager.Instance.GetItem(ItemType.element, "Earth");
						break;
						case 4:
						elementTarget = ItemManager.Instance.GetItem(ItemType.element, "Air");
						break;
						case 5:
						elementTarget = ItemManager.Instance.GetItem(ItemType.element, "Lightning");
						break;
					}
					if(elementTarget.collided)
					{
						elementTarget = ClosestItem();
					}
					
					finalTarget = elementTarget.position;
					FindPath(finalTarget);
				}
				
				if(elementTarget.collided)
					havePath = false;
				
				
			}
			else
			{
				ChangeBehaviour(Behaviour.GetFlag);
			}
			
		}
		
		private Item ClosestItem()
		{
			int shortestDistance = 100;
			Item element = ItemManager.Instance.GetItem(ItemType.element, "Fire");
			
			List<Waypoint> route = new List<Waypoint>();
			
			foreach (Item item in ItemManager.Instance.GetAllItems())
			{
				if(item.Type == ItemType.element && !item.collided)
				{
					int distance;
					route = pathfinder.FindPath(item.position);
					distance = route.Count;
					if (distance < shortestDistance)
					{
						shortestDistance = distance;
						element = item;
					}
				}
			}
			
			return element;
		}
		
		private void ChangeBehaviour(Behaviour b)
		{
			if(behaviour == b)
				return;
			else
			{
				behaviour = b;
				havePath = false;
			}
		}

		private void FindPath(Vector2 pos)
		{
			path = pathfinder.FindPath(pos);
			havePath = true;
		}
		
		private void UpdateTarget()
		{
			if(path.Count > 0)
			{
				targetWP = path[0];
				if(Center.Distance(path[0].tile.Center) <= 30.0f)
				{
					path.RemoveAt(0);
				}
			}

		}
		private Vector2 moveNextTarget()
		{
			if(targetWP != null)
			{
				if(!targetWP.tile.IsCollidable || (targetWP.tile.IsCollidable && targetWP.tile.Key == Element))
					target = targetWP.tile.Center;
			}
			if(path.Count == 0)
				return finalTarget;
			else
			return target;
			
		}
		
		private void MoveInHeadingDirection(float dt)
		{
			dt /= 100.0f;
			//Vector2 force = Seek(target);
			Vector2 acceleration = Arrive(moveNextTarget());

			// a = (v - u)/t  -> v = u + at calculate velocity
			velocity += acceleration * dt;
			float speed = velocity.Length();
			
			if(speed > maxSpeed)
			{
				velocity = velocity.Normalize();
				velocity.X *= maxSpeed;
				velocity.Y *= maxSpeed;
			}
			else if(speed < 1.0f)
			{
				base.Direction = Vec2DNormalize(player1.Position - Position);
				return;
			}
			base.Direction = Vec2DNormalize(velocity);
			// s = s + vt calculate position
			Position = new Vector2(Position.X + velocity.X * _stats.moveSpeed * dt, Position.Y + velocity.Y * _stats.moveSpeed * dt);
	
		}
		
		private Vector2 Seek(Vector2 target)
		{
			Vector2 force = Vec2DNormalize((target - Position));
			
			force.X *= maxSpeed;
			force.Y *= maxSpeed;
			
			force -= velocity;
			
			return force;
		}
		private Vector2 Arrive(Vector2 arriveLocation)
		{
			// travel to next pathfind location (arriveLocation) while checking distance to final target (target)
			Vector2 toTarget = arriveLocation - Position;
				
			float distance = toTarget.Length();
			
			Vector2 force = new Vector2(0.0f, 0.0f);
			
			if(target.Distance(Position) < 64.0f && target.Distance(Position) > 0.0f && distance > 0.0)
			{
				// scale dist by factor 10 otherwise force returned is too small (very slow movement)
				float speed = distance * 10 / maxSpeed;
				force = new Vector2 ((toTarget.X * speed / distance) - velocity.X,(toTarget.Y * speed / distance) - velocity.Y);
	
				return force;
				
			}
			else if( target.Distance(Position) > 0.0f && distance > 0.0)
			{
				float speed = maxSpeed;
				
				force = new Vector2 ((toTarget.X * speed / distance) - velocity.X,(toTarget.Y * speed / distance) - velocity.Y);
			
				return force;
			}
			
			return new Vector2(0.0f, 0.0f);
		}
		private Vector2 Vec2DNormalize(Vector2 v)
		{
			Vector2 vec = v;
			float length = v.Length();
			
			if(length > 0.0f)
			{
				vec.X /= length;
				vec.Y /= length;
			}
			
			return vec;
		}
		
		private Vector2 obstacleAvoidance()
		{
			// distance to project infront of tank (faster tank projects further)
			float detectionDist = 50.0f *( velocity.Length() / maxSpeed);
		
			// create 2 vectors to determine collision infront of tank (max and half dist infront)
			Vector2 ahead = new Vector2(Center.X + Vec2DNormalize(velocity).X * detectionDist, Center.Y + Vec2DNormalize(velocity).Y * detectionDist);
			Vector2 ahead2 = new Vector2(Center.X + Vec2DNormalize(velocity).X * detectionDist/2, Center.Y + Vec2DNormalize(velocity).Y * detectionDist/2);

			Tile nearestObject = FindNearestObstacle(ahead, ahead2);
			Vector2 resultantVelocity = new Vector2(0.0f, 0.0f);
		
			if (null != nearestObject)
			{
				// if tank is going to collide create force away from object
				resultantVelocity = ahead - nearestObject.Center;
				resultantVelocity = resultantVelocity.Normalize();
		
				//resultantVelocity *= maxSpeed;
			}
		
			return resultantVelocity;
		}
		private Tile FindNearestObstacle(Vector2 ahead, Vector2 ahead2)
		{
			Tile nearest = null;
			
			float distanceToObject;
			float distanceToNearest = 1000.0f;
			
			foreach(Tile t in Tile.Collisions)
			{
				if((t.Key != Element || t.Key != 'E') && t.IsCollidable)
				{
					bool collision;
					if(LineIntersectsTile(ahead, t) || LineIntersectsTile(ahead2, t) || LineIntersectsTile(Center, t))
					{
						collision = true;
					}
					else
					{
						collision = false;
					}
					
					distanceToObject = Position.Distance(t.Center);
					
					if(collision && (null == nearest || distanceToObject < distanceToNearest))
					{
						distanceToNearest = distanceToObject;
						nearest = t;
					}
				}
			}
			
			return nearest;
		}
		
		private bool LineIntersectsTile(Vector2 ahead, Tile obstacle)
		{
			float width = Width /2;
			float height = Height /2;
			
//			if (ahead.X == Center.X)
//			{
//				height = 0.0f;
//				width = 0.0f;
//			}
			
			// -------- when sprite are actaully 64x64
			//float oWidth = obstacle.Quad.Bounds2().Point11.X;
			//float oHeight = obstacle.Quad.Bounds2().Point11.Y;
			
			float oWidth = 42.0f ;
			float oHeight = 64.0f;
		
			if (ahead.X + width/2 < obstacle.Center.X - oWidth/ 2.0)
				return false;
			else if (ahead.X - width/2 >  obstacle.Center.X + oWidth / 2.0)
				return false;
			else if (ahead.Y  + height/2 <  obstacle.Center.Y - oHeight / 2.0)
				return false;
			else if (ahead.Y - height/2 >  obstacle.Center.Y + oHeight / 2.0)
				return false;
			else
			{
				return true;
			}
		}
	
		private void Shoot(Vector2 target)
		{
			ShootingDirection = Vec2DNormalize(target - Position);
			Shoot ();
		}
		
		override public void Shoot()
		{
			
			if (_stats.mana >= _stats.manaCost)
			{
				if(shootTimer > maxFireRate)
				{
					_stats.mana -= _stats.manaCost;
					if (AppMain.TYPEOFGAME.Equals("MULTIPLAYER"))
					{
						AppMain.client.SetActionMessage('S');
					}
					playerState = PlayerState.Shooting;
					
					ProjectileManager.Instance.Shoot(this);
					canShoot = false;
					shootTimer = 0.0f;
				}
			}
			
		}
	}
}

