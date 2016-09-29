using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	private float ySpawnPos = 6;
	private float newPieceDelay = 0.15f;

	private float selectedPieceScaleFactor = 1.2f;
	private float selectedPieceScaleTime = 0.3f;
	private float swapPieceTime = 0.5f;
	private float checkGameOverTime = 1;
	private float destroyPieceTime = 0.25f;

	private void Start () {
		mainCam = Camera.main;
		BuildBoard ();
	}

	private void Update () {
		GetInput ();
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

	private void GetInput () {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = mainCam.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit, 100)) {
				if (hit.collider.tag == "piece") {
					Piece hitPiece = hit.collider.gameObject.GetComponent<Piece>();

					if (selectedPiece == null) {
						selectedPiece = hitPiece;
						iTween.ScaleTo (selectedPiece.gameObject, iTween.Hash (
							"scale", Vector3.one * selectedPieceScaleFactor,
							"isLocal", true,
							"time", selectedPieceScaleTime
						));
					} else {
						if (hitPiece == selectedPiece || hitPiece.IsNeighbour (selectedPiece) == false) {
							iTween.ScaleTo (selectedPiece.gameObject, iTween.Hash (
								"scale", Vector3.one,
								"isLocal", true,
								"time", selectedPieceScaleTime
							));
						} else if (hitPiece.IsNeighbour (selectedPiece)) {
							AttemptMatch (selectedPiece, hitPiece);
						}

						selectedPiece = null;
					}
				}
			}
		}
	}

	private void AttemptMatch (Piece piece0, Piece piece1) {
		StartCoroutine(AttemptMatchRoutine (piece0, piece1));
	}

	private IEnumerator AttemptMatchRoutine (Piece piece0, Piece piece1) {
		iTween.Stop (piece0.gameObject);
		iTween.Stop (piece1.gameObject);

		piece0.transform.localScale = Vector3.one;
		piece1.transform.localScale = Vector3.one;

		Vector2 coords0 = piece0.coords;
		Vector2 coords1 = piece1.coords;

		Vector3 pos0 = piece0.transform.position;
		Vector3 pos1 = piece1.transform.position;

		iTween.MoveTo (piece0.gameObject, iTween.Hash (
			"position", pos1,
			"isLocal", true,
			"time", swapPieceTime
		));		
		iTween.MoveTo (piece1.gameObject, iTween.Hash (
			"position", pos0,
			"isLocal", true,
			"time", swapPieceTime
		));

		piece0.coords = coords1;
		piece1.coords = coords0;

		board [(int)piece0.coords.x, (int)piece0.coords.y] = piece0;
		board [(int)piece1.coords.x, (int)piece1.coords.y] = piece1;

		yield return new WaitForSeconds (swapPieceTime);

		List<Piece> matchingPieces = CheckMatch (piece0);
		if (matchingPieces.Count == 0) {
			matchingPieces = CheckMatch (piece1);
		}

		if (matchingPieces.Count < 3) {
			iTween.MoveTo (piece0.gameObject, iTween.Hash (
				"position", pos0,
				"isLocal", true,
				"time", swapPieceTime
			));		
			iTween.MoveTo (piece1.gameObject, iTween.Hash (
				"position", pos1,
				"isLocal", true,
				"time", swapPieceTime
			));

			piece0.coords = coords0;
			piece1.coords = coords1;

			board [(int)piece0.coords.x, (int)piece0.coords.y] = piece0;
			board [(int)piece1.coords.x, (int)piece1.coords.y] = piece1;

			yield return new WaitForSeconds (checkGameOverTime);

			CheckGameOver ();

		} else {
			foreach (Piece piece in matchingPieces) {
				piece.isDestroyed = true;

				score += 100;

				iTween.ScaleTo (piece.gameObject, iTween.Hash (
					"scale", Vector3.zero,
					"isLocal", true,
					"time", destroyPieceTime
				));
			}
			return new WaitForSeconds (destroyPieceTime);

			DropPieces ();
			AddPieces ();

			yield return new WaitForSeconds (checkGameOverTime);

			CheckGameOver ();
		}
	}

	private void CheckMatch (Piece piece) {

	}

	private void CheckGameOver () {

	}

	private void DropPieces () {
		for (int y = 0; y < boardHeight; y++) {
			for (int x = 0; x < boardWidth; x++) {
				if (board [x, y].isDestroyed) {
					bool isDropped = false;

					for (int j = y + 1; j < boardHeight && !isDropped; j++) {
						Vector2 coord0 = board [x, y].coords;
						Vector2 coord1 = board [x, j].coords;

						board [x, y].coords = coord1;
						board [x, j].coords = coord0;

						iTween.MoveTo (board [x, j].gameObject, iTween.Hash (
							"position", board[x, y].transform.position,
							"isLocal", true,
							"time", swapPieceTime
						));

						board [x, y].transform.localPosition = board [x, j].transform.localPosition;

						Piece fallingPiece = board [x, j];
						board [x, j] = board [x, y];
						board [x, y] = fallingPiece;

						isDropped = true;
					}
				}
			}
		}
	}

	private void AddPieces () {
		int firstY = -1;

		for (int y = 0; y < boardHeight; y++) {
			for (int x = 0; x < boardWidth; x++) {
				if (board [x, y].isDestroyed) {

					if (firstY = -1) {
						firstY = y;

						Piece oldPiece = board [x, y];
						GameObject pieceObject = Instantiate (piecePrefab);
						pieceObject.transform.SetParent (levelContainer);
						pieceObject.transform.localPosition = new Vector3 (
							oldPiece.transform.position.x,
							ySpawnPos,
							0
						);

						iTween.MoveTo (pieceObject, iTween.Hash (
							"position", oldPiece.transform.localPosition,
							"isLocal", true,
							"time", swapPieceTime,
							"delay", newPieceDelay * (y - firstY)
						));

						Piece piece = pieceObject.GetComponent<Piece> ();
						piece.coords = oldPiece.coords;
						board [x, y] = piece;

						Destroy (oldPiece.gameObject);
					}

				}
			}
		}
	}
}