using System;
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
        private Action _onFail;
        public Vector3 Point { get; private set; }

        public void Cast(FishingArea area, Vector3 pointToReach, Action onFail)
        {
            Point = pointToReach;
            StartCoroutine(MoveTo(pointToReach, transform.position, 2f));
            transform.parent = null;
            _onFail = onFail;
            _currentArea = area;
        }

        public void ReleaseFish()
        {
            _currentArea?.ActiveFish?.FishingEnd();
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
            // Wobble animation
            LeanTween.moveY(gameObject, transform.position.y - 0.5f, .85f).setEasePunch();
            _currentArea.FishingStart(transform, _onFail);
        }

        private void OnDisable()
        {
            if (!Application.isPlaying) return;
            _currentArea?.FishingEnd(false);
        }
    }
}