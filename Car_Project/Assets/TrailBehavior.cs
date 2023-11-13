//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TrailBehavior : MonoBehaviour
//{
//	[SerializeField]
//	private GameObject _Left;
//	[SerializeField]
//	private GameObject _Right;
//	[SerializeField]
//	private GameObject _trailmesh;
//	[SerializeField]
//	private int _trailframelength;

//	private Mesh _mesh;
//	private Vector3[] _vertices;
//	private int[] _triangles;
//	private int _framecount;
//	private Vector3 _previousleftposition;
//	private Vector3 _previousrightposition;

//	private const int NUM_VERTICES = 12;
	
//	void Start()
//    {
//        _mesh = new Mesh();
//		_trailmesh.GetComponent<MeshFilter>().mesh = _mesh;
//		_vertices = new Vector3[_trailframelength * NUM_VERTICES];
//		_triangles = new int[_vertices.Length];
//		_previousleftposition = _Left.transform.position;
//		_previousrightposition = _Right.transform.position;
//    }

//	// Update is called once per frame
//	void LateUpdate()
//	{
//		if (_framecount == (_trailframelength * NUM_VERTICES))
//		{
//			_framecount = 0;
//		}

//		_vertices[_framecount] = _Right.transform.position;
//		_vertices[_framecount + 1] = _Left.transform.position;
//		_vertices[_framecount + 2] = _previousleftposition;

//		_vertices[_framecount + 3] = _Right.transform.position;
//		_vertices[_framecount + 4] = _previousleftposition;
//		_vertices[_framecount + 5] = _Left.transform.position;

//		_vertices[_framecount + 6] = _previousleftposition;
//		_vertices[_framecount + 7] = _Right.transform.position;
//		_vertices[_framecount + 8] = _previousrightposition;

//		_vertices[_framecount + 9] = _previousleftposition;
//		_vertices[_framecount + 10] = _previousrightposition;
//		_vertices[_framecount + 11] = _Right.transform.position;

//		_triangles[_framecount] = _framecount;
//		_triangles[_framecount + 1] = _framecount + 1;
//		_triangles[_framecount + 2] = _framecount + 2;
//		_triangles[_framecount + 3] = _framecount + 3;
//		_triangles[_framecount + 4] = _framecount + 4;
//		_triangles[_framecount + 5] = _framecount + 5;
//		_triangles[_framecount + 6] = _framecount + 6;
//		_triangles[_framecount + 7] = _framecount + 7;
//		_triangles[_framecount + 8] = _framecount + 8;
//		_triangles[_framecount + 9] = _framecount + 9;
//		_triangles[_framecount + 10] = _framecount + 10;
//		_triangles[_framecount + 11] = _framecount + 11;

//		_mesh.vertices = _vertices;
//		_mesh.triangles = _triangles;

//		_previousleftposition = _Left.transform.position;
//		_previousrightposition = _Right.transform.position;

//		_framecount += NUM_VERTICES;


//	}
//}
