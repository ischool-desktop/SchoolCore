using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolCore
{
    public class PermRecLogProcess
    {
        // 修改前
        Dictionary<string, string> _BeforeData;
        // 修改后
        Dictionary<string, string> _AfterData;

        string _ActionBy = "";
        string _Action = "";
        private string _DescTitile = "";

        /// <summary>
        /// 动作来自谁
        /// </summary>
        /// <param name="str"></param>
        public void SetActionBy(string strModelName, string strItemName)
        {
            _ActionBy = strModelName + "." + strItemName;
        }

        /// <summary>
        /// 设定说明Title
        /// </summary>
        /// <param name="DescTitle"></param>
        public void SetDescTitle(string DescTitle)
        {
            _DescTitile = DescTitle;
        }

        public string GetDescTitle()
        {
            return _DescTitile;
        }
        /// <summary>
        /// 动作
        /// </summary>
        /// <param name="strAction"></param>
        public void SetAction(string strAction)
        {
            _Action = strAction;
        }

        public string GetAction()
        {
            return _Action;
        }

        public PermRecLogProcess()
        {
            _BeforeData = new Dictionary<string, string>();
            _AfterData = new Dictionary<string, string>();
        }

        /// <summary>
        /// 储存前数据
        /// </summary>
        /// <param name="TextTitle"></param>
        /// <param name="Text"></param>
        public void SetBeforeSaveText(string TextTitle, string Text)
        {
            if (_BeforeData.ContainsKey(TextTitle))
                _BeforeData[TextTitle] = Text;
            else
                _BeforeData.Add(TextTitle, Text);
        }

        public Dictionary<string, string> GetBeforeSaveText()
        {
            return _BeforeData;
        }

        public Dictionary<string, string> GetAfterSaveText()
        {
            return _AfterData;
        }

        /// <summary>
        /// 储存后数据
        /// </summary>
        /// <param name="TextTitle"></param>
        /// <param name="Text"></param>
        public void SetAfterSaveText(string TextTitle, string Text)
        {
            if (_AfterData.ContainsKey(TextTitle))
                _AfterData[TextTitle] = Text;
            else
                _AfterData.Add(TextTitle, Text);
        }

        /// <summary>
        /// 储存 log(记数据修改前后),targetCategory:目标分类,targetID:辨识ID
        /// </summary>
        /// <param name="BeforeTitleName"></param>
        /// <param name="AfterTitleName"></param>
        public void SaveLog(string BeforeTitleName, string AfterTitleName, string targetCategory, string targetID)
        {
            //string strDecs = "";
            StringBuilder sb = new StringBuilder();

            // 修改
            foreach (KeyValuePair<string, string> item in _BeforeData)
            {
                if (_AfterData.ContainsKey(item.Key))
                {
                    if (item.Value == _AfterData[item.Key]) continue;

                    sb.Append("[" + item.Key + "] 由「 " + item.Value + "」");
                    sb.Append("改为「" + _AfterData[item.Key] + "」;");
                    sb.Append("\r\n");
                }
            }

            // 当有新增时
            foreach (KeyValuePair<string, string> item in _AfterData)
            {
                if (!_BeforeData.ContainsKey(item.Key))
                {
                    sb.Append("新增[" + item.Key + "] 「" + _AfterData[item.Key] + "」;");
                    sb.Append("\r\n");
                }
            }


            // 当有删除
            foreach (KeyValuePair<string, string> item in _BeforeData)
            {
                if (!_AfterData.ContainsKey(item.Key))
                {
                    sb.Append("[" + item.Key + "] 由「 " + item.Value + "」");
                    sb.Append("被删除;");
                    sb.Append("\r\n");
                }
            }


            FISCA.LogAgent.ApplicationLog.Log(_ActionBy, _Action, targetCategory, targetID, _DescTitile + sb.ToString());
            _BeforeData.Clear();
            _AfterData.Clear();
        }

        /// <summary>
        /// 简易 log,(ActionBy,Actions,Desc)
        /// </summary>
        /// <param name="ActionBy"></param>
        /// <param name="Action"></param>
        /// <param name="Desc"></param>
        public void SaveLog(string ActionBy, string Action, string Desc)
        {
            Desc = _DescTitile + Desc;
            FISCA.LogAgent.ApplicationLog.Log(ActionBy, Action, Desc);
        }

        /// <summary>
        /// 简易 log2,(ActionBy,Actions,targetCategory:目标分类,targetID:辨识ID,Desc)
        /// </summary>
        /// <param name="ActionBy"></param>
        /// <param name="Actions"></param>
        /// <param name="targetCategory"></param>
        /// <param name="targetID"></param>
        /// <param name="Desc"></param>
        public void SaveLog(string ActionBy, string Actions, string targetCategory, string targetID, string Desc)
        {
            Desc = _DescTitile + Desc;
            FISCA.LogAgent.ApplicationLog.Log(ActionBy, Actions, targetCategory, targetID, Desc);
        }


    }
}

