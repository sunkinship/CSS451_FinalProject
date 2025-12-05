using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    private List<Prize> collectedPrizes = new List<Prize>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Prize.PRIZE_TAG))
        {
            Prize prize = other.GetComponent<Prize>();
            if (prize == null)
            {
                Debug.LogError($"{other.name}: Object should have a Prize script attached to it");
                return;
            }
            if (collectedPrizes.Contains(prize))
               return;

            collectedPrizes.Add(prize);
            GameManager.Instance.GotPrize(other.GetComponent<Prize>().scoreValue);
        }
    }
}
