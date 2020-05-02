using UnityEngine;

namespace Fishing.Rod
{
    public class LineHandler : MonoBehaviour
    {
        private LineRenderer _line;
        private Transform _floaterPos;
        
        private void Start() 
        {
            _floaterPos = transform.GetChild(0);
            _line = GetComponent<LineRenderer>();
            _line.SetPosition(0, transform.position);
        }

        private void Update() 
        {
            _line.SetPosition(0, transform.position);
            _line.SetPosition(1, _floaterPos.position);
        }
    }
}