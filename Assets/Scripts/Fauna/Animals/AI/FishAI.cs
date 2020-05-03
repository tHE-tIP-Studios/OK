using System.Collections;
using System.Collections.Generic;
using Fishing.Area;
using UnityEngine;

namespace Fauna.Animals.AI
{
    public class FishAI
    {
        public const float MIN_DISTANCE = 0.6f;
        //private const float INTEREST_COOLDOWN = 15.0f;
        private const float INTEREST_COOLDOWN = 0.1f;
        public const float FOV = 45.0f;
        public const float FOV_DISTANCE = 3.0f;
        private const int MOVE_POINTS = 3;
        private Fish _fish;
        private Vector3 _targetPos;

        private LayerMask _animalMask;
        private int _animalsInRange;

        private float _speedFactor;
        private float _baitSpeedFactor;

        private bool _baitFound;
        private bool _baitRollSuccessful;
        private bool IgnoreBait => Time.unscaledTime - _timeOfLooseInterest < INTEREST_COOLDOWN;
        private float _timeOfLooseInterest;

        private Transform transform => _fish.transform;

        public Vector3 TargetPoint => _targetPos;

        public FishAI(Fish fish)
        {
            _fish = fish;
            _speedFactor = 1;
            _baitSpeedFactor = 0.5f;
            _animalMask = LayerMask.GetMask(Animal.ANIMAL_LAYER_NAME);
            _timeOfLooseInterest = Time.unscaledTime;
            GetNewPoint();
        }

        public void DoWander(float moveSpeed = 1, float rotationSpeed = 0.6f)
        {
            DoMovement(moveSpeed * _speedFactor);
            DoRotation(rotationSpeed * _speedFactor);
            CheckDistanceToPoint();
        }

        public void MoveTowardsBait(Vector3 baitPos, float moveSpeed = 1, float rotationSpeed = 1.2f)
        {
            _targetPos = baitPos;
            DoMovement(moveSpeed * _baitSpeedFactor);
            DoRotation(rotationSpeed * (_baitSpeedFactor * 2));
            if (_baitFound && !IgnoreBait)
                CheckDistanceToBait(baitPos);
        }

        public void LookForAnimalsInFront()
        {
            if (Physics.Raycast(transform.position, transform.forward, MIN_DISTANCE * 2, _animalMask))
            {
                _speedFactor -= Time.fixedDeltaTime;
                if (_speedFactor < 0.1f)
                {
                    _speedFactor = 0.5f;
                    // Skip point
                    GetNewPoint();
                }
            }
            else if (_speedFactor < 1)
            {
                _speedFactor += Time.fixedDeltaTime;
            }
        }

        public void LookForBait(Vector3 baitPosition)
        {
            if (IgnoreBait || _fish.ContainingArea.BaitTaken) return;

            Vector3 directionToTarget = transform.position - baitPosition;
            float angle = Vector3.Angle(transform.forward, directionToTarget);
            float distance = directionToTarget.magnitude;

            if (Mathf.Abs(angle) < FOV && distance < FOV_DISTANCE)
            {
                OnBaitFound();
            }
        }

        public void LockOnBait()
        {
            Vector3 direction = _fish.ContainingArea.ActiveFishingScript.BaitVelocityDirection;
            direction.z = direction.y;
            direction.y = 0;
            direction.x *= _fish.ContainingArea.ActiveFishingScript.TendencySide;
            Vector3 newPos = (-direction) + _fish.ContainingArea.BaitTransform.position;
            transform.position = newPos;
            transform.LookAt(_fish.ContainingArea.BaitTransform);
        }

        private void OnBaitFound()
        {
            _baitFound = true;
            // Bait roll
            _baitRollSuccessful = Random.Range(.0f, 1.0f) <
                _fish.Info.CatchingValues.BaitInterest;
            _fish.BaitFound();
            Debug.Log("bait is in front of me");
        }

        private void DoMovement(float speed)
        {
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }

        private void DoRotation(float speed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_targetPos - transform.position);
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }

        private void CheckDistanceToPoint()
        {
            if (Vector3.Distance(_targetPos, transform.position) < MIN_DISTANCE)
                GetNewPoint();
        }

        private void CheckDistanceToBait(Vector3 baitPosition)
        {
            if (Vector3.Distance(baitPosition, transform.position) < MIN_DISTANCE)
            {
                if (_baitRollSuccessful)
                {
                    Debug.LogWarning("Bite!");
                    _fish.BiteBait();
                }
                else
                {
                    GetNewPoint();
                    Debug.LogWarning("Lost Interest");
                    _timeOfLooseInterest = Time.unscaledTime;
                    _baitFound = false;
                    _fish.LooseInterest();
                }
            }
        }

        private void GetNewPoint()
        {
            _targetPos = Spawner.GetRandomPosInArea(_fish.ContainingArea);
        }
    }
}