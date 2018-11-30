using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTemporalFading : MonoBehaviour {

    public Color ColorLimitMin;
    public Color ColorLimitMax;
    public float FadeTime = 1f;

    private Material mat;

	void Start () {
        mat = gameObject.GetComponent<MeshRenderer>().material;
    }
	
	void Update () {
        mat.color = Color.Lerp(ColorLimitMin, ColorLimitMax, Mathf.Sin(Time.realtimeSinceStartup/FadeTime)/2f + 0.5f);

    }
}
