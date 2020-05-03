using UnityEngine;
using Fishing.Area;
using System.Collections;

namespace Fishing.Rod
{
    public class Floater : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _mid;

        public void Cast(FishingArea area, Vector3 pointToReach)
        {
            StartCoroutine(MoveTo(pointToReach, transform.position, 1.5f));
            transform.parent = null;
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
        }
    }
}