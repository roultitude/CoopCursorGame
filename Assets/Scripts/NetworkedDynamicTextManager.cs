using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkedDynamicTextManager : NetworkBehaviour
{
    public static NetworkedDynamicTextManager Instance;

    [SerializeField] private DynamicTextData defaultData;
    [SerializeField] private DynamicTextData[] textData;
    [SerializeField] private GameObject canvasPrefab;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateText2DSynced(Vector2 position, string text) //overload for default
    {
        int idx = DynamicTextDataToIndex(defaultData);
        CreateText2DRPC(position, text, idx);
    }
    public void CreateText2DSynced(Vector2 position, string text, DynamicTextData data)
    {
        int idx = DynamicTextDataToIndex(data);
        CreateText2DRPC(position, text, idx);
    }


    [Rpc(SendTo.Everyone)]
    private void CreateText2DRPC(Vector2 position, string text, int idx)
    {
        GameObject newText = Instantiate(canvasPrefab, position, Quaternion.identity);
        DynamicTextData data = IndexToDynamicTextData(idx);
        newText.transform.GetComponent<DynamicText2D>().Initialise(text, data);
    }
    
    private int DynamicTextDataToIndex(DynamicTextData data)
    {
        int idx = Array.FindIndex(textData, item => item == data);
        if(idx == -1)
        {
            Debug.LogWarning("DID NOT FIND REQUESTED TEXT DATA, RETURNING IDX = 0");
            idx = 0;
        }
        return idx;
    }

    private DynamicTextData IndexToDynamicTextData(int idx)
    {
        if(idx < textData.Length)
        {
            return textData[idx];
        } else
        {
            Debug.LogWarning("DID NOT FIND REQUESTED IINDEX, USING DEFAULT DATA");
            return defaultData;
        }
    }

    /*
    [Rpc(SendTo.Everyone)]
    private void CreateTextRPC(Vector3 position, string text, DynamicTextData data)
    {
        GameObject newText = Instantiate(canvasPrefab, position, Quaternion.identity);
        newText.transform.GetComponent<DynamicText>().Initialise(text, data);
    }
    */
}
