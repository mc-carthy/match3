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
	private Vector2 coords;
	private bool isDestroyed;

	private void Start () {
		index = Random.Range (0, colors.Length);
		ren = GetComponent<Renderer> ();
		ren.material.SetColor ("_Color", colors [index]);
	}
}