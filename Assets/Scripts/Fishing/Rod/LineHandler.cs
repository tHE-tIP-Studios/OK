using UnityEngine;

namespace Fishing.Rod
{
    public class LineHandler : MonoBehaviour
    {
        private LineRenderer _line;
        private Transform _floaterPos;
        
        private void Start() 
        {
            _line = GetComponent<LineRenderer>();
            _line.SetPosition(0, transform.position);
        }

        private void Update() 
        {
            _line.SetPosition(0, transform.position);
            if (_floaterPos != null)
            {
                _line.SetPosition(1, _floaterPos.position);
            }
            else
            {
                _line.SetPosition(1, transform.position);

            }
        }

        public void NewTarget(Transform target)
        {
            _floaterPos = target;
        }

        public void Release()
        {
            _floaterPos = null;
            // Move point 2 thowards point 1 quickly.
        }
    }
}