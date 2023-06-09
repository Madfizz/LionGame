using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface Interactable
{
    public static Action<bool> InteractUIPromptEvent;

    public void OnPlayerInteract(Collider player, bool canInteract);
}
