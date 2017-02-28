using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[System.Serializable]
public class Pathfinder : MonoBehaviour {

	private enum Direction {Left, Up, Right};

	private enum Orientation {movingLeft, movingUp, movingRight};

	[SerializeField]
	private AssetManager a_AssetManager;

	[SerializeField]
	private Orientation a_Orientation;

	[SerializeField]
	private Boulder a_Boulder;

	[SerializeField]
	private BoulderSpawnManager a_BoulderSpawnManager;

	/// <summary>
	/// Array of all blocks in the maze
	/// </summary>
	private Block[][] mazeBlocks;

	/// <summary>
	/// Block that Pathfinder is currently within bounds of. Updated via trigger collider.
	/// </summary>
	[SerializeField]
	private Block currentBlock;

	/// <summary>
	/// Parent GameObject containing all blocks in main path
	/// </summary>
	[SerializeField]
	private Transform a_PathObject;

	[SerializeField]
	private Transform a_BlocksObject;

	/// <summary>
	/// List of all blocks contained in the main path (not including branching paths)
	/// </summary>
	[SerializeField]
	private List<Block> a_Path;
	public List<Block> getPath() {return a_Path;}
	public Block getBlock(int p_Index) {return a_Path [p_Index];}
	/// <summary>
	/// List of blocks that are corners in the path
	/// </summary>
	[SerializeField]
	private List<Block> a_Corners;

	[SerializeField]
	private int a_mazeHeight;
	public int getMazeHeight() {return a_mazeHeight;}

	[SerializeField]
	private int a_mazeWidth;
	public int getMazeWidth() {return a_mazeWidth;}

	[SerializeField]
	private bool startSelected;

	[SerializeField]
	private bool donePathfinding;

	[SerializeField]
	private bool doneBranching;

	/// <summary>
	/// States to keep track of Pathfinders permitted directions
	/// </summary>
	private bool canMoveUp;
	private bool canMoveLeft;
	private bool canMoveRight;


	/// <summary>
	/// Sets the size mazeBlocks array using the localScale of the MazeGenerator plane
	/// </summary>
	/// <param name="rowTotal">Row total.</param>
	/// <param name="colTotal">Col total.</param>
	public void setBlocks(int rowTotal, int colTotal)
	{
		mazeBlocks = new Block[rowTotal][];
		a_mazeHeight = rowTotal;
		a_mazeWidth = colTotal;

		for (int mBlocksIndex = 0; mBlocksIndex < mazeBlocks.Length; mBlocksIndex++) 
		{
			mazeBlocks[mBlocksIndex] = new Block[colTotal];
		}
	}

	/// <summary>
	/// Inserts the blocks into the mazeBlocks array
	/// </summary>
	/// <param name="p_Block">P block.</param>
	/// <param name="rowPos">Row position.</param>
	/// <param name="colPos">Col position.</param>
	public void insertBlock(Block p_Block, int rowPos, int colPos)
	{
		mazeBlocks[rowPos][colPos] = p_Block;
	}

	/// <summary>
	/// Randomly chooses a block from the 0th row to be start block
	/// </summary>
	public void pickStart()
	{
		/* 
		 * Randomly choose from which column the startBlock will be
		 * Row will always be 0
		 */
		int startBlockIndex = Random.Range (3, a_mazeWidth-3);
		Block startBlock = mazeBlocks [0] [startBlockIndex];

		// Highlight startBlock
		//startBlock.highlight();

		// Add startBlock to a_Path array of all blocks in the path
		a_Path.Add(startBlock);

		// Set the parent of the block to the Path gameObject
		startBlock.GetComponent<Transform>().SetParent(a_PathObject);

		// Debug Code ------
		Vector3 v_Position = mazeBlocks[0][startBlockIndex].gameObject.GetComponent<Transform>().position;
		transform.position = v_Position;
		// ----------
	}
		

	/// <summary>
	/// Moves Pathfinder Left, ensuring it doesn't go out of bounds
	/// </summary>
	private void moveLeft(int moveValue)
	{
	
		// If too close to left border of maze, return immediately
		if (currentBlock.getMazePositionY() <= 1) { return; }

		// Set states to remove backtracking during Path Creation
		canMoveUp = true;
		canMoveLeft = true;
		canMoveRight = false;


		/* 
		 * Loop to move Pathfinder in specified direction 
		*/
		for (int x = moveValue; x >= 0; x--) 
		{
			// If too close to left border of maze, break immediately
			if (currentBlock.getMazePositionY () <= 1) { break; }


			/* 
			 * IF ORIENTATION CHANGES - (From Up to Left) 
			 * BEFORE BEGINNING LEFTWARDS MOTION MAKE CURRENTBLOCK A CORNER
			 * 
			 */
			if (x == moveValue) 
			{
				switch (a_Orientation) 
				{
				// If movingUp at time Pathfinder starts headed left, currentBlock is TopLeft corner
				case Orientation.movingUp:
					currentBlock.a_CornerType = Block.CornerType.TopRight;
					a_Corners.Add(currentBlock);
					break;
				}
			}

			// Get coords for the neighbouring block left
			int x_Pos = currentBlock.getMazePositionX ();
			int y_Pos = currentBlock.getMazePositionY () - 1;
			Block leftNeighbour = mazeBlocks [x_Pos] [y_Pos];


			// Debug Code ------
			Vector3 v_Position = leftNeighbour.gameObject.GetComponent<Transform>().position;
			transform.position = v_Position;
			//leftNeighbour.highlight();
			// ----------

			// Set parent of leftNeighbour to Path gameObject
			leftNeighbour.GetComponent<Transform>().SetParent(a_PathObject);

			// Add leftNeighbour to a_Path array
			a_Path.Add(leftNeighbour);

			// Set currentBlock to left neighbour
			currentBlock = leftNeighbour;
		}

		// Set Pathfinder orientation to movingLeft-wards
		a_Orientation = Orientation.movingLeft;

	}

	private void moveUp(int moveValue)
	{
		// If at top of maze, set donePathfinding to true, return immediately
		if (currentBlock.getMazePositionX() == a_mazeHeight-1) 
		{
			donePathfinding = true;
			return; 
		}

		// Sets states to ensure upwards motion doesn't stack repetitively
		canMoveUp = false;
		canMoveLeft = true;
		canMoveRight = true;


		/* 
		 * Loop to move Pathfinder in specified direction 
		*/
		for (int x = moveValue; x >= 0; x--) 
		{
			// If at top of maze, return immediately
			if (currentBlock.getMazePositionX() == a_mazeHeight-1) { return; }

			/* 
			 * IF ORIENTATION CHANGES - (From Left to Up, or from Right to Up) 
			 * BEFORE BEGINNING UPWARDS MOTION MAKE CURRENTBLOCK A CORNER
			 * 
			 */
			if (x == moveValue) 
			{
				switch (a_Orientation) {

				// If movingLeft at time Pathfinder starts headed upwards, currentBlock is BottomLeft corner
				case Orientation.movingLeft:
					currentBlock.a_CornerType = Block.CornerType.BottomLeft;
					a_Corners.Add(currentBlock);
					break;

				// If movingRight at time Pathfinder starts headed upwards, currentBlock is BottomRight corner
				case Orientation.movingRight:
					currentBlock.a_CornerType = Block.CornerType.BottomRight;
					a_Corners.Add(currentBlock);
					break;
				}
			}


			// Get coords for the neighbouring block above
			int x_Pos = currentBlock.getMazePositionX() + 1;
			int y_Pos = currentBlock.getMazePositionY();
			Block upNeighbour = mazeBlocks [x_Pos][y_Pos];


			// Debug Code ------
			Vector3 v_Position = upNeighbour.gameObject.GetComponent<Transform>().position;
			transform.position = v_Position;
			//upNeighbour.highlight();
			// ----------

			// Set upNeighbour parent to Path gameObject
			upNeighbour.GetComponent<Transform>().SetParent(a_PathObject);

			// Add upwards Neighbour to final path
			a_Path.Add(upNeighbour);

			// Set currentBlock to upwardsNeighbour
			currentBlock = upNeighbour;
		}

		// Set Pathfinder orientation to movingUp-wards
		a_Orientation = Orientation.movingUp;
			
	}

	private void moveRight(int moveValue)
	{
		// If too close to right border of maze, return immediately
		if (currentBlock.getMazePositionY() >= a_mazeWidth-2) { return; }

		// Set states to remove backtracking during Path Creation
		canMoveUp = true;
		canMoveLeft = false;
		canMoveRight = true;


		/* 
		 * Loop to move Pathfinder in specified direction 
		*/
		for (int x = moveValue; x >= 0; x--) 
		{
			// If too close to right border of maze, break immediately
			if (currentBlock.getMazePositionY() >= a_mazeWidth-2) { break; }

			/* 
			 * IF ORIENTATION CHANGES - (From Up to Right) 
			 * BEFORE BEGINNING RIGHTWARDS MOTION, MAKE CURRENTBLOCK A CORNER
			 * 
			 */
			if (x == moveValue) 
			{
				switch (a_Orientation) 
				{
				// If movingUp at time Pathfinder starts headed left, currentBlock is TopLeft corner
				case Orientation.movingUp:
					currentBlock.a_CornerType = Block.CornerType.TopLeft;
					a_Corners.Add(currentBlock);
					break;
				}
			}

			// Get coords for the neighbouring block right
			int x_Pos = currentBlock.getMazePositionX();
			int y_Pos = currentBlock.getMazePositionY() + 1;
			Block rightNeighbour = mazeBlocks [x_Pos][y_Pos];


			// Debug Code ------
			Vector3 v_Position = rightNeighbour.gameObject.GetComponent<Transform>().position;
			transform.position = v_Position;
			//rightNeighbour.highlight();
			// ----------

			// Set rightNeighbour parent to Path gameObject
			rightNeighbour.GetComponent<Transform>().SetParent(a_PathObject);

			// Add rightwards Neighbour to final path
			a_Path.Add(rightNeighbour);

			// Set currentBlock to rightNeighbour
			currentBlock = rightNeighbour;
		}

		// Set Pathfinder orientation to movingRight-wards
		a_Orientation = Orientation.movingRight;
	}


	/// <summary>
	/// Move the Pathfinder to the next block(s) in the maze to create a path
	/// </summary>
	private void move()
	{

		// If at top of maze, return immediately
		if (currentBlock.getMazePositionX () == a_mazeHeight - 1) 
		{
			donePathfinding = true;
			return;
		}

		// 0 - Left, 1 - Up, 2 - Right
		// Randomly choose an integer which decides the direction the Pathfinder will move next
		Direction moveDirection = (Direction)Random.Range (0, 3);

		// Can move between 1 to 3 (inclusive) steps in any direction
		int moveAmount;// = Random.Range (2, 4);


		switch (moveDirection) 
		{
		case Direction.Left:
			if (!canMoveLeft) {break;}
			moveAmount = Random.Range (2,9);
			moveLeft (moveAmount);
			break;
		case Direction.Up:
			if (!canMoveUp) { break; }
			moveAmount = Random.Range (4, 7);
			//moveAmount = 3;
			moveUp (moveAmount);
			break;
		case Direction.Right:
			if (!canMoveRight) {break;}
			moveAmount = Random.Range (2, 9);
			moveRight (moveAmount);
			break;
		}
	}

	private void createBranches()
	{
		
		int branchCount = 3;

		// Debug Code -----
		Vector3 v_Position;

		// Loop for every Block in the path that is also a corner
		foreach (Block cornerBlock in a_Corners) 
		{
			// If branchCount quota is reached, break out of loop
			if (branchCount == 0) { break; }

			// Continue past any corners on the 0th rows
			if (cornerBlock.getMazePositionX() == 0) { continue; }

			// Check if it is possible to branch off to the side
			if (cornerBlock.checkBranchLateral(a_mazeWidth) ) 
			{  
				v_Position = cornerBlock.GetComponent<Transform>().position;
				transform.position = v_Position;
				currentBlock = cornerBlock;

				branchLateral(cornerBlock);

				// Decrement number of branches remaining to create
				branchCount--;

				// Highlight the branch
				//cornerBlock.highlightRed();

				// Move on to next corner
				continue;
			}

			// If side branch not possible, check if vertical branch is possible
			else if (cornerBlock.checkBranchVertical(a_mazeHeight))
			{
				v_Position = cornerBlock.GetComponent<Transform>().position;
				transform.position = v_Position;
				currentBlock = cornerBlock;

				branchVertical(cornerBlock);

				// Decrement number of branches remaining to create
				branchCount--;

				// Highlight the branch
				//cornerBlock.highlightRed();

				// Move on to next corner
				continue;
			}

		}

		int lastIndex = a_Path.Count;

		doneBranching = true;

		a_BoulderSpawnManager.setTriggerLocation (a_Path);
		a_AssetManager.placeDoor(a_Path[lastIndex-1]);

		hidePathObject();

		gameObject.SetActive (false);
	}

	private void branchLateral (Block p_Block)
	{
		Block.CornerType v_CornerType = p_Block.a_CornerType;

		switch (v_CornerType) 
		{
		case Block.CornerType.BottomLeft:
		case Block.CornerType.TopLeft:
			branchLeft (Random.Range (5, 10), p_Block);
			break;

		case Block.CornerType.BottomRight:
		case Block.CornerType.TopRight:
			branchRight (Random.Range (5,10), p_Block);
			break;
		}
	}

	private void branchVertical (Block p_Block)
	{
		Block.CornerType v_CornerType = p_Block.a_CornerType;

		switch (v_CornerType) 
		{
		case Block.CornerType.BottomLeft:
		case Block.CornerType.BottomRight:
			branchDown(3, p_Block);
			break;

		case Block.CornerType.TopLeft:
		case Block.CornerType.TopRight:
			branchUp(3, p_Block);
			break;
		}
	}

	private void branchLeft (int branchValue, Block cornerBlock)
	{
		int x_Pos;
		int y_Pos;
		Vector3 v_Position;
		cornerBlock.a_BranchPath = new Block[branchValue];

		/* 
		 * Loop to branch in specified direction 
		*/
		for (int x = branchValue; x > 0; x--) 
		{

			// Get coords for the neighbouring block left
			x_Pos = currentBlock.getMazePositionX ();
			y_Pos = currentBlock.getMazePositionY () - 1;
			Block leftBranchNeighbour = mazeBlocks [x_Pos] [y_Pos];


			// Debug Code ------
			v_Position = leftBranchNeighbour.gameObject.GetComponent<Transform>().position;
			transform.position = v_Position;
			//leftBranchNeighbour.highlightPurple();
			// ----------

			// Set parent of leftBranchNeighbour to Path gameObject
			leftBranchNeighbour.GetComponent<Transform>().SetParent(a_PathObject);

			// Add leftBranchNeighbour to cornerBlock a_BranchPath array
			cornerBlock.a_BranchPath[(branchValue-x)] = leftBranchNeighbour;

			// Set currentBlock to left neighbour
			currentBlock = leftBranchNeighbour;
		}

		int lastIndex = cornerBlock.a_BranchPath.Length-1;
		a_AssetManager.placeKey(cornerBlock.a_BranchPath[lastIndex], -90f);
	}

	private void branchRight(int branchValue, Block cornerBlock)
	{
		int x_Pos;
		int y_Pos;
		Vector3 v_Position;
		cornerBlock.a_BranchPath = new Block[branchValue];

		/* 
		 * Loop to branch in specified direction 
		*/
		for (int x = branchValue; x > 0; x--) 
		{

			// Get coords for the neighbouring block right
			x_Pos = currentBlock.getMazePositionX ();
			y_Pos = currentBlock.getMazePositionY () + 1;
			Block rightBranchNeighbour = mazeBlocks [x_Pos] [y_Pos];


			// Debug Code ------
			v_Position = rightBranchNeighbour.gameObject.GetComponent<Transform>().position;
			transform.position = v_Position;
			//rightBranchNeighbour.highlightPurple();
			// ----------

			// Set parent of rightBranchNeighbour to Path gameObject
			rightBranchNeighbour.GetComponent<Transform>().SetParent(a_PathObject);

			// Add rightBranchNeighbour to a_Path array
			cornerBlock.a_BranchPath[(branchValue-x)] = rightBranchNeighbour;

			// Set currentBlock to right neighbour
			currentBlock = rightBranchNeighbour;
		}

		int lastIndex = cornerBlock.a_BranchPath.Length-1;
		a_AssetManager.placeKey(cornerBlock.a_BranchPath[lastIndex], 90);
	}

	private void branchUp(int branchValue, Block cornerBlock)
	{
		int x_Pos;
		int y_Pos;
		Vector3 v_Position;
		cornerBlock.a_BranchPath = new Block[branchValue];

		/* 
		 * Loop to branch in specified direction 
		*/
		for (int x = branchValue; x > 0; x--) 
		{

			// Get coords for the neighbouring block up
			x_Pos = currentBlock.getMazePositionX ()+1;
			y_Pos = currentBlock.getMazePositionY ();
			Block upBranchNeighbour = mazeBlocks [x_Pos] [y_Pos];


			// Debug Code ------
			v_Position = upBranchNeighbour.gameObject.GetComponent<Transform>().position;
			transform.position = v_Position;
			//upBranchNeighbour.highlightPurple();
			// ----------

			// Set parent of upBranchNeighbour to Path gameObject
			upBranchNeighbour.GetComponent<Transform>().SetParent(a_PathObject);

			// Add leftNeighbour to a_Path array
			cornerBlock.a_BranchPath[(branchValue-x)] = upBranchNeighbour;

			// Set currentBlock to up neighbour
			currentBlock = upBranchNeighbour;
		}

		int lastIndex = cornerBlock.a_BranchPath.Length-1;
		a_AssetManager.placeKey(cornerBlock.a_BranchPath[lastIndex], 0);
	}

	private void branchDown(int branchValue, Block cornerBlock)
	{
		int x_Pos;
		int y_Pos;
		Vector3 v_Position;
		cornerBlock.a_BranchPath = new Block[branchValue];

		/* 
		 * Loop to branch in specified direction 
		*/
		for (int x = branchValue; x > 0; x--) 
		{

			// Get coords for the neighbouring block down
			x_Pos = currentBlock.getMazePositionX () - 1;
			y_Pos = currentBlock.getMazePositionY ();
			Block downBranchNeighbour = mazeBlocks [x_Pos] [y_Pos];


			// Debug Code ------
			v_Position = downBranchNeighbour.gameObject.GetComponent<Transform>().position;
			transform.position = v_Position;
			//downBranchNeighbour.highlightPurple();
			// ----------

			// Set parent of downBranchNeighbour to Path gameObject
			downBranchNeighbour.GetComponent<Transform>().SetParent(a_PathObject);

			// Add leftNeighbour to a_Path array
			cornerBlock.a_BranchPath[(branchValue-x)] = downBranchNeighbour;

			// Set currentBlock to down neighbour
			currentBlock = downBranchNeighbour;
		}

		int lastIndex = cornerBlock.a_BranchPath.Length-1;
		a_AssetManager.placeKey(cornerBlock.a_BranchPath[lastIndex], 180);
	}

	private void hidePathObject()
	{
		foreach (Transform child in a_PathObject) 
		{
			child.GetComponent<MeshRenderer> ().enabled = false;
			child.GetComponent<Transform> ().tag = "Path";
		}

		foreach (Transform child in a_BlocksObject) 
		{
			child.GetComponent<BoxCollider> ().isTrigger = false;
		}


	}
		

	private void OnTriggerEnter(Collider other)
	{

		if (other.CompareTag ("Block")) 
		{
			currentBlock = other.GetComponent<Block>();
			startSelected = true;
		}
	}

	// Use this for initialization
	void Start () {

		a_Path = new List<Block>();

		canMoveUp = true;
		canMoveLeft = true;
		canMoveRight = true;

		startSelected = false;

		donePathfinding = false;
	}
	
	// Update is called once per frame
	void Update () {


		if (!donePathfinding && startSelected) 
		{
			move();
		}

		if (donePathfinding && !doneBranching) 
		{
			createBranches();
		}
	}
}
