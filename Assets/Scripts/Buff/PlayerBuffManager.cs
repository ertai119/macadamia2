using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{
    public List<BuffEffect> buffList = new List<BuffEffect>();

	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (buffList.Count == 0)
            return;

        List<BuffEffect> removeList = new List<BuffEffect>();

        for (int i = 0; i < buffList.Count; i++)
        {
            BuffEffect buff = buffList[i];
            buff.OnUpdate();

            if (buff.IsEnd())
            {
                buff.OnExit();
                removeList.Add(buff);
            }
        }

        if (removeList.Count > 0)
        {
            for (int i = 0; i < removeList.Count; i++)
            {
                buffList.Remove(removeList[i]);
            }
        }
	}

    public void ClearAllBuff()
    {
        buffList.Clear();
    }

    public void AddBuff(BuffEffect buff)
    {
        BuffEffect newBuff = new BuffEffect();
        newBuff = buff;
        buffList.Add(newBuff);

        buff.OnEnter();
    }
}
