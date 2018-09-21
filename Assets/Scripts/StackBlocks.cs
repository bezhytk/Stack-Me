using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StackBlocks : MonoBehaviour 
{
	public Text scoreText;
	public Color32[] gameColors = new Color32[4];
	public Material blockMaterials;
	public GameObject endScreen;
	
	private const float BOUNDS_SIZE = 3.5f;
	private const float BLOCK_MOVING_SPEED = 5.0f;
	private const float ERROR_MARGIN = 0.1f;
	private const float STACK_BOUNDS_GAIN = 0.25f;
	private const int COMBO_START_GAIN = 3;


	private GameObject[] stackBlock;
	private Vector2 stackBounds = new Vector2 (BOUNDS_SIZE, BOUNDS_SIZE);

	private int blockIndex;
	private int scoreCount = 0;
	private int combo = 0;

	private float blockTransition = 0.0f;
	private float moveSpeed = 2.5f;
	private float placedPosition;

	private bool isMovingOnX = true;
	private bool isGameOver = false;

	private Vector3 desiredPosition;
	private Vector3 lastBlockPosition;
	
	private void Start () 
	{
		stackBlock = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++)
		{
			stackBlock [i] = transform.GetChild (i).gameObject;
			ColorMesh(stackBlock[i].GetComponent<MeshFilter>().mesh);
		}

		blockIndex = transform.childCount - 1;
	}

	private void DropWreck(Vector3 pos,Vector3 scale)
	{
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.AddComponent<Rigidbody> ();

		go.GetComponent<MeshRenderer> ().material = blockMaterials;
		ColorMesh(go.GetComponent<MeshFilter> ().mesh);
	}

	private void Update () 
	{
		if (isGameOver)
			return;

		if (Input.GetMouseButtonDown (0))
		{
			if (PlaceBlock ())
			{
				SpawnBlock ();
				scoreCount++;
				scoreText.text = scoreCount.ToString ();
			}
			else
			{
				EndGame ();
			}
		}

		MoveBlock ();
		//Move the block
		transform.position = Vector3.Lerp(transform.position,desiredPosition,BLOCK_MOVING_SPEED * Time.deltaTime);		
	}

	private void MoveBlock ()
	{
		blockTransition += Time.deltaTime * moveSpeed;
		if(isMovingOnX)
			stackBlock[blockIndex].transform.localPosition = new Vector3 (Mathf.Sin(blockTransition) * BOUNDS_SIZE,scoreCount,placedPosition);
		else
			stackBlock[blockIndex].transform.localPosition = new Vector3 (placedPosition,scoreCount,Mathf.Sin(blockTransition) * BOUNDS_SIZE);
	}

	private void SpawnBlock ()
	{
		lastBlockPosition = stackBlock [blockIndex].transform.localPosition;
		blockIndex--;
		if(blockIndex < 0)
			blockIndex = transform.childCount - 1;
		
		desiredPosition = (Vector3.down) * scoreCount;
		stackBlock[blockIndex].transform.localPosition = new Vector3 (0, scoreCount, 0);
		stackBlock[blockIndex].transform.localScale = new Vector3(stackBounds.x,1,stackBounds.y);
	
		ColorMesh(stackBlock [blockIndex].GetComponent<MeshFilter> ().mesh);
	}

	private bool PlaceBlock ()
	{
		Transform t = stackBlock [blockIndex].transform;

		if (isMovingOnX)
		{
			float deltaX = lastBlockPosition.x - t.position.x;
			if (Mathf.Abs (deltaX) > ERROR_MARGIN)
			{
				//Cut the block
				combo = 0;
				stackBounds.x -= Mathf.Abs (deltaX);
				if(stackBounds.x <= 0)
					return false;

				float middle = lastBlockPosition.x + t.localPosition.x / 2;
				t.localScale = new Vector3(stackBounds.x,1,stackBounds.y);
				DropWreck
				(
					new Vector3 (t.position.x
						, t.position.y
						, (t.position.x > 0)
						? t.position.x + (t.localScale.x / 2)
						: t.position.x - (t.localScale.x / 2)),
					new Vector3 (Mathf.Abs(deltaX),1,t.localScale.z)	
				);
				t.localPosition = new Vector3(middle - (lastBlockPosition.x /2), scoreCount, lastBlockPosition.z);
			}
			else
			{
				if (combo > COMBO_START_GAIN)
				{
					stackBounds.x += STACK_BOUNDS_GAIN;
					if (stackBounds.x > BOUNDS_SIZE)
						stackBounds.x = BOUNDS_SIZE;
					float middle = lastBlockPosition.x + t.localPosition.x / 2;
					t.localScale = new Vector3(stackBounds.x,1,stackBounds.y);
					t.localPosition = new Vector3(middle - (lastBlockPosition.x /2), scoreCount, lastBlockPosition.z);
				}
				combo++;
				t.localPosition = new Vector3 (lastBlockPosition.x, scoreCount, lastBlockPosition.z);
			}
		}
		else
		{
			float deltaZ = lastBlockPosition.z - t.position.z;
			if (Mathf.Abs (deltaZ) > ERROR_MARGIN)
			{
				//Cut the block
				combo = 0;
				stackBounds.y -= Mathf.Abs (deltaZ);
				if(stackBounds.y <= 0)
					return false;

				float middle = lastBlockPosition.z + t.localPosition.z / 2;
				t.localScale = new Vector3(stackBounds.x,1,stackBounds.y);
				DropWreck
					(
						new Vector3 (t.position.x
							, t.position.y
							, (t.position.z > 0)
							? t.position.z + (t.localScale.z / 2)
							: t.position.z - (t.localScale.z / 2)),
						new Vector3 (t.localScale.x, 1, Mathf.Abs(deltaZ))	
					);
				t.localPosition = new Vector3(lastBlockPosition.x, scoreCount,middle - (lastBlockPosition.z / 2));
			}
			else
			{
				if (combo > COMBO_START_GAIN)
				{
					stackBounds.y += STACK_BOUNDS_GAIN;
					if (stackBounds.y > BOUNDS_SIZE)
						stackBounds.y = BOUNDS_SIZE;
					float middle = lastBlockPosition.z + t.localPosition.z / 2;
					t.localScale = new Vector3(stackBounds.x,1,stackBounds.y);
					t.localPosition = new Vector3(lastBlockPosition.x, scoreCount,middle - (lastBlockPosition.z / 2));
				}
				combo++;
				t.localPosition = new Vector3 (lastBlockPosition.x, scoreCount, lastBlockPosition.z);
			}
		}

		placedPosition = (isMovingOnX)
			? t.localPosition.x
			: t.localPosition.z;		
		isMovingOnX = !isMovingOnX;

		return true;
	}

	private void ColorMesh(Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];
		float f = Mathf.Sin(scoreCount * 0.25f);

		for(int i = 0; i < vertices.Length; i++)
			colors[i] = Lerp4(gameColors[0],gameColors[1],gameColors[2],gameColors[3],f);

		mesh.colors32 = colors;
	}

	private Color32 Lerp4(Color32 a,Color32 b,Color32 c,Color32 d,float t)
	{
		if (t < 0.33f)
			return Color.Lerp (a, b, t / 0.33f);
		else if (t < 0.66f)
			return Color.Lerp (b, c, (t - 0.33f) / 0.33f);
		else
			return Color.Lerp (c, d, (t - 0.66f) / 0.66f);
	}

	private void EndGame ()
	{
		if (PlayerPrefs.GetInt ("score") < scoreCount)
			PlayerPrefs.SetInt ("score", scoreCount);
		isGameOver = true;
		endScreen.SetActive (true);
		stackBlock[blockIndex].AddComponent<Rigidbody> ();
	}

	public void OnButtonClick(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}
}
