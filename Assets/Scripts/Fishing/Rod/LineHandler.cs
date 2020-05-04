using UnityEngine;

namespace Fishing.Rod
{
    public class LineHandler : MonoBehaviour
    {
        [SerializeField] private float SECTION_LENGTH = 1f;
        [SerializeField] private int _lineResolution = 10;
        [SerializeField] private Material _lineMaterial;
        [SerializeField] private float _dampening = .2f;
        private LineRenderer _line;
        private Transform _floaterPos;
        private VerletLinePoint[] _points;

        public Transform EndPoint {get; set;}

        private void InitRope()
        {
            Vector3 pointPosition = _floaterPos.position;

            for (int i = 0; i < _lineResolution; i++)
            {
                _points[i] = new VerletLinePoint(pointPosition, Physics.gravity);

                pointPosition.y -= SECTION_LENGTH;
            }
        }

        private void Update() 
        {
            if (_line != null)
                _line.SetPosition(0, transform.position);
            if (_floaterPos != null)
            {
                DisplayRope();
            }
        }

        private void FixedUpdate() 
        {
            if (_floaterPos != null)
                UpdateRopeSimulation(Time.fixedDeltaTime);    
        }

        private void DisplayRope()
        {
            for (int i = 0; i < _lineResolution; i++)
            {
                _line.SetPosition(i, _points[i].Pos);
            }
            //_floaterPos.LookAt(_points[_lineResolution - 2].Pos);
        }

        private void UpdateRopeSimulation(float dt)
        {
            // FIrst rope section to the tip of the rod
            _points[0].Pos = transform.position;
            _points[_points.Length - 1].Pos =  EndPoint.position;
            UpdateVerlet(dt);

            // Check for the currect section lenght
            // The more iterations you do here the more realistic it will look
            for (int i = 0; i < 20; i++)
            {
                UpdateConstrains();
            }
        }

        private void UpdateConstrains()
        {
            for (int i = 0; i < _lineResolution - 1; i++)
            {
                ConstrainLine(ref _points[i], ref _points[i + 1], SECTION_LENGTH);
            }
        }

        private void ConstrainLine(ref VerletLinePoint p1, ref VerletLinePoint p2,
            float restLength)
        {
            Vector3 delta = p2.Pos - p1.Pos;
            float deltaLength = delta.magnitude;

            float diff = (deltaLength - restLength)/deltaLength;

            p1.Pos += delta * diff * SECTION_LENGTH;
            p2.Pos -= delta * diff * SECTION_LENGTH;
        }

        private void UpdateVerlet(float dt)
        {
            for (int i = 0; i < _lineResolution; i++)
            {
                Verlet(ref _points[i], dt);
            }
        }

        /// <summary>
        /// Calculates the verlet acceleration using a point and a time delta
        /// </summary>
        /// <param name="point"> point to calculate for</param>
        /// <param name="dt"> delta time</param>
        private void Verlet(ref VerletLinePoint point, float dt)
        {
            Vector3 temp = point.Pos;
            point.Pos += ((point.Pos - point.OldPos) * _dampening) + (point.Acceleration*dt*dt);
            point.OldPos = temp;
        }

        public void NewTarget(Transform target)
        {
            _floaterPos = target;
            EndPoint = target;
            Floater tempF = _floaterPos.GetComponent<Floater>();
            
            // Calculate line distance via hypotenuse with player height
            Vector3 groundPos = transform.position;
            groundPos.y = tempF.Point.y;
            float a = Vector3.Distance(tempF.Point, groundPos);
            float b = transform.position.y;
            _lineResolution = Mathf.FloorToInt(Mathf.Sqrt((a*a) + (b*b)));
            _lineResolution *=2;
            _lineResolution += 2;
            Debug.Log(_lineResolution);
            _points = new VerletLinePoint[_lineResolution];
            gameObject.AddComponent<LineRenderer>();
            _line = GetComponent<LineRenderer>();
            _line.material = _lineMaterial;
            _line.startWidth = .02f;
            _line.endWidth = .02f;
            _line.SetPosition(0, transform.position);

            _line.positionCount = _lineResolution;
            InitRope();
        }

        public void Release()
        {
            _floaterPos = null;
            Destroy(_line);
            // Move point 2 thowards point 1 quickly.
        }
    }
}