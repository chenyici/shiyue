using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

[CSharpCallLua]
public delegate void LuaUpdateItemClass(int a, GameObject go);

[CSharpCallLua]
public class ScroLLViewRect : ScrollRect
{
    private float CurScroLLViewPos = 0;
    private int StartScroLLViewIndex; //当前位置对应的下标
    private int EndScroLLViewIndex; //当前位置对应的下标

    private int ChildCount = 5; //Content下的子节点数
    private int MaxCount = 10; //最大数量
    private int ItemDistance = 5; //每项之间的距离
    private int ItemSize = 50; //一项的大小，可以是宽，也可以是竖

    private Vector2 ContentFirstItemStartAnchoredPosition;

    public int ContentL = 0; //Content的总长度

    private List<int> ItemList = new List<int>();

    private LuaUpdateItemClass LuaUpdateItem;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Init()
    {
        ChildCount = content.transform.childCount;
        if (vertical)
        {
            ContentFirstItemStartAnchoredPosition = new Vector3(0, -ItemSize / 2, 0);
        }

        for (int i = 0; i < ChildCount; i++)
        {
            ItemList.Add(i);
            RefreshItemPaotion(i, content.transform.GetChild(i));
            RunLuaUpdateItem(i, content.transform.GetChild(i).gameObject);
        }
        ContentL = MaxCount * ItemSize + (MaxCount - 1) * ItemDistance;

        CurScroLLViewPos = 0;
        StartScroLLViewIndex = 0;
        EndScroLLViewIndex = ChildCount-1;

        content.sizeDelta = new Vector2(content.sizeDelta.x, ContentL);

        onValueChanged.AddListener(onValueChangedCallback);
    }

    public void SetData(int maxCount, int itemDistance, int itemSize, LuaUpdateItemClass luaUpdateItem) {
        MaxCount = maxCount;
        ItemDistance = itemDistance;
        ItemSize = itemSize;

        LuaUpdateItem = luaUpdateItem;
        Init();
    }

    public void AddItem(int addCount) {
        onValueChanged.RemoveListener(onValueChangedCallback);
        MaxCount = MaxCount + addCount;
        ContentL = MaxCount * ItemSize + (MaxCount - 1) * ItemDistance;
        content.sizeDelta = new Vector2(content.sizeDelta.x, ContentL);
        onValueChanged.AddListener(onValueChangedCallback);
    }

    public void onValueChangedCallback(Vector2 pos) {
        //Debug.Log("---" + content.localPosition.y);
        if (content.localPosition.y > CurScroLLViewPos) {
            //向下
            if (content.localPosition.y - CurScroLLViewPos >= ItemSize/2) {
                if (EndScroLLViewIndex < MaxCount-1)
                {
                    int moveIndex = ItemList[0];
                    ItemList.RemoveAt(0);
                    StartScroLLViewIndex += 1;
                    EndScroLLViewIndex += 1;
                    SetItemPosition(moveIndex, true);
                    ItemList.Add(moveIndex);

                    CurScroLLViewPos = StartScroLLViewIndex * ItemSize + (StartScroLLViewIndex-1) * ItemDistance;
                }
            }
        }

        if (content.localPosition.y < CurScroLLViewPos) {
            //向上
            if (CurScroLLViewPos - content.localPosition.y >= ItemSize/2)
            {
                if (StartScroLLViewIndex > 0)
                {
                    int moveIndex = ItemList[ItemList.Count-1];
                    ItemList.RemoveAt(ItemList.Count - 1);
                    StartScroLLViewIndex -= 1;
                    EndScroLLViewIndex -= 1;
                    SetItemPosition(moveIndex, false);
                    ItemList.Insert(0, moveIndex);
                    CurScroLLViewPos = StartScroLLViewIndex * ItemSize + (StartScroLLViewIndex - 1) * ItemDistance;
                }
            }
        }
    }

    private void SetItemPosition(int moveIndex, bool isDown) {
        if (vertical)
        {
            for (int i = 0; i < ChildCount; i++)
            {
                if (i == moveIndex) {
                    Transform tempTrans = content.transform.GetChild(i);
                    if (isDown)
                    {
                        RefreshItemPaotion(EndScroLLViewIndex, tempTrans);
                        RunLuaUpdateItem(EndScroLLViewIndex, tempTrans.gameObject);
                    }
                    else {
                        RefreshItemPaotion(StartScroLLViewIndex, tempTrans);
                        RunLuaUpdateItem(StartScroLLViewIndex, tempTrans.gameObject);
                    }
                    break;
                }
            }
        }
        else { 
        }
    }

    private void RefreshItemPaotion(int index, Transform goTrans) {
        float height = index * ItemSize + (index) * ItemDistance;
        goTrans.gameObject.name = index.ToString();
        goTrans.GetComponent<RectTransform>().anchoredPosition = ContentFirstItemStartAnchoredPosition + new Vector2(0, -height);
        Debug.Log(goTrans.localPosition);
    }

    private void RunLuaUpdateItem(int index, GameObject go) {
        if (LuaUpdateItem != null) {
            LuaUpdateItem(index, go);
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (LuaUpdateItem != null)
        {
            LuaUpdateItem = null;
        }
    }
}
