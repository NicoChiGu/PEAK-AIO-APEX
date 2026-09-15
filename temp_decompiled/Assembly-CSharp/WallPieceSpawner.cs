using System;
using UnityEngine;

// Token: 0x0200036D RID: 877
public class WallPieceSpawner : MonoBehaviour
{
	// Token: 0x06001651 RID: 5713 RVA: 0x000739EC File Offset: 0x00071BEC
	private void Go()
	{
		this.wall = base.GetComponent<Wall>();
		this.wall.WallInit();
		this.root = base.transform.Find("Pieces");
		this.Clear();
		for (int i = 0; i < 50; i++)
		{
			this.DoSpawns();
		}
	}

	// Token: 0x06001652 RID: 5714 RVA: 0x00073A40 File Offset: 0x00071C40
	private void DoSpawns()
	{
		for (int i = 0; i < this.wall.gridSize.x; i++)
		{
			for (int j = 0; j < this.wall.gridSize.y; j++)
			{
				WallPiece randomPiece = this.GetRandomPiece();
				if (this.wall.PieceFits(randomPiece, i, j))
				{
					this.SpawnPiece(randomPiece, i, j);
				}
			}
		}
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x00073AA4 File Offset: 0x00071CA4
	private void SpawnPiece(WallPiece piece, int x, int y)
	{
		WallPiece component = HelperFunctions.SpawnPrefab(piece.gameObject, this.wall.GetGridPos(x, y), Quaternion.identity, this.root).GetComponent<WallPiece>();
		component.wallPosition = new Vector2Int(x, y);
		this.wall.AddPiece(component);
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x00073AF3 File Offset: 0x00071CF3
	private WallPiece GetRandomPiece()
	{
		return this.pieces[Random.Range(0, this.pieces.Length)];
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x00073B0C File Offset: 0x00071D0C
	private void Clear()
	{
		this.root = base.transform.Find("Pieces");
		for (int i = this.root.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(this.root.GetChild(i).gameObject);
		}
	}

	// Token: 0x04001535 RID: 5429
	public WallPiece[] pieces;

	// Token: 0x04001536 RID: 5430
	private Transform root;

	// Token: 0x04001537 RID: 5431
	private Wall wall;
}
