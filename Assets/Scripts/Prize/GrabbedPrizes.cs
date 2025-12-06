using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GrabbedPrizes
{
    private List<Prize> grabbedPrizes = new();
    public bool IsCarryingPrize => grabbedPrizes.Count > 0;

    private Transform prizePos;
    private Collider prizeCol;  

    public GrabbedPrizes(Transform prizePos, Collider prizeSpot)
    {
        this.prizePos = prizePos;
        this.prizeCol = prizeSpot;
    }

    public void AddPrize(Prize prize)
    {
        if (prize == null || grabbedPrizes.Contains(prize)) return;

        grabbedPrizes.Add(prize);
    }

    public void GrabPrizes()
    {
        foreach (Prize prize in grabbedPrizes)
        {
            prize.OnGrab(prizePos);
        }
    }

    public void DropPrizes()
    {
        foreach (Prize prize in grabbedPrizes)
        {
            prize.OnRelease();
        }
        grabbedPrizes.Clear();
    }

    //rolls chance to drop based on prize overlap amount
    //the more overlap the prize has with the claw center, the lower the chance to drop
    public void TryDropChance()
    {
        for (int i = grabbedPrizes.Count - 1; i >= 0; i--)
        {
            float prizeOverlap = grabbedPrizes[i].GetOverlapPercent(prizeCol) * 100;
            float overlapRoll = Random.Range(0, 100);
            bool shouldDrop = prizeOverlap < overlapRoll;

            //Debug.Log($"{prizeOverlap} < {overlapRoll} Drop {shouldDrop}");

            if (shouldDrop)
            {
                grabbedPrizes[i].OnRelease();
                grabbedPrizes.RemoveAt(i);
            }
        }
    }
}
