using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishing.Area
{
    public class FishingMarker : MonoBehaviour
    {
        [SerializeField] private FishingArea _containingArea;

        public FishingArea ContainingArea => _containingArea;

        public void SetContainingArea(FishingArea area)
        {
            _containingArea = area;
        }

        public void Delete()
        {
            _containingArea.DeleteMarker(this);
        }
    }
}