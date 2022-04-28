using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] Text AIText;

        // Start is called before the first frame update
        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            AIText.text = playerConversant.GetText();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
