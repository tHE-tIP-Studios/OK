using System.Collections;
using Fishing.Area;
using UnityEngine;

namespace Fishing.Rod
{
    public class Floater : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _mid;
        private FishingArea _currentArea;

        public Vector3 Point { get; private set; }

        public void Cast(FishingArea area, Vector3 pointToReach)
        {
            Point = pointToReach;
            StartCoroutine(MoveTo(pointToReach, transform.position, 1.5f));
            transform.parent = null;
            _currentArea = area;
        }

        public void FollowTarget(Transform target)
        {
            _target = target;
        }

        private void Update()
        {
            if (_target != null)
            {
                transform.position = _target.position;
            }
        }

        private Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            _mid = Vector3.Lerp(start, end, t);
            return new Vector3(_mid.x,
                ParabolaHeight(t, height) + Mathf.Lerp(start.y, end.y, t),
                _mid.z);
        }

        private float ParabolaHeight(float x, float height)
        {
            return -4 * height * x * x + 4 * height * x;
        }

        private IEnumerator MoveTo(Vector3 end, Vector3 start, float height)
        {
            float time = 0.0f;
            while (Vector3.Distance(_mid, end) > 0.0f)
            {
                transform.position = Parabola(start, end, height, time * 2);
                time += Time.deltaTime;
                yield return null;
            }
            OnWaterLand();
        }

        private void OnWaterLand()
        {
            _currentArea.FishingStart(transform);
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying) return;
            _currentArea?.FishingEnd(false);
        }
    }
}