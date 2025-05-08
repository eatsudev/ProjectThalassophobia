using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueItem : Interactable
{
    [TextArea]
    public string content;
    public Sprite clueSprite;

    public override void Interact()
    {
        Debug.Log("Reading item: " + interactName);
        UIManager.Instance.ShowReadableUI(content, clueSprite);
    }
}
