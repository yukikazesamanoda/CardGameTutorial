using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickCard : MonoBehaviour
{
    public DeckManager deckManger;
    private LibraryManager libraryManager;
    private PlayerDataManager pdm;
    // Start is called before the first frame update
    void Start()
    {
        libraryManager = GameObject.Find("LibraryManager").GetComponent<LibraryManager>();
        deckManger = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        pdm = GameObject.Find("PlayerData").GetComponent<PlayerDataManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnBuildClick()
    {
        int id = GetComponent<CardDisplay>().card.id;
        if (pdm.playerCards[id] > 0) // 确保牌库中有该卡牌
        {
            if (pdm.playerDeck[id] > 2){

            }else
            {
                pdm.playerCards[id] -= 1; // 减少牌库中对应卡牌的数量
                pdm.playerDeck[id] += 1;  // 增加卡组中对应卡牌的数量
                deckManger.UpdateDeck();
                libraryManager.UpdateLibrary();
                pdm.updateText();
                pdm.SavePlayerData(); // 保存数据到文件
            }
        }
    }
    public void OnRemoveClick()
    {
        int id = GetComponent<CardDisplay>().card.id;
        if (pdm.playerDeck[id] > 0) // 确保卡组中有该卡牌
        {
            pdm.playerDeck[id] -= 1;  // 减少卡组中对应卡牌的数量
            pdm.playerCards[id] += 1; // 增加牌库中对应卡牌的数量
            deckManger.UpdateDeck();
            libraryManager.UpdateLibrary();
            pdm.updateText();
            pdm.SavePlayerData(); // 保存数据到文件
        }
    }
    public void OnBattleClick()
    {

    }
}
