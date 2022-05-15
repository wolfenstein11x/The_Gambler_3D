using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;
using UnityEngine.UI;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Sprite profilePic;
        [SerializeField] Dialogue dialogue = null;
        [SerializeField] Dialogue midQuestDialogue = null;
        [SerializeField] Dialogue postQuestDialogue = null;

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (dialogue == null)
            {
                return false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);
            }
            return true;
        }

        public Sprite GetProfilePic()
        {
            return profilePic;
        }

        public void SetDialogueToMidQuest()
        {
            dialogue = midQuestDialogue;
        }

    }
}
