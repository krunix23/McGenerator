using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;

namespace McGenerator
{
    class ConfigHandler
    {
        private Logger logger_;
        XmlDocument cfgdoc_;

        public ConfigHandler(Logger logger)
        {
            logger_ = logger;

            try
            {
                cfgdoc_ = new XmlDocument();
                cfgdoc_.Load("config.xml");
            }
            catch( SystemException ex )
            {
                logger_.Log(string.Format("{0}(): {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message), LogCategory.lcError);
            }
            logger_.Log("Created ConfigHandler", LogCategory.lcMessage);
        }

        public string[] GetAllTypes()
        {
            try
            {
                XmlNodeList elemList = cfgdoc_.GetElementsByTagName("camera");
                string[] sResults = new string[elemList.Count];

                for (int i = 0; i < elemList.Count; i++)
                {
                    string attrType = elemList[i].Attributes["type"].Value;
                    string attrMac = elemList[i].Attributes["mac"].Value;
                    logger_.Log(string.Format("Definition found for: {0} - {1}", attrType, attrMac), LogCategory.lcMessage);
                    sResults[i] = attrType;
                }
                return sResults;
            }
            catch( SystemException ex)
            {
                logger_.Log(string.Format("{0}(): {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message), LogCategory.lcError);
                return null;
            }
        }

        public string GetDefaultMAC(string type)
        {
            try
            {
                XmlNodeList elemList = cfgdoc_.GetElementsByTagName("camera");

                for (int i = 0; i < elemList.Count; i++)
                {
                    if (elemList[i].Attributes["type"].Value == type)
                    {
                        return elemList[i].Attributes["mac"].Value;
                    }
                }
            }
            catch (SystemException ex)
            {
                logger_.Log(string.Format("{0}(): {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message), LogCategory.lcError);
            }
            return string.Empty;
        }

        public static Int64 MACStringToInt64(string smac)
        {
            Int64 iResult = 0;

            if(smac != string.Empty)
            {
                try
                {
                    string value = smac.Replace(":", "");
                    iResult = Convert.ToInt64(value, 16);
                }
                catch {}
            }
            return iResult;
        }

        public static string MACInt64ToString(Int64 imac)
        {
            if(imac != 0)
            {
                string sMac = string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
                     ((imac >> 40) & 0xff),
                     ((imac >> 32) & 0xff),
                     ((imac >> 24) & 0xff),
                     ((imac >> 16) & 0xff),
                     ((imac >> 8) & 0xff),
                     ((imac >> 0) & 0xff));
                return sMac;
            }
            return string.Empty;
        }

        public void GenerateSpreadsheet(string type, string mac1st, decimal nummacs)
        {
            if (mac1st == string.Empty || Decimal.ToInt32(nummacs) == 0)
            {
                return;
            }

            try
            {
                string[] sMacs = new string[Decimal.ToInt32(nummacs)];
                XmlNodeList elemList = cfgdoc_.GetElementsByTagName("camera");
                int iStepWidth = 0;
                SLDocument slDoc = new SLDocument();
                slDoc.SetColumnWidth("A", 25.0);

                for (int i = 0; i < elemList.Count; i++)
                {
                    if (elemList[i].Attributes["type"].Value == type)
                    {
                        iStepWidth = int.Parse(elemList[i].Attributes["step"].Value);
                    }
                }

                sMacs[0] = mac1st;

                for( int i = 1; i < Decimal.ToInt32(nummacs); i++)
                {
                    
                    string tmpMAC = sMacs[i - 1];
                    Int64 iMAC = MACStringToInt64(tmpMAC);
                    iMAC += iStepWidth;
                    sMacs[i] = MACInt64ToString(iMAC);
                }

                for (int i = 0; i < sMacs.Length; i++ )
                {
                    string sCell = "A" + (i + 1).ToString();
                    slDoc.SetCellValue(sCell, sMacs[i]);
                    logger_.Log(string.Format("New MAC: {0}", sMacs[i]), LogCategory.lcMessage);
                }

                slDoc.SaveAs(type + ".xlsx");
            }
            catch(SystemException ex)
            {
                logger_.Log(string.Format("{0}(): {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message), LogCategory.lcError);
            }
        }
    }
}
