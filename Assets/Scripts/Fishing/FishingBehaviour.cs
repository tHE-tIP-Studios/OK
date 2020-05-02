using System.Collections;
using UnityEngine;

namespace Fishing
{
    public class FishingBehaviour : MonoBehaviour
    {
        public virtual void StartBehaviour()
        {

        }

        private IEnumerator CWaitForFishBite()
        {
            yield return new WaitForSeconds(2);
        }
    }
}