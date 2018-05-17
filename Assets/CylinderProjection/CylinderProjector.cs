using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CylinderProjector : MonoBehaviour {

    public static Mesh cylinderMesh
    {
        get
        {
            if (!CylinderProjector._cylinderMesh){
                var prim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                CylinderProjector._cylinderMesh =  prim.GetComponent<MeshFilter>().sharedMesh;
                GameObject.DestroyImmediate(prim);
            }
            return CylinderProjector._cylinderMesh;
        }
    }
	

    private static Mesh _cylinderMesh;

	private static bool isDebugMode = false;
	
	[MenuItem("CylinderProjection/DebugMode")]
	public static void ToggleDebugmode()
	{
		CylinderProjector.isDebugMode = !CylinderProjector.isDebugMode;
	}

    public int width = 1024;

    public int height = 512;

    public float cylinderRadius = 2;

    public float cylinderHeight = 1;
	
	public Camera cylinderCamera;

	public uint stride = 4;

	public uint capturePerFrame = 128;
	
	private Texture2D _texture;

	private Texture2D _slitTexture2D;

	private RenderTexture _slitTexture;

	public bool render = false;

	public Texture2D texture
	{
		get
		{
			if (this.width <= 0)
			{
				this.width = 1;
			}

			if (this.height <= 0)
			{
				this.height = 1;
			}
			if (!this._texture)
			{
				this._texture = new Texture2D(this.width,this.height,TextureFormat.RGBA32,false,false);
			}
			if (this._texture.width != this.width || this._texture.height != this.height)
			{
				this._texture = new Texture2D(this.width,this.height,TextureFormat.RGBA32,false,false);
			}
			return this._texture;
		}
	}
	
	public RenderTexture slitTexture{
		get
		{
			if (this.width <= 0)
			{
				this.width = 1;
			}
			if (this.height <= 0)
			{
				this.height = 1;
			}

			if (!this._slitTexture)
			{
				this._slitTexture = new RenderTexture((int)this.stride, this.height, 1);
				this._slitTexture2D = new Texture2D((int)this.stride, this.height, TextureFormat.RGBA32,false,false);
			}

			if (this._slitTexture.height != this.height)
			{
				this._slitTexture = new RenderTexture((int)this.stride,this.height,1);
				this._slitTexture2D = new Texture2D((int)this.stride, this.height, TextureFormat.RGBA32,false,false);
			}

			return this._slitTexture;
		}
		
	}

	// Update is called once per frame
	void Update ()
	{
		if (!this.render)
		{
			return;
		}
		this.StartRender();
		this.render = false;
	}

	void StartRender()
	{
		var tick = 2 * Mathf.PI / this.width * this.stride;
		this.cylinderCamera.targetTexture = this.slitTexture;
		this.cylinderCamera.orthographicSize = this.cylinderHeight;
		StartCoroutine(RenderJob());
	}

	IEnumerator RenderJob()
	{
		Time.timeScale = 0f;
		for (var i = 0; i < this.width/this.stride; i++)
		{
			RenderAt(i);
			if (i % this.capturePerFrame == 0)
			{
				yield return null;
			}
		}
		Time.timeScale = 1f;
	}
	

	void RenderAt(int index)
	{
		var tick = 2 * Mathf.PI / this.width * stride;
		var theta = tick * index;
		var cameraObject = this.cylinderCamera.gameObject;
		cameraObject.transform.localPosition = new Vector3(Mathf.Cos(theta)*this.cylinderRadius/2,this.transform.position.y,Mathf.Sin(theta) * this.cylinderRadius/2);
		cameraObject.transform.localRotation = Quaternion.AngleAxis(-theta * Mathf.Rad2Deg - 90,Vector3.up);
		this.cylinderCamera.Render();
		RenderTexture.active = this.slitTexture;
		this._slitTexture2D.ReadPixels( new Rect( 0, 0, this._slitTexture2D.width, this._slitTexture2D.height), 0, 0 );
		this._slitTexture2D.Apply();
		RenderTexture.active = null;
		Graphics.CopyTexture(this._slitTexture2D,0,0,0,0,this.slitTexture.width,this.slitTexture.height,this.texture,0,0,index * (int)this.stride,0);
		this.texture.Apply();
		
	}

	private void OnDrawGizmosSelected()
	{	
		this.DrawGizomos();
	}

	private void OnDrawGizmos()
	{
		if (!CylinderProjector.isDebugMode)
		{
			return;
		}
		this.DrawGizomos();
	}

	private void DrawGizomos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireMesh(CylinderProjector.cylinderMesh, this.transform.position, this.transform.rotation, new Vector3(this.cylinderRadius, this.cylinderHeight, this.cylinderRadius));
		Gizmos.color = Color.red;
		Gizmos.DrawRay(this.cylinderCamera.gameObject.transform.position,this.cylinderCamera.gameObject.transform.forward * 2 * this.cylinderRadius);

	}

}
