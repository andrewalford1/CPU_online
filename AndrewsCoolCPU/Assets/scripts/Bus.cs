using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour {

    [SerializeField] Animator[] animators = null;

    private void Start() {
        ToggleAnimators(false);
    }

    private void ToggleAnimators(bool active) {
        foreach (Animator animator in animators)
        {
            animator.enabled = active;
        }
    }

    public void StartTransferringData() {
        ToggleAnimators(true);
    }

    public void StopTransferringData() {
        ToggleAnimators(false);
    }
}
