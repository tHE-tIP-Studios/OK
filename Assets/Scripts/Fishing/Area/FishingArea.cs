using UnityEngine;
using Utilities;

namespace Fishing.Area
{
    public class FishingArea : MonoBehaviour
    {
        public const string FISHING_LAYER = "FishingArea";
        [SerializeField] private Color _gizmosColor = Color.magenta;
        [SerializeField] private FishingMarker[] _markers = null;
        [SerializeField] private Transform _markersParent;
        [SerializeField] private FishingAreaType _areaType = default;

        private Vector2[] _polygonArray;
        private MeshCollider areaCollider;

        public Transform MarkersParent => _markersParent;
        public FishingMarker[] Markers => _markers;
        public FishingAreaType AreaType => _areaType;

        private void Awake()
        {
            if (_markers == null) return;

            _polygonArray = new Vector2[_markers.Length];
            for (int i = 0; i < _markers.Length; i++)
                _polygonArray[i] = new Vector2(_markers[i].transform.position.x, _markers[i].transform.position.z);

            CreateMesh();
        }

        // Solution taken from
        //http://wiki.unity3d.com/index.php?title=PolyContainsPoint
        // It ignores the Y for now!
        public bool IsInside(Vector3 positionToCheck)
        {
            if (_polygonArray == null) return false;

            Vector2 p = new Vector2(positionToCheck.x, positionToCheck.z);
            bool inside = false;
            int j = _polygonArray.Length - 1;
            for (int i = 0; i < _polygonArray.Length; j = i++)
            {
                Vector2 pi = _polygonArray[i];
                Vector2 pj = _polygonArray[j];
                if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                    (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                    inside = !inside;
            }
            return inside;
        }

        public void FishingStart()
        {
            areaCollider.enabled = false;
            Debug.Log("Started Fishing!");
        }

        public void FishingEnd()
        {
            areaCollider.enabled = true;
        }

        private void CreateMesh()
        {
            Mesh mesh = new Mesh();
            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            Vector3[] vertices = new Vector3[_markers.Length];
            int[] triangles;

            CreateShape();
            FinalizeMesh();

            void CreateShape()
            {
                for (int i = 0; i < _markers.Length; i++)
                    vertices[i] = _markers[i].transform.localPosition;

                Triangulator tr = new Triangulator(_polygonArray);
                triangles = tr.Triangulate();
            }

            void FinalizeMesh()
            {
                filter.mesh = mesh;
                mesh.Clear();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                areaCollider = gameObject.AddComponent<MeshCollider>();
            }
        }

        #region Inspector Methods
        public bool MarkersParentExists()
        {
            Transform found = transform.Find("Markers");

            _markersParent = found;

            return found != null;
        }

        public Transform CreateMarkersParent()
        {
            _markersParent = new GameObject("Markers").transform;
            _markersParent.SetParent(transform);
            return _markersParent;
        }

        public FishingMarker NewMarker()
        {
            FishingMarker newMarker = new GameObject()
                .AddComponent<FishingMarker>();
            newMarker.SetContainingArea(this);
            newMarker.transform.SetParent(MarkersParent);

            if (_markers == null)
            {
                _markers = new FishingMarker[1] { newMarker };
                newMarker.transform.position = transform.position;
                newMarker.name = "Marker 1";
            }
            else
            {
                FishingMarker[] newMarkerArray = new FishingMarker[_markers.Length + 1];
                for (int i = 0; i < _markers.Length + 1; i++)
                {
                    if (i < _markers.Length)
                        newMarkerArray[i] = _markers[i];
                    else
                    {
                        newMarkerArray[i] = newMarker;
                        newMarker.gameObject.name = "Marker " + (i + 1);
                    }
                }

                if (_markers.Length > 2)
                {
                    newMarker.transform.position = GetMiddlePoint(
                        _markers[0].transform.position,
                        _markers[_markers.Length - 1].transform.position
                    );
                }
                else
                {
                    newMarker.transform.position = MarkersParent.transform.position;
                }

                _markers = newMarkerArray;
            }

            return newMarker;
        }

        private Vector3 GetMiddlePoint(Vector3 point1, Vector3 point2)
        {
            Vector3 finalPos = Vector3.zero;
            finalPos.x = (point1.x + point2.x) / 2;
            finalPos.y = (point1.y + point2.y) / 2;
            finalPos.z = (point1.z + point2.z) / 2;
            return finalPos;
        }

        public void DeleteMarker(FishingMarker marker)
        {
            if (_markers.Length - 1 > 0)
            {
                bool deleted = false;
                FishingMarker[] newMarkerArray = new FishingMarker[_markers.Length - 1];
                for (int i = 0; i < _markers.Length; i++)
                {
                    if (_markers[i] == marker)
                        deleted = true;
                    else if (!deleted)
                        newMarkerArray[i] = _markers[i];
                    else
                    {
                        newMarkerArray[i - 1] = _markers[i];
                        _markers[i].name = "Marker " + (i);
                    }
                }

                _markers = newMarkerArray;
            }
            else
            {
                _markers = null;
            }

            DestroyImmediate(marker.gameObject);
        }

        public void ResetArea()
        {
            if (Markers == null) return;
            foreach (FishingMarker m in Markers)
                if (m != null)
                    DestroyImmediate(m.gameObject);

            _markers = null;
        }

        public bool ContainsMarker(Transform toCheck)
        {
            if (_markers == null) return false;

            for (int i = 0; i < _markers.Length; i++)
            {
                if (_markers[i] == toCheck)
                    return true;
            }
            return false;
        }
        #endregion

        private void OnDrawGizmos()
        {
            if (_markers == null) return;
            Gizmos.color = _gizmosColor;

            for (int i = 0; i < _markers.Length; i++)
            {
                if (_markers[i] == null) return;

                Vector3 from = _markers[i].transform.position;
                Vector3 to = i + 1 < _markers.Length ?
                    _markers[i + 1].transform.position :
                    _markers[0].transform.position;

                if (_markers.Length >= 2)
                    Gizmos.DrawLine(from, to);

                Gizmos.DrawSphere(from, .05f);

#if UNITY_EDITOR
                UnityEditor.Handles.Label(from, _markers[i].gameObject.name);
#endif
            }
        }
    }
}