using UnityEngine;
using System.Collections;
using System;

public class PlanarGraph
{
		public Hashtable Nodes;
		public bool Created;

		private ArrayList triangles;
		private int numNodes;
		private int numEdges;
		private int length;
		private int height;
		private float connectedness;
		private float clustering;
		private float minePct;

		public PlanarGraph (int nn, int len, int ht, float con, float cl, float mp)
		{
				Nodes = new Hashtable ();
				Created = false;
				triangles = new ArrayList ();
				numNodes = nn;
				numEdges = 0;
				clustering = cl;
				length = len;
				height = ht;
				connectedness = con;
				minePct = mp;
		}
	
		public void GenerateGraph (int nn, int len, int ht, float con, float cl)
		{
				Created = true;
				numNodes = nn;
				clustering = cl;
				length = len;
				height = ht;
				connectedness = con;

				// generate point cloud
				for (int i = 0; i < numNodes; i++) {
						Node n;
						if (i == 0) {
								n = new Node (i, new Vector3 (UnityEngine.Random.Range (-length / 2, length / 2), 0, UnityEngine.Random.Range (-height / 2, height / 2)), UnityEngine.Random.value < minePct ? true : false);
				
						} else {
								Node prev = (Node)Nodes [i - 1];
								Vector3 prevPos = prev.position;
							
								if (UnityEngine.Random.value < clustering) {
										n = new Node (i, new Vector3 (prevPos.x + UnityEngine.Random.Range (-length / 10, length / 10), 0, prevPos.z + UnityEngine.Random.Range (-height / 10, height / 10)), UnityEngine.Random.value < minePct ? true : false);
								} else {
										n = new Node (i, new Vector3 (UnityEngine.Random.Range (-length / 2, length / 2), 0, UnityEngine.Random.Range (-height / 2, height / 2)), UnityEngine.Random.value < minePct ? true : false);
					
								}
				
						}	
						Nodes.Add (i, n);
				}

				// create bounding triangle
				Node top = new Node (1000, new Vector3 (0, 0, 10000), false);
				Node ll = new Node (1001, new Vector3 (-10000, 0, -10000), false);
				Node lr = new Node (1002, new Vector3 (10000, 0, -10000), false);
				Triangle first = new Triangle (top, ll, lr);
				triangles.Add (first);

				ArrayList trianglesRemove = new ArrayList ();

				// make triangulation
				Triangle t1, t2, t3;
				foreach (Node n in Nodes.Values) {
						trianglesRemove = new ArrayList ();

						foreach (Triangle t in triangles) {
								// insert new node 
								if (t.ContainsNode (n)) {
										// add new traingles for new node
										t1 = new Triangle (t.a, t.b, n);
										t1.ReWind ();
										triangles.Add (t1);
										t2 = new Triangle (t.b, t.c, n);
										t2.ReWind ();
										triangles.Add (t2);
										t3 = new Triangle (t.c, t.a, n);
										t3.ReWind ();
										triangles.Add (t3);

										// remove old triangle
										trianglesRemove.Add (t);
										break;
								}
						}

						foreach (Triangle rem in trianglesRemove) {
								triangles.Remove (rem);
						}
				}

				// make delaunay
				trianglesRemove = new ArrayList ();
				ArrayList trianglesAdd = new ArrayList ();

				bool flip = true;

				while (flip) {
						flip = false;
						trianglesRemove.Clear ();
						trianglesAdd.Clear ();

						foreach (Triangle circleTri in triangles) {

								if (flip) {
										break;
								}	

								foreach (Triangle otherTri in triangles) {
										if (circleTri == otherTri) {
												continue;
										}
										// node c
										if ((circleTri.a == otherTri.a && circleTri.b == otherTri.b) ||
												(circleTri.b == otherTri.a && circleTri.a == otherTri.b)) {
												if (circleTri.CircleContains (otherTri.c)) {
														Triangle flipTri1 = new Triangle (circleTri.a, circleTri.c, otherTri.c);
														flipTri1.ReWind ();
														trianglesAdd.Add (flipTri1);

														Triangle flipTri2 = new Triangle (circleTri.b, circleTri.c, otherTri.c);
														flipTri2.ReWind ();
														trianglesAdd.Add (flipTri2);

														trianglesRemove.Add (circleTri);
														trianglesRemove.Add (otherTri);

														flip = true;
														break;
												}
										} else if ((circleTri.b == otherTri.a && circleTri.c == otherTri.b) ||
												(circleTri.c == otherTri.a && circleTri.b == otherTri.b)) {
												if (circleTri.CircleContains (otherTri.c)) {
														Triangle flipTri1 = new Triangle (circleTri.a, circleTri.b, otherTri.c);
														flipTri1.ReWind ();
														trianglesAdd.Add (flipTri1);
							
														Triangle flipTri2 = new Triangle (circleTri.a, circleTri.c, otherTri.c);
														flipTri2.ReWind ();
														trianglesAdd.Add (flipTri2);
							
														trianglesRemove.Add (circleTri);
														trianglesRemove.Add (otherTri);
							
														flip = true;
														break;
												}
										} else if ((circleTri.a == otherTri.a && circleTri.c == otherTri.b) ||
												(circleTri.c == otherTri.a && circleTri.a == otherTri.b)) {
												if (circleTri.CircleContains (otherTri.c)) {
														Triangle flipTri1 = new Triangle (circleTri.b, circleTri.a, otherTri.c);
														flipTri1.ReWind ();
														trianglesAdd.Add (flipTri1);
							
														Triangle flipTri2 = new Triangle (circleTri.b, circleTri.c, otherTri.c);
														flipTri2.ReWind ();
														trianglesAdd.Add (flipTri2);
							
														trianglesRemove.Add (circleTri);
														trianglesRemove.Add (otherTri);
							
														flip = true;
														break;
												}
										}

					// node a
					else if ((circleTri.b == otherTri.b && circleTri.c == otherTri.c) ||
												(circleTri.c == otherTri.b && circleTri.b == otherTri.c)) {
												if (circleTri.CircleContains (otherTri.a)) {
														Triangle flipTri1 = new Triangle (circleTri.a, circleTri.b, otherTri.a);
														flipTri1.ReWind ();
														trianglesAdd.Add (flipTri1);
							
														Triangle flipTri2 = new Triangle (circleTri.a, circleTri.c, otherTri.a);
														flipTri2.ReWind ();
														trianglesAdd.Add (flipTri2);
							
														trianglesRemove.Add (circleTri);
														trianglesRemove.Add (otherTri);
							
														flip = true;
														break;
												}
										} else if ((circleTri.a == otherTri.b && circleTri.c == otherTri.c) ||
												(circleTri.c == otherTri.b && circleTri.a == otherTri.c)) {
												if (circleTri.CircleContains (otherTri.a)) {
														Triangle flipTri1 = new Triangle (circleTri.b, circleTri.a, otherTri.a);
														flipTri1.ReWind ();
														trianglesAdd.Add (flipTri1);
							
														Triangle flipTri2 = new Triangle (circleTri.b, circleTri.c, otherTri.a);
														flipTri2.ReWind ();
														trianglesAdd.Add (flipTri2);
							
														trianglesRemove.Add (circleTri);
														trianglesRemove.Add (otherTri);
							
														flip = true;
														break;
												}
										} else if ((circleTri.a == otherTri.b && circleTri.b == otherTri.c) ||
												(circleTri.b == otherTri.b && circleTri.a == otherTri.c)) {
												if (circleTri.CircleContains (otherTri.a)) {
														Triangle flipTri1 = new Triangle (circleTri.c, circleTri.a, otherTri.a);
														flipTri1.ReWind ();
														trianglesAdd.Add (flipTri1);
							
														Triangle flipTri2 = new Triangle (circleTri.c, circleTri.b, otherTri.a);
														flipTri2.ReWind ();
														trianglesAdd.Add (flipTri2);
							
														trianglesRemove.Add (circleTri);
														trianglesRemove.Add (otherTri);
							
														flip = true;
														break;
												}
										}

					// node b
					else if ((circleTri.a == otherTri.a && circleTri.c == otherTri.c) ||
												(circleTri.c == otherTri.a && circleTri.a == otherTri.c)) {
												if (circleTri.CircleContains (otherTri.b)) {
														Triangle flipTri1 = new Triangle (circleTri.b, circleTri.c, otherTri.b);
														flipTri1.ReWind ();
														trianglesAdd.Add (flipTri1);
							
														Triangle flipTri2 = new Triangle (circleTri.b, circleTri.a, otherTri.b);
														flipTri2.ReWind ();
														trianglesAdd.Add (flipTri2);
							
														trianglesRemove.Add (circleTri);
														trianglesRemove.Add (otherTri);
							
														flip = true;
														break;
												}
										} else if ((circleTri.a == otherTri.a && circleTri.b == otherTri.c) ||
												(circleTri.b == otherTri.a && circleTri.a == otherTri.c)) {
												if (circleTri.CircleContains (otherTri.b)) {
														Triangle flipTri1 = new Triangle (circleTri.c, circleTri.a, otherTri.b);
														flipTri1.ReWind ();
														trianglesAdd.Add (flipTri1);
							
														Triangle flipTri2 = new Triangle (circleTri.c, circleTri.b, otherTri.b);
														flipTri2.ReWind ();
														trianglesAdd.Add (flipTri2);
							
														trianglesRemove.Add (circleTri);
														trianglesRemove.Add (otherTri);
							
														flip = true;
														break;
												}
										} else if ((circleTri.b == otherTri.a && circleTri.c == otherTri.c) ||
												(circleTri.c == otherTri.a && circleTri.b == otherTri.c)) {
												if (circleTri.CircleContains (otherTri.b)) {
														Triangle flipTri1 = new Triangle (circleTri.a, circleTri.b, otherTri.b);
														flipTri1.ReWind ();
														trianglesAdd.Add (flipTri1);
							
														Triangle flipTri2 = new Triangle (circleTri.a, circleTri.c, otherTri.b);
														flipTri2.ReWind ();
														trianglesAdd.Add (flipTri2);
							
														trianglesRemove.Add (circleTri);
														trianglesRemove.Add (otherTri);
							
														flip = true;
														break;
												}
										} 
								}
						}

						// update lists
						foreach (Triangle t in trianglesRemove) {
								triangles.Remove (t);
						}

						foreach (Triangle t in trianglesAdd) {
								triangles.Add (t);
						}
				}

				// remove bounding triangles
				trianglesRemove.Clear ();

				foreach (Triangle t in triangles) {
						if (t.a == top || t.b == top || t.c == top ||
								t.a == ll || t.b == ll || t.c == ll ||
								t.a == lr || t.b == lr || t.c == lr) {
								trianglesRemove.Add (t);
						}
				}

				foreach (Triangle t in trianglesRemove) {
						if (t.a == top || t.b == top || t.c == top ||
								t.a == ll || t.b == ll || t.c == ll ||
								t.a == lr || t.b == lr || t.c == lr) {
								triangles.Remove (t);
						}
				}

				// populate nodes
				foreach (Triangle t in triangles) {
						Node a = (Node)Nodes [t.a.id];
					
						if (!a.Neighbors.Contains (t.b)) {
								a.Neighbors.Add (t.b);
								numEdges++;
						}

						if (!a.Neighbors.Contains (t.c)) {
								a.Neighbors.Add (t.c);
								numEdges++;
						}
					
						Nodes.Remove (a.id);
						Nodes.Add (a.id, a);

						Node b = (Node)Nodes [t.b.id];
			
						if (!b.Neighbors.Contains (t.a)) {
								b.Neighbors.Add (t.a);
								numEdges++;
						}

						if (!b.Neighbors.Contains (t.c)) {
								b.Neighbors.Add (t.c);
								numEdges++;
						}

						Nodes.Remove (b.id);
						Nodes.Add (b.id, b);

						Node c = (Node)Nodes [t.c.id];
			
						if (!c.Neighbors.Contains (t.a)) {
								c.Neighbors.Add (t.a);
								numEdges++;
						}
			
						if (!b.Neighbors.Contains (t.b)) {
								c.Neighbors.Add (t.b);
								numEdges++;
						}

						Nodes.Remove (c.id);
						Nodes.Add (c.id, c);
				}

				// remove some edges
				numEdges /= 2;
				int edgesToRemove = (int)((1 - connectedness) * (numEdges - (numNodes - 1)));

				while (edgesToRemove > 0) {
						int randomIndex = UnityEngine.Random.Range (0, numNodes - 1);
						Node randomNode = (Node)Nodes [randomIndex];
					
						if (randomNode.Neighbors.Count >= 2) {
								int neighborIndex = UnityEngine.Random.Range (0, randomNode.Neighbors.Count - 1);
								Node neighborNodeRef = (Node)randomNode.Neighbors [neighborIndex];
								Node neighborNode = (Node)Nodes [neighborNodeRef.id];
						
								if (neighborNode.Neighbors.Count >= 2) {
										randomNode.Neighbors.RemoveAt (neighborIndex);
							
										for (int i = 0; i < neighborNode.Neighbors.Count; i++) {
												if (neighborNode.Neighbors [i] == randomNode) {
														neighborNode.Neighbors.RemoveAt (i);
														break;
												}
										}
							
										Nodes.Remove (randomNode.id);
										Nodes.Add (randomNode.id, randomNode);
										Nodes.Remove (neighborNode.id);
										Nodes.Add (neighborNode.id, neighborNode);

										edgesToRemove--;
								}
						}
				}
		}

	public void ResetGraph ()
		{ 

				Nodes.Clear ();
				triangles.Clear ();

		}

		public IEnumerable GetNodes ()
		{
				return Nodes.Values;
		}

		public void PrintInfo ()
		{
				Debug.Log (triangles.Count);
		}
}


