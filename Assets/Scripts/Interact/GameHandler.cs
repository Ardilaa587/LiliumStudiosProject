using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private WindowQuestPointer windowQuestPointer;
    [SerializeField] private Transform[] questTargets;

    private int currentTargetIndex = 0;
    [SerializeField] private Transform playerTransform;

    private void Start()
    {
        SetCurrentTargetPointer();
    }

    private void Update()
    {
        if (questTargets.Length > 0 && currentTargetIndex < questTargets.Length && playerTransform != null)
        {
            Transform currentTarget = questTargets[currentTargetIndex];

            if (Vector3.Distance(playerTransform.position, currentTarget.position) < 5f)
            {
                GoToNextQuestTarget();
            }
        }
    }

    private void SetCurrentTargetPointer()
    {
        if (questTargets.Length > 0 && currentTargetIndex < questTargets.Length)
        {
            windowQuestPointer.SetTarget(questTargets[currentTargetIndex]);
        }
        else
        {
            windowQuestPointer.Hide();
        }
    }

    public void GoToNextQuestTarget()
    {
        currentTargetIndex++;
        SetCurrentTargetPointer();
    }
}
