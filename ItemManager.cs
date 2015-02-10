using System;
using System.Diagnostics;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace TheATeam
{
	public enum ItemType
	{
		flag,
		element,
		fire,
		water,
	}
	public class ItemManager
	{
		private static ItemManager instance = new ItemManager();

		private List<Item> items;
		//public List<Item> Items { get { return items;}}
		private Item leftFlag, rightFlag, fireElement, waterElement;
		private Scene scene;
		
		private static Vector2i flagIndex = Tile.LoadSpriteIndex('~');
		private static Vector2i fireIndex = Tile.LoadSpriteIndex('R');
		private static Vector2i waterIndex =  Tile.LoadSpriteIndex('B');
		
		private ItemManager ()
		{
			scene = GameSceneManager.currentScene;
			
			items = new List<Item>();
			if(items.Count == 0)
			{
				Vector2 pos1 = new Vector2(100,290);
				leftFlag = new Item(scene, pos1, flagIndex, ItemType.flag, "Player1Flag");
				items.Add(leftFlag);
			}
		}
		public void initFlags()
		{
			Vector2 pos1 = leftFlag.position;
			items.Remove(leftFlag);
			leftFlag = new Item(scene, pos1, flagIndex, ItemType.flag, "Player1Flag");
			items.Add(leftFlag);
			Vector2 pos2 = new Vector2(864,290);
			rightFlag = new Item(scene, pos2, flagIndex, ItemType.flag, "Player2Flag");
			items.Add(rightFlag);
		}
		public void initElements()
		{
			if(null == GetItem(ItemType.element, "Fire")) 
			{
			Vector2 pos1 = new Vector2(480,190);
			Vector2 pos2 = new Vector2(480,390);
			fireElement = new Item(scene, pos1, fireIndex, ItemType.element, "Fire");
			waterElement = new Item(scene, pos2, waterIndex, ItemType.element, "Water");
				
			items.Add(fireElement);
			items.Add(waterElement);
			}
		}
		public static ItemManager Instance
		{
			get{return instance;}
		}
		
		public void Update(float dt)
		{
//			redItem.Update(dt); //could be array
//			blueItem.Update(dt);
			foreach(Item item in items)
			{
				item.Update(dt);
			}
		}
		
		public void Grabbed()
		{
			//set the item/flag to follow player location... at least for now, drops when player dies
//			if (blueItem.collided == true)
//			{
//				blueItem.iSprite.Visible = false;
//			}
//	
//			if (redItem.collided == true)
//			{
//				redItem.iSprite.Visible = false;
//			}
			foreach(Item item in items)
			{
				if(item.collided == true)
					item.iSprite.Visible = false;
			}
			
		}
		
		public void ItemCollision(Player p1,Player p2)
		{
			Vector2 p1Size = new Vector2(p1.Quad.Bounds2().Point11.X, p1.Quad.Bounds2().Point11.Y);
			Vector2 p2Size = new Vector2(p2.Quad.Bounds2().Point11.X, p2.Quad.Bounds2().Point11.Y);
			// Will need to check this against every tile + player positions
			
			foreach(Item item in items)
			{
				if(!item.collided)
				{
					//check player 1 with items first
					if(item.hasCollided(p1.Position, p1Size))
					{
						Console.WriteLine("Collided with " + item.GetName());
						switch (item.GetName()) 
						{
						case "Water":
							p1.ChangeTiles("Water");
							break;
						case "Fire":
							p1.ChangeTiles("Fire");
							break;
						case "Player1Flag":
							break;
						case "Player2Flag":
							break;
						default:
							break;
						}
						item.iSprite.Visible = false;
						item.collided = true;
					}
				}
			}
			
//				if(blueItem.hasCollided(pos, size))
//					blueItem.collided = true;
//				if(redItem.hasCollided(pos, size))
//					redItem.collided = true;
			
			//Grabbed();
		}
		public Item GetItem(ItemType type, string name)
		{
			Item toReturn = null;
			foreach(Item item in items)
			{
				if(item.GetType() == type && item.GetName().CompareTo(name) == 0)
					toReturn = item;
			}
			return toReturn;
		}
		
	}
}



