using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200036A RID: 874
public class Wall : MonoBehaviour
{
	// Token: 0x0600163B RID: 5691 RVA: 0x0007352F File Offset: 0x0007172F
	internal void WallInit()
	{
		this.pieces = new List<WallPiece>();
	}

	// Token: 0x0600163C RID: 5692 RVA: 0x0007353C File Offset: 0x0007173C
	internal void AddPiece(WallPiece piece)
	{
		this.pieces.Add(piece);
	}

	// Token: 0x0600163D RID: 5693 RVA: 0x0007354C File Offset: 0x0007174C
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
		for (int i = 0; i < this.gridSize.x; i++)
		{
			for (int j = 0; j < this.gridSize.y; j++)
			{
				Gizmos.DrawWireCube(this.GetGridPos(i, j), new Vector3(this.gridCellSize, this.gridCellSize, 0.25f));
			}
		}
	}

	// Token: 0x0600163E RID: 5694 RVA: 0x000735C8 File Offset: 0x000717C8
	internal Vector3 GetGridPos(int x, int y)
	{
		Vector2 vector = (this.gridSize - Vector2.one) * this.gridCellSize;
		Vector2 vector2 = base.transform.position - vector * 0.5f;
		Vector2 vector3 = base.transform.position + vector * 0.5f;
		float num = (float)x / ((float)this.gridSize.x - 1f);
		float num2 = (float)y / ((float)this.gridSize.y - 1f);
		return new Vector3(Mathf.Lerp(vector2.x, vector3.x, num), Mathf.Lerp(vector2.y, vector3.y, num2), base.transform.position.z);
	}

	// Token: 0x0600163F RID: 5695 RVA: 0x000736A0 File Offset: 0x000718A0
	internal Vector3 SnapToPosition(Vector3 position)
	{
		Vector2 vector = (this.gridSize - Vector2.one) * this.gridCellSize;
		Vector2 vector2 = base.transform.position - vector * 0.5f;
		Vector2 vector3 = base.transform.position + vector * 0.5f;
		float num = Mathf.InverseLerp(vector2.x, vector3.x, position.x);
		float num2 = Mathf.InverseLerp(vector2.y, vector3.y, position.y);
		int num3 = Mathf.RoundToInt(num * ((float)this.gridSize.x - 1f));
		int num4 = Mathf.RoundToInt(num2 * ((float)this.gridSize.y - 1f));
		return this.GetGridPos(num3, num4);
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x0007377C File Offset: 0x0007197C
	internal bool PieceFits(WallPiece piece, int x, int y)
	{
		foreach (WallPiece wallPiece in this.pieces)
		{
			if (this.CollisionCheck(piece, x, y, wallPiece))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001641 RID: 5697 RVA: 0x000737DC File Offset: 0x000719DC
	private bool CollisionCheck(WallPiece newPiece, int newPosX, int newPosY, WallPiece existing)
	{
		for (int i = 0; i < newPiece.dimention.x; i++)
		{
			for (int j = 0; j < newPiece.dimention.y; j++)
			{
				Vector2Int vector2Int;
				vector2Int..ctor(newPosX + i, newPosY + j);
				if (this.CollisionCheckSpot(vector2Int, existing))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001642 RID: 5698 RVA: 0x00073830 File Offset: 0x00071A30
	private bool CollisionCheckSpot(Vector2Int checkPos, WallPiece existing)
	{
		for (int i = 0; i < existing.dimention.x; i++)
		{
			for (int j = 0; j < existing.dimention.y; j++)
			{
				if (new Vector2Int(existing.wallPosition.x + i, existing.wallPosition.y + j) == checkPos)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0400152B RID: 5419
	public Vector2Int gridSize;

	// Token: 0x0400152C RID: 5420
	public float gridCellSize;

	// Token: 0x0400152D RID: 5421
	public List<WallPiece> pieces = new List<WallPiece>();
}
