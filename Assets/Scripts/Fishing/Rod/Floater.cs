using UnityEngine;
using Fishing.Area;
using System.Collections;

namespace Fishing.Rod
{
    public class Floater : MonoBehaviour
    {
        [SerializeField] float _throwHeight = 5f;
        [SerializeField] float _pullHeight = 0f;
        Vector3 _mid;
        
        public void Cast (FishingArea area, Vector3 pointToReach)
        {
            Debug.Log("Casted");
            StartCoroutine(MoveTo(pointToReach, transform.position, _throwHeight));
        }

        public void PullBack()
        {
            StartCoroutine(MoveTo(transform.parent.position,
                 transform.position, _pullHeight));
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
            
            while(Vector3.Distance(_mid, end) > 0.0f)
            {
                transform.position = Parabola(start, end, height ,time);
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}