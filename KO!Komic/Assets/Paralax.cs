using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [System.Serializable]
    public class ParalaxLayer
    {
        public Sprite background;
        public float parallaxStrength; // 0 (estático) a 1 (move com câmera)
        public Vector3 initialOffset = new Vector3(2.71f, 0, 0);
    }



}
