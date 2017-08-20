using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffData
{
    public ePropertyType propType;
    public float value;
    public float duration;
}

public class BuffEntity : MonoBehaviour
{
    public List<BuffData> buffDataList = new List<BuffData>();

    public void GiveBuff(GameObject targetObj)
    {
        if (CommonUtil.IsPlayer(targetObj) == false)
            return;
        
        Player player = targetObj.GetComponent<Player>();
        if (player == null)
            return;

        PlayerBuffManager playerBuffMgr = player.GetComponent<PlayerBuffManager>();
        if (playerBuffMgr == null)
            return;

        for (int i = 0; i < buffDataList.Count; i++)
        {
            BuffData buffData = buffDataList[i];

            BuffEffect buff = new BuffEffect();

            buff.duration = buffData.duration;
            buff.propType = buffData.propType;
            buff.value = buffData.value;
            buff.target = player.gameObject;

            playerBuffMgr.AddBuff(buff);
        }
    }
}
