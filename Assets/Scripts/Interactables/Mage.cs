using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Mage : MonoBehaviour, Interactable
{   
    [Header("Mage Data")]
    [SerializeField] Color _colour;
    [SerializeField] TextAsset _introductionText;
    [SerializeField] TextAsset _conclusionText;
    [SerializeField] float _moveOffTime;
    NPCEventData _mageEventData;

    [Header("Event Triggers")]
    [SerializeField] GameObject _introTrigger;
    [SerializeField] GameObject _conclusionTrigger;
    [SerializeField] GameObject _moveToEndGameTrigger;
    
    [Header("Position Data")]
    [SerializeField] Transform _pos1Start;
    [SerializeField] Transform _pos1End;
    [SerializeField] Transform _pos2;
    [SerializeField] Transform _finalPos;

    // Components
    SpriteRenderer _spriteRenderer;

    // Flags
    bool _introDone;
    bool _conclusionDone;

    // Events
    public static Action<bool> MageEvent;

    void OnEnable()
    {
        NarrativeManager.EndOfNarrativeEvent += OnDialogueFinished;
    }

    void OnDisable()
    {
        NarrativeManager.EndOfNarrativeEvent -= OnDialogueFinished;
    }

    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sortingLayerName = "Z0";
        _introDone = false;
        _conclusionDone = false;
        _introTrigger.SetActive(true);
        _conclusionTrigger.SetActive(false);
        _moveToEndGameTrigger.SetActive(false);
        _mageEventData = new NPCEventData(false, false, _pos1Start, _introductionText, _colour);
        transform.position = _pos1Start.position;
    }

    public void OnPlayerInteract(Collider player, bool canInteract)
    {
        // If conversations have not been completed
        if (canInteract)
        {
            _mageEventData.CanInteract = canInteract;
            _mageEventData.IsOnRightSide = player.transform.position.x > transform.position.x ? false : true;

            if (!_introDone || !_conclusionDone)
            {   
                // If intro is done, keep camera off of Mage since Mage moves off screen
                if (!_introDone)
                {
                    _mageEventData.Transform = _pos1Start;
                }  
                // Keep camera on Mage
                else
                {
                    _mageEventData.Transform = transform;
                }
                NPC.SendNarrativeDataEvent?.Invoke(_mageEventData);
                MageEvent?.Invoke(true);
            }
            // If conversation is completed and player exits tutorial level
            else 
            {
                // Move Mage to endgame area
                transform.position = _finalPos.position;
                _spriteRenderer.sortingLayerName = "Z6";
            }
        }
    }

    void OnDialogueFinished()
    {
        // Signals introduction has been completed
        if (_mageEventData.CurrentDialogue == _introductionText)
        {
            _introDone = true;
            _introTrigger.SetActive(false);
            _conclusionTrigger.SetActive(true);
            _mageEventData.CurrentDialogue = _conclusionText;
            StartCoroutine(MoveOutOfFrameAnim());
        }
        // Signals conclusion has been completed
        else if (_mageEventData.CurrentDialogue == _conclusionText)
        {
            _conclusionDone = true;
            _conclusionTrigger.SetActive(false);
            _moveToEndGameTrigger.SetActive(true);
        }
    }

    // Move Mage off screen mysteriously
    IEnumerator MoveOutOfFrameAnim()
    {
        Vector3 currentPos = _pos1Start.position;
        float time = 0;
        
        while (time < _moveOffTime)
        {
            transform.position = Vector3.Lerp(currentPos, _pos1End.position, time / _moveOffTime);
            time += Time.deltaTime;
            yield return null;
        }

        // Place Mage at tutorial end pos
        transform.position = _pos2.position;
    }
}
