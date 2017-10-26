﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitoring : MonoBehaviour
{
    [Header("---- 視点設定 ----")]
    [SerializeField]
    private float _fov = 60f;

    [SerializeField]
    private float _near = 0.03f;

    [SerializeField]
    private float _far = 1000f;

    [SerializeField]
    private Collider[] _targets;

    [SerializeField]
    private Color _insideColor = Color.blue;

    [SerializeField]
    private Color _intersectColor = Color.green;

    [SerializeField]
    private Color _outsideColor = Color.red;

    private Material[] _materials;

    private float Aspect
    {
        get
        {
            return (float)Screen.width / Screen.height;
        }
    }

    private void Start()
    {
        ShowPlanes();

        Matrix4x4 pmat = Camera.main.projectionMatrix;
        //Matrix4x4 m = Matrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane);

        _materials = new Material[_targets.Length];
        for (int i = 0; i < _targets.Length; i++)
        {
            _materials[i] = _targets[i].GetComponent<Renderer>().material;
        }
    }

    private void Update()
    {
        for (int i = 0; i < _targets.Length; i++)
        {
            CheckFrustum.State result = CheckFrustum.Detect(_targets[i], Camera.main.projectionMatrix, Camera.main.transform, Camera.main.nearClipPlane, Camera.main.farClipPlane);
            switch (result)
            {
                case CheckFrustum.State.Outside:
                    _materials[i].color = _outsideColor;
                    break;
                case CheckFrustum.State.Inside:
                    _materials[i].color = _insideColor;
                    break;
                case CheckFrustum.State.Intersect:
                    _materials[i].color = _intersectColor;
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawFrustum(transform.position, _fov, _far, _near, Aspect);
        //Gizmos.DrawFrustum(transform.position, _fov, _far, _near, Camera.main.aspect);
    }

    private void ShowPlanes()
    {
        Matrix4x4 pmat = Camera.main.projectionMatrix;
        //Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        Plane[] planes = CheckFrustum.CalculateFrustumPlanes(pmat, Camera.main.transform, Camera.main.nearClipPlane, Camera.main.farClipPlane);
        int i = 0;
        while (i < planes.Length)
        {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
            p.name = "Plane " + i.ToString();
            p.transform.position = -planes[i].normal * planes[i].distance;
            p.transform.rotation = Quaternion.FromToRotation(Vector3.up, planes[i].normal);
            i++;

            p.transform.parent = Camera.main.transform;
        }
    }
}