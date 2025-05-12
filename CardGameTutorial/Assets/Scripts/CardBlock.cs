using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 该类用于处理卡牌区域的交互逻辑，实现了 IPointerClickHandler 接口以响应点击事件
public class CardBlock : MonoBehaviour, IPointerClickHandler
{
    // 召唤区域的游戏对象，用于显示召唤相关的 UI 元素
    public GameObject summonBlock;
    // 攻击区域的游戏对象，当前可能不需要使用
    public GameObject attackBlock;
    // 当前放置在该区域的怪兽卡牌游戏对象
    public GameObject monsterCard;
    public GameObject IconBlock;
    // Start 方法在脚本实例被启用时调用，通常用于初始化操作
    void Start()
    {
        // 这里注释掉的代码用于获取 BattleManager 实例，可能是为了后续与战斗管理器交互
        // BattleManager = GameObject.Find("AIBattleManager").GetComponent<BattleManager>();
    }

    // Update 方法每帧都会被调用，目前为空，可根据需求添加逻辑
    void Update()
    {

    }

    // 该方法用于激活召唤区域，显示召唤相关的 UI 元素
    public void SetSummon()
    {
        summonBlock.SetActive(true);
    }
    public void SetIcon()
    {
        IconBlock.SetActive(true);

    }

    // 该方法用于激活攻击区域，显示攻击相关的 UI 元素
    public void SetAttack()
    {
        attackBlock.SetActive(true);
    }

    // 该方法用于关闭召唤区域和攻击区域，隐藏相关的 UI 元素
    public void CloseAll()
    {
        summonBlock.SetActive(false);
       attackBlock.SetActive(false);
     
    }

    // 实现 IPointerClickHandler 接口的方法，当该卡牌区域被点击时触发
    public void OnPointerClick(PointerEventData eventData)
    {
        // 检查召唤区域是否处于激活状态
        if (summonBlock.activeInHierarchy)
        {
            // 调用 BattleManager 实例的 SummonCofirm 方法，确认召唤操作
            BattleManager.Instance.SummonCofirm(transform);
            // 这里注释掉的代码用于标记该区域已经有怪兽，可能是旧的实现方式
            // hasMonster = true;
        }
        // 可以在这里添加日志输出，用于调试，确认点击事件是否触发
        // Debug.Log("click block");
    }
}