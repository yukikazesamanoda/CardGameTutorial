using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// �������ڴ���������Ľ����߼���ʵ���� IPointerClickHandler �ӿ�����Ӧ����¼�
public class CardBlock : MonoBehaviour, IPointerClickHandler
{
    // �ٻ��������Ϸ����������ʾ�ٻ���ص� UI Ԫ��
    public GameObject summonBlock;
    // �����������Ϸ���󣬵�ǰ���ܲ���Ҫʹ��
    public GameObject attackBlock;
    // ��ǰ�����ڸ�����Ĺ��޿�����Ϸ����
    public GameObject monsterCard;
    public GameObject IconBlock;
    // Start �����ڽű�ʵ��������ʱ���ã�ͨ�����ڳ�ʼ������
    void Start()
    {
        // ����ע�͵��Ĵ������ڻ�ȡ BattleManager ʵ����������Ϊ�˺�����ս������������
        // BattleManager = GameObject.Find("AIBattleManager").GetComponent<BattleManager>();
    }

    // Update ����ÿ֡���ᱻ���ã�ĿǰΪ�գ��ɸ�����������߼�
    void Update()
    {

    }

    // �÷������ڼ����ٻ�������ʾ�ٻ���ص� UI Ԫ��
    public void SetSummon()
    {
        summonBlock.SetActive(true);
    }
    public void SetIcon()
    {
        IconBlock.SetActive(true);

    }

    // �÷������ڼ����������ʾ������ص� UI Ԫ��
    public void SetAttack()
    {
        attackBlock.SetActive(true);
    }

    // �÷������ڹر��ٻ�����͹�������������ص� UI Ԫ��
    public void CloseAll()
    {
        summonBlock.SetActive(false);
       attackBlock.SetActive(false);
     
    }

    // ʵ�� IPointerClickHandler �ӿڵķ��������ÿ������򱻵��ʱ����
    public void OnPointerClick(PointerEventData eventData)
    {
        // ����ٻ������Ƿ��ڼ���״̬
        if (summonBlock.activeInHierarchy)
        {
            // ���� BattleManager ʵ���� SummonCofirm ������ȷ���ٻ�����
            BattleManager.Instance.SummonCofirm(transform);
            // ����ע�͵��Ĵ������ڱ�Ǹ������Ѿ��й��ޣ������Ǿɵ�ʵ�ַ�ʽ
            // hasMonster = true;
        }
        // ���������������־��������ڵ��ԣ�ȷ�ϵ���¼��Ƿ񴥷�
        // Debug.Log("click block");
    }
}