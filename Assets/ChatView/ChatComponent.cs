using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatComponent : MonoBehaviour
{
    List<string> ChatIconList = new List<string>() {
        "#001",
        "#002"
    };
    List<string> IconsTypeList = new List<string>();
    List<int> IconsPosList = new List<int>();
    List<GameObject> IconsGoList = new List<GameObject>(); //一般建立缓存池

    private const int ITEM_ICON_WIDTH = 4; //每个#001占4位

    private Text mShowText;

    private Font mTextFont;
    private string mOriginalText;
    private float mLineTextWidth = 0;

    private void Awake()
    {
        mLineTextWidth = GetComponent<RectTransform>().sizeDelta.x;
        mShowText = GetComponent<Text>();
        RequestCharactersInTexture();
    }

    public void SetChatDesc(string desc) {
        mOriginalText = desc;
        mShowText.text = ReplaceIconIndexValue(desc);
        StartCoroutine(RunIERefreshIconList());
    }

    IEnumerator RunIERefreshIconList() {
        yield return null;
        RefreshIconList();
    }

    private string ReplaceIconIndexValue(string desc) {
        IconsTypeList.Clear();
        IconsPosList.Clear();

        for (int i = 0; i< desc.Length; i++)
        {
            if (desc[i] == '#')
            {
                string value = desc.Substring(i, ITEM_ICON_WIDTH);

                if (ChatIconList.Contains(value))
                {
                    desc = desc.Remove(i, ITEM_ICON_WIDTH).Insert(i, "\u00A0\u00A0\u00A0\u00A0");
                    int index = GetIconPosition(desc.Substring(0, i));
                    if (index != -1)
                    {
                        IconsTypeList.Add(value);
                        IconsPosList.Add(i);
                    }
                }
            }
        }

        return desc;
    }

    private void RefreshIconList() {

        for (int i = 0; i < IconsGoList.Count; i++) {
            IconsGoList[i].gameObject.SetActive(false);
        }

        float lineWidth = GetLineWidth();
        for (int i = 0; i < IconsTypeList.Count; i++) {
            GameObject tempIconGo = null;
            if (IconsGoList.Count > i){
                tempIconGo = IconsGoList[i];
            }
            else {
                tempIconGo = Resources.Load<GameObject>(IconsTypeList[i]);
                tempIconGo = GameObject.Instantiate(tempIconGo, mShowText.transform);
                tempIconGo.transform.localScale = Vector3.one;
                IconsGoList.Add(tempIconGo);
            }
            tempIconGo.GetComponent<RectTransform>().sizeDelta = new Vector2(mShowText.fontSize, mShowText.fontSize);
            tempIconGo.SetActive(true);

            int line = 0;
            float OffsetValue = CheckLine(IconsPosList[i], out line);

            tempIconGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(OffsetValue + mShowText.fontSize/2, -mShowText.fontSize / 2 - mShowText.fontSize * line - lineWidth * line);
        }
    }

    private float CheckLine(int index, out int line) {
        float value = 0;
        IList<UILineInfo> lines = mShowText.cachedTextGenerator.lines;

        line = 0;
        for (int i = lines.Count-1; i >= 0; i--){

            if (index >= lines[i].startCharIdx) {
                //Debug.Log(":Length:" + mShowText.text.Length);
                string temp = mShowText.text.Substring(lines[i].startCharIdx, index- lines[i].startCharIdx);
                int len = GetIconPosition(mShowText.text.Substring(lines[i].startCharIdx, index - lines[i].startCharIdx));
                //Debug.Log(":len:" + len);
                line = i;
                return len;
            }
        }
        return value;
    }

    /// <summary>
    /// 获取行与行之间的距离
    /// </summary>
    /// <returns></returns>
    private float GetLineWidth() {
        float lineWidth = 0;
        IList<UILineInfo> lines = mShowText.cachedTextGenerator.lines;
        if (lines.Count >= 2) {
            lineWidth = (lines[0].topY - lines[1].topY) - mShowText.fontSize;
        }
        return lineWidth;
    }

    private void RequestCharactersInTexture() {
        mTextFont = mShowText.font;
        mTextFont.RequestCharactersInTexture(mShowText.text, mShowText.fontSize, FontStyle.Normal);
    }

    private int GetIconPosition(string value) {
        if (mTextFont != null)
        {
            CharacterInfo characterInfo;
            float width = 0f;
            for (int j = 0; j < value.Length; j++)
            {
                mTextFont.GetCharacterInfo(value[j], out characterInfo, mShowText.fontSize);
                width += characterInfo.advance;
            }
            return (int)(width);
        }
        return -1;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < IconsGoList.Count; i++) {
            GameObject.Destroy(IconsGoList[i].gameObject);
        }
        IconsGoList.Clear();
    }
}
