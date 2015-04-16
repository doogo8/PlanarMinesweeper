using UnityEngine;
using System.Collections;
using System;

public class Triangle
{
	public Node a;
	public Node b;
	public Node c;

	public Triangle (Node aa, Node bb, Node cc) {
		a = aa;
		b = bb;
		c = cc;
	}
	
	public bool ContainsNode (Node p)
	{
		// Prepare our barycentric variables
		Vector3 u = b.position - a.position;
		Vector3 v = c.position - a.position;
		Vector3 w = p.position - a.position;
		
		Vector3 vCrossW = Vector3.Cross(v, w);
		Vector3 vCrossU = Vector3.Cross(v, u);
		
		// Test sign of r
		if (Vector3.Dot(vCrossW, vCrossU) < 0)
			return false;
		
		Vector3 uCrossW = Vector3.Cross(u, w);
		Vector3 uCrossV = Vector3.Cross(u, v);
		
		// Test sign of t
		if (Vector3.Dot(uCrossW, uCrossV) < 0)
			return false;
		
		// At this point, we know that r and t and both > 0.
		// Therefore, as long as their sum is <= 1, each must be less <= 1
		float denom = uCrossV.magnitude;
		float r = vCrossW.magnitude / denom;
		float t = uCrossW.magnitude / denom;
		
		
		return (r + t <= 1);
	}

	public void ReWind() {
		if (b.position.z > a.position.z) {
			Node temp = b;
			b = a;
			a = temp;
		}
		if (c.position.z > a.position.z) {
			Node temp = c;
			c = a;
			a = temp;
		} 

		Vector3 v1 = b.position - a.position;
		Vector3 v2 = c.position - a.position;

		Vector3 cross = Vector3.Cross (v1, v2);

		if (cross.y > 0.0) {
			Node temp = b;
			b = c;
			c = temp;

		}
	}

	public bool CircleContains( Node n) {
		// http://www.gamedev.net/topic/504470-calculate-triangle-circumcircle-and-inscribed_circleincircle/

		// lines from a to b and a to c
		var AB = b.position - a.position;
		var AC = c.position - a.position;
		
		// perpendicular vector on triangle
		var N = Vector3.Normalize(Vector3.Cross(AB, AC));
		
		// find the points halfway on AB and AC
		var halfAB = a.position + AB*0.5f;
		var halfAC = a.position + AC*0.5f;
		
		// build vectors perpendicular to ab and ac
		var perpAB = Vector3.Cross(AB, N);
		var perpAC = Vector3.Cross(AC, N);
		
		// find intersection between the two lines
		// D: halfAB + t*perpAB
		// E: halfAC + s*perpAC
		var center = LineLineIntersection(halfAB, perpAB, halfAC, perpAC);
		// the radius is the distance between center and any point
		// distance(A, B) = length(A-B)
		var radius = Vector3.Distance(center, a.position);

		return Vector3.Distance(n.position, center) < radius;
	}
	
	Vector3 LineLineIntersection(Vector3 originD, Vector3 directionD, Vector3 originE, Vector3 directionE) {
		// http://www.gamedev.net/topic/504470-calculate-triangle-circumcircle-and-inscribed_circleincircle/

		directionD.Normalize();
		directionE.Normalize();
		var N = Vector3.Cross(directionD, directionE);
		var SR = originD - originE;
		var absX = Math.Abs(N.x);
		var absY = Math.Abs(N.y);
		var absZ = Math.Abs(N.z);
		float t;
		if (absZ > absX && absZ > absY) {
			t = (SR.x*directionE.y - SR.y*directionE.x)/N.z;
		} else if (absX > absY) {
			t = (SR.y*directionE.z - SR.z*directionE.y)/N.x;
		} else {
			t = (SR.z*directionE.x - SR.x*directionE.z)/N.y;
		}
		return originD - t*directionD;
	}
	
}

