using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class CDMgr : MonoBehaviour  //����Ϊ��д������������ע��������Ҫ��Scene��object����һ�£���Ȼ����
{
    private static CDMgr instance;
    public static CDMgr Instance //�����þ�̬���������̬������ʵ�����Ը���ʵ���ķ��ʡ� 
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

    private Dictionary<string, float> dic = new(); //���漼��������ȴʱ����Ϣ
    private Dictionary<string, float> cds = new(); //���浱ǰ�ļ�������ʣ����ȴʱ��
    private List<string> ids = new(); //���漼����

    void Update()
    {
        //��ȴ��ʱ
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

    //��һ���������ַ���Ҳ�ɣ�����Ϊ�˷��㴫���õ�ö��
    //�ڶ�������Ϊcdʱ��
    public void AddCD(string id, float time)
    {
        if (dic.ContainsKey(id))
        {
            Debug.LogError("��ID�Ѵ��ڣ�");
            return;
        }
        dic.Add(id, time);
        cds.Add(id, 0); //һ��ʼû������ȴ
        ids.Add(id);
    }

    //������ȴ
    public void StartCool(string id)
    {
        if (!dic.ContainsKey(id))
        {
            Debug.LogError("�����ڸ�ID��");
            return;
        }

        cds[id] = dic[id];
    }

    //�Ƿ���ȴ�����������ȴ�����᷵��true�����¿�ʼ����cd
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
            Debug.LogError("�����ڸ�ID��");
            return false;
        }
    }

    //��ȡ��ǰCD
    public float? GetCurrentCD(string id)
    {
        if (cds.ContainsKey(id))
        {
            return cds[id];
        }
        else
        {
            Debug.LogError("�����ڸ�ID��");
            return null;
        }
    }

    //��ȡCDʱ��
    public float? GetWholeCD(string id)
    {
        if (dic.ContainsKey(id))
        {
            return dic[id];
        }
        else
        {
            Debug.LogError("�����ڸ�ID��");
            return null;
        }
    }

    //�Ƴ�id
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
            Debug.LogError("�����ڸ�ID��");
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