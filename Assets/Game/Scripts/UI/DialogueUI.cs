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
        [SerializeField] Button nextButton;
        [SerializeField] GameObject AIResponse;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;

        // Start is called before the first frame update
        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            nextButton.onClick.AddListener(Next);
            
            UpdateUI();
        }

        void Next()
        {
            playerConversant.Next();
            UpdateUI();
        }

        // Update is called once per frame
        void UpdateUI()
        {
            
            AIResponse.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());

            if (playerConversant.IsChoosing())
            {
                foreach(Transform item in choiceRoot)
            {
                    Destroy(item.gameObject);
                }

                foreach (DialogueNode choice in playerConversant.GetChoices())
                {
                    GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                    var textComp = choiceInstance.GetComponentInChildren<Text>();
                    textComp.text = choice.GetText();
                }
            }

            else
            {
                AIText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }
    }
}
