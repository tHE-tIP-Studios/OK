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
        protected FishAI _aiBehaviour;
        public FishingArea ContainingArea { get; private set; }

        public void Init(AnimalInfo info, FishingArea containingArea)
        {
            base.Init(info);
            ContainingArea = containingArea;
            _aiBehaviour = new FishAI(this);
            Behaviour = DoWander;
        }

        public void FishingStart()
        {
            Behaviour += LookForBait;
            FixedUpdateBehaviour = LookInFront;
        }

        public void FishingEnd()
        {
            FixedUpdateBehaviour = null;
            Behaviour = DoWander;
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

        private void LookForBait()
        {
            _aiBehaviour.LookForBait(Vector3.zero);
        }

        private void MoveTowardsBait()
        {
            _aiBehaviour.MoveTowardsBait(Vector3.zero);
        }
        //* ///

        //* Physics based behaviors
        private void LookInFront()
        {
            _aiBehaviour.LookForAnimalsInFront();
        }
        //* ///

        //* AI events
        public void BaitFound()
        {
            ContainingArea.FishInterested();
            Behaviour = MoveTowardsBait;
            FixedUpdateBehaviour -= LookInFront;
        }

        public void BiteBait()
        {
            Behaviour = null;
            FixedUpdateBehaviour = null;
            ContainingArea.FishBite(this);
        }

        public void LooseInterest()
        {
            ContainingArea.FishLostInterest();
            Behaviour = DoWander;
            Behaviour += LookForBait;
            FixedUpdateBehaviour += LookInFront;
        }
        //* ///

        private void OnDrawGizmos()
        {
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