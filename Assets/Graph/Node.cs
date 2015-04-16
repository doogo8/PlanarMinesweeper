using System.Collections;
using System;
using UnityEngine;

public class Node 
{
	public int id { get; set; }
	public Vector3 position { get; set; }
	public ArrayList Neighbors { get; set; }

	public bool HasMine { get; set; }
	public bool IsRevealed { get; set; }

	public Node () {
		position = new Vector3 ();
		Neighbors = new ArrayList ();
		HasMine = false;
		IsRevealed = false;
	}

	public Node (int i, Vector3 pos, bool mined) {
		id = i;
		position = pos;
		Neighbors = new ArrayList ();
		HasMine = mined;
		IsRevealed = false;
	}

}
