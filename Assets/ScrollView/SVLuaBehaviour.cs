using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace XLuaTest
{
    [CSharpCallLua]
    public class SVLuaBehaviour : MonoBehaviour
    {

        public ScroLLViewRect ScroLLViewRect;
        public TextAsset luaScript;
        public Injection[] injections;
        LuaEnv luaEnv;

        //private Action LuaUpdateItem;
        private Action luaOnDestroy;
        private LuaTable scriptEnv;

        void Awake()
        {
            luaEnv = new LuaEnv();
            luaEnv.DoString(luaScript.text, "LuaTestScroLLViewScript", scriptEnv);

            LuaUpdateItemClass calc_new = luaEnv.Global.GetInPath<LuaUpdateItemClass>("LuaUpdateItem");
            int maxCount = luaEnv.Global.GetInPath<int>("MaxCount");
            int itemDistance = luaEnv.Global.GetInPath<int>("ItemDistance");
            int itemSize = luaEnv.Global.GetInPath<int>("ItemSize");
            ScroLLViewRect.SetData(maxCount, itemDistance, itemSize, calc_new);
        }


        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 300, 80), "Add 50"))
            {
                ScroLLViewRect.AddItem(50);
            }
        }
        void OnDestroy()
        {
            if (luaOnDestroy != null)
            {
                luaOnDestroy();
            }
        }
    }
}
