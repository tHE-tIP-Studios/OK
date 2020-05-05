using System;
using System.Collections;
using System.Collections.Generic;
using Fauna.Animals.AI;
using Fishing.Area;
using UnityEngine;

namespace Fauna.Animals
{
    public class Fish : Animal
    {
        [SerializeField] private bool _drawGizmos = false;
        protected FishAI _aiBehaviour;

        public Vector3 MouthPivotOffset { get; private set; }
        public FishingArea ContainingArea { get; private set; }

        public void Init(AnimalInfo info, FishingArea containingArea)
        {
            base.Init(info);
            ContainingArea = containingArea;
            MouthPivotOffset = transform.Find("Mouth Pivot").localPosition;
            _aiBehaviour = new FishAI(this);
            Behaviour = DoWander;
        }

        // Only called when caught, hopefully 
        public void ForceStopAI()
        {
            Behaviour = null;
            FixedUpdateBehaviour = null;
        }

        public void FishingStart()
        {
            Behaviour += LookForBait;
            FixedUpdateBehaviour = LookForOthers;
        }

        public void FishingEnd()
        {
            FixedUpdateBehaviour = null;
            Behaviour = DoWander;
        }

        public void RunFrom(Vector3 position)
        {

        }

        protected override void OnUpdate()
        {
            Behaviour?.Invoke();
        }

        private void FixedUpdate()
        {
            FixedUpdateBehaviour?.Invoke();
        }

        //* Behaviors
        private void DoWander()
        {
            _aiBehaviour.DoWander();
        }

        private void MoveTowardsBait()
        {
            _aiBehaviour.MoveTowardsBait(ContainingArea.BaitTransform.position);
        }

        private void LookForBait()
        {
            _aiBehaviour.LookForBait(ContainingArea.BaitTransform.position);
        }
        //* ///

        //* Physics based behaviors
        private void LookForOthers()
        {
            _aiBehaviour.LookForAnimalsInFront();
        }
        //* ///

        //* AI events
        public void BaitFound()
        {
            ContainingArea.FishInterested(this);
            Behaviour = MoveTowardsBait;
            FixedUpdateBehaviour -= LookForOthers;
        }

        public void BiteBait()
        {
            ForceStopAI();
            Behaviour = _aiBehaviour.LockOnBait;
            ContainingArea.FishBite(this);
        }

        public void LooseInterest()
        {
            ContainingArea.FishLostInterest();
            Behaviour = DoWander;
            Behaviour += LookForBait;
            FixedUpdateBehaviour += LookForOthers;
        }
        //* ///

        private void OnDrawGizmos()
        {
            if (!_drawGizmos) return;
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(_aiBehaviour.TargetPoint, FishAI.MIN_DISTANCE);
            Gizmos.DrawLine(transform.position, _aiBehaviour.TargetPoint);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * FishAI.FOV_DISTANCE);
            Gizmos.color = Color.blue;

            float totalFOV = FishAI.FOV;
            float rayRange = FishAI.FOV_DISTANCE;
            float halfFOV = totalFOV / 2.0f;

            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, transform.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, transform.up);
            Quaternion topRayRotation = Quaternion.AngleAxis(halfFOV, transform.right);
            Quaternion bottomRayRotation = Quaternion.AngleAxis(-halfFOV, transform.right);

            Vector3 leftRayDirection = leftRayRotation * transform.forward;
            Vector3 rightRayDirection = rightRayRotation * transform.forward;
            Vector3 topRayDirection = topRayRotation * transform.forward;
            Vector3 bottomRayDirection = bottomRayRotation * transform.forward;

            Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
            Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);
            Gizmos.DrawRay(transform.position, topRayDirection * rayRange);
            Gizmos.DrawRay(transform.position, bottomRayDirection * rayRange);
        }

        Action Behaviour;
        Action FixedUpdateBehaviour;
    }
}