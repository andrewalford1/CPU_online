using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bus : MonoBehaviour {

    [SerializeField] Animator[] animators = null;
    [SerializeField] Image[] sprites = null;

    private List<Sprite> animatedSprites = new List<Sprite>();

    private void Start() {
        for(int i = 0; i < sprites.Length; i++) {
            animatedSprites.Add(sprites[i].sprite);
        }
        ToggleAnimators(false);
    }

    private void ToggleAnimators(bool active) {

        for(int i = 0; i < sprites.Length; i++) {
            if(active) {
                sprites[i].sprite = animatedSprites[i];
            } else {
                sprites[i].sprite = null;
            }
        }
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
