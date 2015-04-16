using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class CreateGraphScript : MonoBehaviour
{

		PlanarGraph pg;
		private Color c = Color.red;
		private int lengthOfLineRenderer = 2;
		private int numMines;
		private int graphWidth;
		private int graphHeight;
		private float clustering;
		private float connectedness;
		private float minePct;

		// Use this for initialization
		void Start ()
		{
				numMines = 25;
				graphWidth = 30;
				graphHeight = 20;
				clustering = 0.5f;
				connectedness = 0.5f;
				minePct = 0.5f;
	
				pg = new PlanarGraph (numMines, graphWidth, graphHeight, clustering, connectedness, minePct);
		}

		void OnGUI ()
		{
				GUI.contentColor = Color.black;
				foreach (Node n in pg.GetNodes()) {
						Vector3 screenPoint = Camera.main.WorldToScreenPoint (n.position);
						GUI.Label (new Rect (screenPoint.x, Screen.height - screenPoint.y, 100, 20), n.id.ToString ());
				}
	}

		public void RevealNode ()
		{
				InputField input = (InputField)GameObject.Find ("RevealInput").GetComponent<InputField> ();
				int nodeToReveal = int.Parse (input.text);
				Node revealNode = (Node)pg.Nodes [nodeToReveal];
				
				revealNode.IsRevealed = true;
				pg.Nodes.Remove (revealNode.id);
				pg.Nodes.Add (revealNode.id, revealNode);

				if (!revealNode.HasMine) {
						RevealNeighbors (revealNode);
				}

				DrawGraph ();
		}

		public void RevealNeighbors(Node neighbor) {
			foreach (Node n in neighbor.Neighbors) {
				Node revealNeighbor = (Node)pg.Nodes [n.id];
				if (!revealNeighbor.HasMine && !revealNeighbor.IsRevealed) {
					revealNeighbor.IsRevealed = true;
					pg.Nodes.Remove (revealNeighbor.id);
					pg.Nodes.Add (revealNeighbor.id, revealNeighbor);
					RevealNeighbors(n);
				}	
			}
		}

		public void SetNumMines ()
		{
				Slider slider = (Slider)GameObject.Find ("MinesSlider").GetComponent<Slider> ();
				numMines = (int)slider.value;
				Text text = (Text)GameObject.Find ("MinesText").GetComponent<Text> ();
				text.text = "Mines: " + numMines;
		}

		public void SetWidth ()
		{
				Slider slider = (Slider)GameObject.Find ("WidthSlider").GetComponent<Slider> ();
				graphWidth = (int)slider.value;
				Text text = (Text)GameObject.Find ("WidthText").GetComponent<Text> ();
				text.text = "Width: " + graphWidth;
		}

		public void SetHeight ()
		{
				Slider slider = (Slider)GameObject.Find ("HeightSlider").GetComponent<Slider> ();
				graphHeight = (int)slider.value;
				Text text = (Text)GameObject.Find ("HeightText").GetComponent<Text> ();
				text.text = "Height: " + graphHeight;
		}

		public void SetConnectedness ()
		{
				Slider slider = (Slider)GameObject.Find ("ConnectednessSlider").GetComponent<Slider> ();
				connectedness = (float)slider.value;
				Text text = (Text)GameObject.Find ("ConnectednessText").GetComponent<Text> ();
				text.text = "Connectedness: " + connectedness;
		}

		public void SetClustering ()
		{
				Slider slider = (Slider)GameObject.Find ("ClusteringSlider").GetComponent<Slider> ();
				clustering = (float)slider.value;
				Text text = (Text)GameObject.Find ("ClusteringText").GetComponent<Text> ();
				text.text = "Clustering: " + clustering;
		}
	
		public void CreateGraph ()
		{
				pg.ResetGraph ();

				pg.GenerateGraph (numMines, graphWidth, graphHeight, clustering, connectedness);

				DrawGraph ();
		}

		private void DestroyObjects ()
		{
				// destroy game objects
				GameObject[] mines = GameObject.FindGameObjectsWithTag ("Mine");
				GameObject[] nodes = GameObject.FindGameObjectsWithTag ("Node");
				GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");

				foreach (GameObject mine in mines) {
						GameObject.Destroy (mine);
				}

				foreach (GameObject node in nodes) {
						GameObject.Destroy (node);
				}
				
				foreach (GameObject line in lines) {
						GameObject.Destroy (line);
				}
		}

		void DrawGraph ()
		{
				// destroy game objects
				DestroyObjects ();

				// instantiate new nodes
				foreach (Node n in pg.GetNodes()) {
						if (n.IsRevealed) {
								if (n.HasMine) {
										GameObject mine = (GameObject)GameObject.Instantiate (Resources.Load ("Mine"), n.position, Quaternion.identity);

								} else {
										GameObject node = (GameObject)GameObject.Instantiate (Resources.Load ("Node"), n.position, Quaternion.identity);
								}
				
								// draw lines
								foreach (Node neighbor in n.Neighbors) {
										if (neighbor.IsRevealed) {
												GameObject dummyNode = new GameObject ();
												dummyNode.transform.position = neighbor.position;
												dummyNode.tag = "Line";
					
												LineRenderer lineRenderer = dummyNode.AddComponent<LineRenderer> ();
												lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
												lineRenderer.SetColors (c, c);
												lineRenderer.SetWidth (0.2F, 0.2F);
												lineRenderer.SetVertexCount (lengthOfLineRenderer);
												lineRenderer.SetPosition (0, n.position);
												lineRenderer.SetPosition (1, neighbor.position);
										}
								}
						}
					
				}

		}
}
