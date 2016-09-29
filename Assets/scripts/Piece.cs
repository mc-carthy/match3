using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {

	private Renderer ren;
	private Color[] colors = new Color[6] {
		Color.red,
		Color.blue,
		Color.green,
		Color.white,
		Color.yellow,
		Color.magenta
	};
	private int index;

	public bool isDestroyed;
	public Vector2 coords;

	private void Start () {
		index = Random.Range (0, colors.Length);
		ren = GetComponent<Renderer> ();
		ren.material.SetColor ("_Color", colors [index]);
	}

	public bool IsNeighbour (Piece otherPiece) {
		int hDist = Mathf.Abs((int)(coords.x - otherPiece.coords.x));
		int vDist = Mathf.Abs((int)(coords.y - otherPiece.coords.y));
		int totalDist = hDist + vDist;
		return (totalDist == 1);
	}
}