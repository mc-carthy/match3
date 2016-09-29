using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	[SerializeField]
	private Transform levelContainer;
	[SerializeField]
	private GameObject piecePrefab;
	private Camera mainCam;
	private int boardWidth = 6;
	private int boardHeight = 5;
	private float pieceSpacing = 1.5f;
	private int score;
	private float gameTimer;
	private bool isGameOver;
	private Piece[,] board;
	private Piece selectedPiece;

	private void Start () {
		mainCam = Camera.main;
		BuildBoard ();
	}

	private void BuildBoard () {
		board = new Piece[boardWidth, boardHeight];

		for (int y = 0; y < boardHeight; y++) {
			for (int x = 0; x < boardWidth; x++) {
				GameObject newPiece = Instantiate (piecePrefab);
				newPiece.transform.SetParent (levelContainer);
				newPiece.transform.localPosition = new Vector3 (
					(-boardWidth * pieceSpacing) / 2f + (pieceSpacing / 2f) + (x * pieceSpacing),
					(-boardHeight * pieceSpacing) / 2f + (pieceSpacing / 2f) + (y * pieceSpacing),
					0
				);

				Piece piece = newPiece.GetComponent<Piece> ();
				piece.coords = new Vector2 (x, y);

				board [x, y] = piece;
			}
		}
	}
}