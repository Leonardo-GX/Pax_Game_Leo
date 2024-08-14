using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class CDMgr : MonoBehaviour  //这里为改写单例的样例，注意类名需要和Scene中object名称一致，不然报错
{
    private static CDMgr instance;
    public static CDMgr Instance //这里用静态属性替代静态方法来实现外界对该类实例的访问。 
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {

        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as CDMgr;
            DontDestroyOnLoad(gameObject);
        }

    }

    private Dictionary<string, float> dic = new(); //保存技能名和冷却时长信息
    private Dictionary<string, float> cds = new(); //保存当前的技能名和剩余冷却时间
    private List<string> ids = new(); //保存技能名

    void Update()
    {
        //冷却计时
        if (ids.Count > 0)
        {
            ids.ForEach(_ =>
            {
                if (cds[_] > 0)
                {
                    cds[_] -= Time.deltaTime;
                }
                else
                {
                    cds[_] = 0;
                }
            });
        }
    }

    //第一个参数传字符串也可，这里为了方便传参用的枚举
    //第二个参数为cd时长
    public void AddCD(string id, float time)
    {
        if (dic.ContainsKey(id))
        {
            Debug.LogError("该ID已存在！");
            return;
        }
        dic.Add(id, time);
        cds.Add(id, 0); //一开始没进入冷却
        ids.Add(id);
    }

    //进入冷却
    public void StartCool(string id)
    {
        if (!dic.ContainsKey(id))
        {
            Debug.LogError("不存在该ID！");
            return;
        }

        cds[id] = dic[id];
    }

    //是否冷却结束，如果冷却结束会返回true并重新开始计算cd
    public bool IsReady(string id)
    {
        if (cds.ContainsKey(id))
        {
            if (cds[id] <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.LogError("不存在该ID！");
            return false;
        }
    }

    //获取当前CD
    public float? GetCurrentCD(string id)
    {
        if (cds.ContainsKey(id))
        {
            return cds[id];
        }
        else
        {
            Debug.LogError("不存在该ID！");
            return null;
        }
    }

    //获取CD时长
    public float? GetWholeCD(string id)
    {
        if (dic.ContainsKey(id))
        {
            return dic[id];
        }
        else
        {
            Debug.LogError("不存在该ID！");
            return null;
        }
    }

    //移除id
    public void RemoveCD(string id)
    {
        if (dic.ContainsKey(id))
        {
            dic.Remove(id);
            cds.Remove(id);
            ids.Remove(id);
        }
        else
        {
            Debug.LogError("不存在该ID！");
        }
    }
}

public class SingletonMonoBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }
    }
    protected virtual void Awake()
    {

        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as T;
        }

    }
}